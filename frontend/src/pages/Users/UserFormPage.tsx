import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { ArrowLeft, Save, User as UserIcon } from 'lucide-react';

import { usersApi } from '@/api/endpoints/users.api';
import { vectorsApi } from '@/api/endpoints/vectors.api';
import { useAuthStore } from '@/store/auth.store';
import { useToast } from '@/components/common/Toast';
import { useI18n } from '@/hooks/useI18n';
import { Permission } from '@/types/auth.types';

import { Input } from '@/components/common/Input';
import { Button } from '@/components/common/Button';
import { Card } from '@/components/common/Card';
import { Alert } from '@/components/common/Alert';
import { Loading } from '@/components/common/Loading';

// Schema de validação com Zod dinâmico
const createUserFormSchema = (t: any) => {
  const baseSchema = z.object({
    name: z.string().min(1, t('users.validation.nameRequired')),
    email: z.string().email(t('users.validation.emailInvalid')).min(1, t('users.validation.emailRequired')),
    password: z.string().optional(),
    permission: z.nativeEnum(Permission, {
      required_error: t('users.validation.profileRequired'),
    }),
    vectorId: z.string().optional(),
    isActive: z.boolean().default(true),
  }).refine(
    (data) => {
      // Regra: AdminGlobal não pode ter vetor
      if (data.permission === 'AdminGlobal' && data.vectorId) {
        return false;
      }
      return true;
    },
    {
      message: t('users.validation.adminGlobalNoVector'),
      path: ['vectorId'],
    }
  ).refine(
    (data) => {
      // Regra: Outros perfis devem ter vetor
      if (data.permission !== 'AdminGlobal' && !data.vectorId) {
        return false;
      }
      return true;
    },
    {
      message: t('users.validation.vectorRequired'),
      path: ['vectorId'],
    }
  ).refine(
    (data) => {
      // Regra: Senha deve ter no mínimo 6 caracteres quando fornecida
      if (data.password && data.password.length < 6) {
        return false;
      }
      return true;
    },
    {
      message: t('users.validation.passwordMin'),
      path: ['password'],
    }
  );
  
  return baseSchema;
};

const createCreateUserSchema = (t: any) => {
  return createUserFormSchema(t).refine(
    (data) => {
      if (!data.password || data.password.length === 0) {
        return false;
      }
      return true;
    },
    {
      message: t('users.validation.passwordRequiredOnCreate'),
      path: ['password'],
    }
  );
};

export function UserFormPage() {
  const { t } = useI18n();
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const { user: currentUser } = useAuthStore();
  const queryClient = useQueryClient();

  const isEditMode = !!id;
  const isAdminGlobal = currentUser?.permission === Permission.AdminGlobal;

  // Query para carregar usuário em modo de edição
  const { data: user, isLoading: isLoadingUser, error: userError } = useQuery({
    queryKey: ['user', id],
    queryFn: async () => {
      const result = await usersApi.getById(id!);
      return result;
    },
    enabled: isEditMode,
  });

  // Query para carregar vetores (apenas AdminGlobal)
  const { data: vectorsResponse, isLoading: isLoadingVectors } = useQuery({
    queryKey: ['vectors', 'all'],
    queryFn: () => vectorsApi.list({ page: 1, pageSize: 100 }),
    enabled: isAdminGlobal,
  });

  const vectors = vectorsResponse?.items || [];
  
  // Define o tipo para o form data
  type UserFormData = z.infer<ReturnType<typeof createUserFormSchema>>;

  // Form setup com React Hook Form + Zod
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    watch,
    setValue,
    reset,
  } = useForm<UserFormData>({
    resolver: zodResolver(isEditMode ? createUserFormSchema(t) : createCreateUserSchema(t)),
    defaultValues: {
      name: '',
      email: '',
      password: '',
      permission: Permission.Operador,
      vectorId: '',
      isActive: true,
    },
  });

  // Watch permission para controlar visibilidade do campo vetor
  const selectedPermission = watch('permission');

  // Preencher form quando carregar usuário (modo edição)
  useEffect(() => {
    if (user && isEditMode) {
      reset({
        name: user.name,
        email: user.email,
        password: '',
        permission: user.permission,
        vectorId: user.vectorId || '',
        isActive: user.isActive,
      });
    }
  }, [user, isEditMode, reset]);

  // Limpar vectorId quando selecionar AdminGlobal
  useEffect(() => {
    if (selectedPermission === Permission.AdminGlobal) {
      setValue('vectorId', '');
    }
  }, [selectedPermission, setValue]);

  // Mutation para criar usuário
  const createMutation = useMutation({
    mutationFn: usersApi.create,
    onSuccess: () => {
      // Invalidar cache da listagem de usuários
      queryClient.invalidateQueries({ queryKey: ['users'] });
      showToast('success', t('users.userCreatedSuccess'));
      navigate('/usuarios');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || t('users.errorCreatingUser');
      showToast('error', message);
    },
  });

  // Mutation para atualizar usuário
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: any }) => usersApi.update(id, data),
    onSuccess: () => {
      // Invalidar cache da listagem de usuários e do usuário específico
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['user', id] });
      showToast('success', t('users.userUpdatedSuccess'));
      navigate('/usuarios');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || t('users.errorUpdatingUser');
      showToast('error', message);
    },
  });

  // Submit handler
  const onSubmit = (data: UserFormData) => {
    const payload: any = {
      name: data.name,
      email: data.email,
      permission: data.permission,
      vetorId: data.vectorId && data.vectorId !== '' ? data.vectorId : null,
      isActive: data.isActive,
    };

    // Incluir senha apenas se fornecida
    if (data.password && data.password.length > 0) {
      payload.password = data.password;
    }

    if (isEditMode && id) {
      updateMutation.mutate({ id, data: payload });
    } else {
      createMutation.mutate(payload);
    }
  };

  // Loading state
  if (isEditMode && isLoadingUser) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Loading />
      </div>
    );
  }

  // Error state
  if (isEditMode && userError) {
    return (
      <div className="space-y-4">
        <Alert type="error">
          {t('users.errorLoadingUser')}
        </Alert>
        <Button variant="outline" onClick={() => navigate('/usuarios')}>
          <ArrowLeft className="w-4 h-4 mr-2" />
          {t('users.backToList')}
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="outline" size="sm" onClick={() => navigate('/usuarios')}>
            <ArrowLeft className="w-4 h-4" />
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">
              {isEditMode ? t('users.editUser') : t('users.newUser')}
            </h1>
            <p className="text-sm text-gray-500 mt-1">
              {isEditMode
                ? t('users.updateUserInfo')
                : t('users.fillDataToCreate')}
            </p>
          </div>
        </div>
        <UserIcon className="w-8 h-8 text-gray-400" />
      </div>

      {/* Form */}
      <Card>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Informações Básicas */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">
              {t('users.form.basicInfo')}
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Nome */}
              <div>
                <Input
                  label={t('users.form.name')}
                  placeholder={t('users.form.namePlaceholder')}
                  error={errors.name?.message}
                  {...register('name')}
                  required
                />
              </div>

              {/* Email */}
              <div>
                <Input
                  label={t('users.form.email')}
                  type="email"
                  placeholder={t('users.form.emailPlaceholder')}
                  error={errors.email?.message}
                  {...register('email')}
                  required
                />
              </div>
            </div>

            {/* Senha */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <Input
                  label={isEditMode ? t('users.form.newPassword') : t('users.form.password')}
                  type="password"
                  placeholder={isEditMode ? t('users.form.passwordOptionalPlaceholder') : t('users.form.passwordPlaceholder')}
                  error={errors.password?.message}
                  {...register('password')}
                  required={!isEditMode}
                />
                {isEditMode && (
                  <p className="text-xs text-gray-500 mt-1">
                    {t('users.form.leaveBlankToKeep')}
                  </p>
                )}
              </div>
            </div>
          </div>

          {/* Permissões e Acessos */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">
              {t('users.form.permissionsAndAccess')}
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Perfil */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {t('users.form.role')} <span className="text-red-500">*</span>
                </label>
                <select
                  className={`w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-black focus:border-transparent ${
                    errors.permission ? 'border-red-500' : 'border-gray-300'
                  }`}
                  {...register('permission')}
                >
                  <option value="">{t('users.form.selectProfile')}</option>
                  <option value={Permission.AdminGlobal}>{t('users.permissions.AdminGlobal')}</option>
                  <option value={Permission.AdminVetor}>{t('users.permissions.AdminVetor')}</option>
                  <option value={Permission.Operador}>{t('users.permissions.Operador')}</option>
                </select>
                {errors.permission && (
                  <p className="text-xs text-red-500 mt-1">{errors.permission.message}</p>
                )}
                <p className="text-xs text-gray-500 mt-1">
                  {selectedPermission === Permission.AdminGlobal && t('users.form.permissionDescriptions.AdminGlobal')}
                  {selectedPermission === Permission.AdminVetor && t('users.form.permissionDescriptions.AdminVetor')}
                  {selectedPermission === Permission.Operador && t('users.form.permissionDescriptions.Operador')}
                </p>
              </div>

              {/* Vetor (apenas se não for AdminGlobal) */}
              {selectedPermission !== Permission.AdminGlobal && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    {t('users.form.vector')} <span className="text-red-500">*</span>
                  </label>
                  {isLoadingVectors ? (
                    <div className="flex items-center justify-center py-2">
                      <Loading size="sm" />
                    </div>
                  ) : isAdminGlobal ? (
                    <>
                      <select
                        className={`w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-black focus:border-transparent ${
                          errors.vectorId ? 'border-red-500' : 'border-gray-300'
                        }`}
                        {...register('vectorId')}
                      >
                        <option value="">{t('users.form.selectVector')}</option>
                        {vectors
                          .filter((v) => v.isActive)
                          .map((vector) => (
                            <option key={vector.id} value={vector.id}>
                              {vector.name}
                            </option>
                          ))}
                      </select>
                      {errors.vectorId && (
                        <p className="text-xs text-red-500 mt-1">{errors.vectorId.message}</p>
                      )}
                    </>
                  ) : (
                    <Input
                      value={currentUser?.vectorName || t('users.form.currentVector')}
                      disabled
                      readOnly
                    />
                  )}
                </div>
              )}
            </div>

            {/* Alertas sobre permissões */}
            {selectedPermission === Permission.AdminGlobal && (
              <Alert type="info">
                <strong>{t('common.attention')}:</strong> {t('users.form.alerts.adminGlobalNoVector')}
              </Alert>
            )}

            {selectedPermission === Permission.AdminVetor && (
              <Alert type="warning">
                <strong>{t('common.important')}:</strong> {t('users.form.alerts.adminVetorOnePerVector')}
              </Alert>
            )}
          </div>

          {/* Status */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">{t('users.form.statusSection')}</h2>

            <div className="flex items-center gap-3">
              <input
                type="checkbox"
                id="isActive"
                className="w-4 h-4 text-black border-gray-300 rounded focus:ring-black"
                {...register('isActive')}
              />
              <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
                {t('users.form.userActive')}
              </label>
            </div>
            <p className="text-xs text-gray-500">
              {t('users.form.inactiveUsersCannotLogin')}
            </p>
          </div>

          {/* Actions */}
          <div className="flex items-center justify-end gap-3 pt-4 border-t">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/usuarios')}
              disabled={isSubmitting}
            >
              {t('common.cancel')}
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting}
              isLoading={isSubmitting}
            >
              <Save className="w-4 h-4 mr-2" />
              {isEditMode ? t('users.saveChanges') : t('users.createUser')}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
