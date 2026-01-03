import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import { ArrowLeft, Save, Network } from 'lucide-react';

import { vectorsApi } from '@/api/endpoints/vectors.api';
import { useToast } from '@/components/common/Toast';
import { useI18n } from '@/hooks/useI18n';

import { Input } from '@/components/common/Input';
import { Button } from '@/components/common/Button';
import { Card } from '@/components/common/Card';
import { Alert } from '@/components/common/Alert';
import { Loading } from '@/components/common/Loading';

// Schema de validação com Zod dinâmico
const createVectorFormSchema = (t: (key: string) => string) =>
  z.object({
    name: z.string().min(1, t('vectors.validation.nameRequired')),
    email: z.string().email(t('vectors.validation.emailInvalid')).min(1, t('vectors.validation.emailRequired')),
    login: z.string().min(1, t('vectors.validation.loginRequired')),
    isActive: z.boolean().default(true),
  });

type VectorFormData = z.infer<ReturnType<typeof createVectorFormSchema>>;

export function VectorFormPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const { t } = useI18n();

  const isEditMode = !!id;

  // Query para carregar vetor em modo de edição
  const { data: vector, isLoading: isLoadingVector, error: vectorError } = useQuery({
    queryKey: ['vector', id],
    queryFn: () => vectorsApi.getById(id!),
    enabled: isEditMode,
  });

  // Form setup com React Hook Form + Zod
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<VectorFormData>({
    resolver: zodResolver(createVectorFormSchema(t)),
    defaultValues: {
      name: '',
      email: '',
      login: '',
      isActive: true,
    },
  });

  // Preencher form quando carregar vetor (modo edição)
  useEffect(() => {
    if (vector && isEditMode) {
      reset({
        name: vector.name,
        email: vector.email,
        login: vector.login,
        isActive: vector.isActive,
      });
    }
  }, [vector, isEditMode, reset]);

  // Mutation para criar vetor
  const createMutation = useMutation({
    mutationFn: vectorsApi.create,
    onSuccess: () => {
      showToast('success', t('vectors.vectorCreated'));
      navigate('/vetores');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || t('vectors.vectorError');
      showToast('error', message);
    },
  });

  // Mutation para atualizar vetor
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: any }) => vectorsApi.update(id, data),
    onSuccess: () => {
      showToast('success', t('vectors.vectorUpdated'));
      navigate('/vetores');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || t('vectors.vectorError');
      showToast('error', message);
    },
  });

  // Submit handler
  const onSubmit = (data: VectorFormData) => {
    const payload = {
      name: data.name,
      email: data.email,
      login: data.login,
      isActive: data.isActive,
    };

    if (isEditMode && id) {
      updateMutation.mutate({ id, data: payload });
    } else {
      createMutation.mutate(payload);
    }
  };

  // Loading state
  if (isEditMode && isLoadingVector) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Loading />
      </div>
    );
  }

  // Error state
  if (isEditMode && vectorError) {
    return (
      <div className="space-y-4">
        <Alert type="error">
          {t('vectors.form.errorLoading')}
        </Alert>
        <Button variant="outline" onClick={() => navigate('/vetores')}>
          <ArrowLeft className="w-4 h-4 mr-2" />
          {t('vectors.form.backToList')}
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="outline" size="sm" onClick={() => navigate('/vetores')}>
            <ArrowLeft className="w-4 h-4" />
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">
              {isEditMode ? t('vectors.form.editTitle') : t('vectors.form.title')}
            </h1>
            <p className="text-sm text-gray-500 mt-1">
              {isEditMode
                ? t('vectors.form.editSubtitle')
                : t('vectors.form.subtitle')}
            </p>
          </div>
        </div>
        <Network className="w-8 h-8 text-gray-400" />
      </div>

      {/* Avisos Importantes */}
      {!isEditMode && (
        <Alert type="info">
          <strong>{t('common.attention')}:</strong> {t('vectors.form.createAlert')}
        </Alert>
      )}

      {/* Form */}
      <Card>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Informações Básicas */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">
              {t('vectors.form.basicInfo')}
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Nome */}
              <div>
                <Input
                  label={t('vectors.form.name')}
                  placeholder={t('vectors.form.namePlaceholder')}
                  error={errors.name?.message}
                  {...register('name')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {t('vectors.form.nameHelp')}
                </p>
              </div>

              {/* Email */}
              <div>
                <Input
                  label={t('vectors.form.email')}
                  type="email"
                  placeholder={t('vectors.form.emailPlaceholder')}
                  error={errors.email?.message}
                  {...register('email')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {t('vectors.form.emailHelp')}
                </p>
              </div>

              {/* Login */}
              <div>
                <Input
                  label={t('vectors.form.login')}
                  placeholder={t('vectors.form.loginPlaceholder')}
                  error={errors.login?.message}
                  {...register('login')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {t('vectors.form.loginHelp')}
                </p>
              </div>
            </div>
          </div>

          {/* Status */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">{t('vectors.form.statusSection')}</h2>

            <div className="flex items-center gap-3">
              <input
                type="checkbox"
                id="isActive"
                className="w-4 h-4 text-black border-gray-300 rounded focus:ring-black"
                {...register('isActive')}
              />
              <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
                {t('vectors.form.isActive')}
              </label>
            </div>
            <p className="text-xs text-gray-500">
              {t('vectors.form.isActiveHelp')}
            </p>
          </div>

          {/* Informações sobre Administrador (apenas modo criação) */}
          {!isEditMode && (
            <Alert type="warning">
              <strong>{t('common.important')}:</strong> {t('vectors.form.createWarning')}
              <ul className="list-disc list-inside mt-2 space-y-1">
                <li>{t('vectors.form.createAdminVetor')}</li>
                <li>{t('vectors.form.ensureAdmin')}</li>
              </ul>
            </Alert>
          )}

          {/* Informações sobre Parceiros (modo edição) */}
          {isEditMode && vector && (
            <div className="bg-gray-50 rounded-lg p-4">
              <h3 className="text-sm font-semibold text-gray-900 mb-2">{t('vectors.form.vectorInfo')}</h3>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-gray-500">{t('vectors.form.currentLogin')}</span>
                  <span className="ml-2 font-medium text-gray-900">{vector.login}</span>
                </div>
                <div>
                  <span className="text-gray-500">{t('vectors.form.currentStatus')}</span>
                  <span className={`ml-2 font-medium ${vector.isActive ? 'text-green-600' : 'text-red-600'}`}>
                    {vector.isActive ? t('common.active') : t('common.inactive')}
                  </span>
                </div>
              </div>
            </div>
          )}

          {/* Actions */}
          <div className="flex items-center justify-end gap-3 pt-4 border-t">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/vetores')}
              disabled={isSubmitting}
            >
              {t('vectors.form.cancel')}
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting}
              isLoading={isSubmitting}
            >
              <Save className="w-4 h-4 mr-2" />
              {isEditMode ? t('vectors.form.save') : t('vectors.form.create')}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
