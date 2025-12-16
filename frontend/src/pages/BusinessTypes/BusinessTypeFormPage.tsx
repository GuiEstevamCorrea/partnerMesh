import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import { ArrowLeft } from 'lucide-react';

import { businessTypesApi } from '@/api/endpoints/businessTypes.api';
import { useToast } from '@/components/common/Toast';
import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Card } from '@/components/common/Card';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';

// Schema de validação
const businessTypeFormSchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório'),
  description: z.string().optional(),
  isActive: z.boolean(),
});

type BusinessTypeFormData = z.infer<typeof businessTypeFormSchema>;

export function BusinessTypeFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const { showToast } = useToast();
  const isEditMode = !!id;

  // Form setup
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<BusinessTypeFormData>({
    resolver: zodResolver(businessTypeFormSchema),
    defaultValues: {
      name: '',
      description: '',
      isActive: true,
    },
  });

  // Query: Carregar tipo de negócio (edit mode)
  const { data: businessType, isLoading: isLoadingBusinessType } = useQuery({
    queryKey: ['business-type', id],
    queryFn: () => businessTypesApi.getById(id!),
    enabled: isEditMode,
  });

  // Mutation: Create
  const createMutation = useMutation({
    mutationFn: businessTypesApi.create,
    onSuccess: () => {
      showToast('success', 'Tipo de negócio criado com sucesso!');
      navigate('/tipos-negocio');
    },
    onError: () => {
      showToast('error', 'Erro ao criar tipo de negócio');
    },
  });

  // Mutation: Update
  const updateMutation = useMutation({
    mutationFn: (data: BusinessTypeFormData) => businessTypesApi.update(id!, data),
    onSuccess: () => {
      showToast('success', 'Tipo de negócio atualizado com sucesso!');
      navigate('/tipos-negocio');
    },
    onError: () => {
      showToast('error', 'Erro ao atualizar tipo de negócio');
    },
  });

  // Preencher formulário em modo edição
  useEffect(() => {
    if (businessType) {
      reset({
        name: businessType.name,
        description: businessType.description || '',
        isActive: businessType.isActive,
      });
    }
  }, [businessType, reset]);

  // Submit handler
  const onSubmit = (data: BusinessTypeFormData) => {
    if (isEditMode) {
      updateMutation.mutate(data);
    } else {
      createMutation.mutate({
        name: data.name,
        description: data.description,
      });
    }
  };

  if (isEditMode && isLoadingBusinessType) {
    return <Loading />;
  }

  return (
    <div className="p-6 max-w-4xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button
          variant="outline"
          onClick={() => navigate('/tipos-negocio')}
          className="flex items-center gap-2"
        >
          <ArrowLeft className="w-4 h-4" />
          Voltar
        </Button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditMode ? 'Editar Tipo de Negócio' : 'Novo Tipo de Negócio'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditMode
              ? 'Atualize as informações do tipo de negócio'
              : 'Preencha os dados para criar um novo tipo de negócio'}
          </p>
        </div>
      </div>

      {/* Formulário */}
      <form onSubmit={handleSubmit(onSubmit)}>
        <Card>
          <div className="space-y-6">
            <Alert type="info">
              <p className="text-sm">
                <strong>Tipos de Negócio</strong> são usados para categorizar os negócios cadastrados no sistema.
                Apenas tipos ativos podem ser utilizados em novos negócios.
              </p>
            </Alert>

            {/* Nome */}
            <div>
              <label htmlFor="name" className="block text-sm font-medium text-gray-900 mb-2">
                Nome <span className="text-red-500">*</span>
              </label>
              <Input
                id="name"
                type="text"
                placeholder="Ex: Venda de Produto, Prestação de Serviço..."
                {...register('name')}
                disabled={isSubmitting}
              />
              {errors.name && (
                <p className="text-sm text-red-600 mt-1">{errors.name.message}</p>
              )}
              <p className="text-sm text-gray-600 mt-1">
                Nome único que identifica o tipo de negócio
              </p>
            </div>

            {/* Descrição */}
            <div>
              <label htmlFor="description" className="block text-sm font-medium text-gray-900 mb-2">
                Descrição
              </label>
              <textarea
                id="description"
                rows={4}
                placeholder="Descreva as características deste tipo de negócio..."
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black focus:border-transparent resize-none"
                {...register('description')}
                disabled={isSubmitting}
              />
              {errors.description && (
                <p className="text-sm text-red-600 mt-1">{errors.description.message}</p>
              )}
              <p className="text-sm text-gray-600 mt-1">
                Informações adicionais sobre este tipo de negócio (opcional)
              </p>
            </div>

            {/* Status - Apenas em modo edição */}
            {isEditMode && (
              <div>
                <div className="flex items-center gap-3">
                  <input
                    id="isActive"
                    type="checkbox"
                    className="w-4 h-4 text-black border-gray-300 rounded focus:ring-black"
                    {...register('isActive')}
                    disabled={isSubmitting}
                  />
                  <label htmlFor="isActive" className="text-sm font-medium text-gray-900">
                    Tipo de negócio ativo
                  </label>
                </div>
                <p className="text-sm text-gray-600 mt-2 ml-7">
                  Tipos inativos não podem ser utilizados em novos negócios
                </p>
              </div>
            )}

            {/* Informações adicionais em modo edição */}
            {isEditMode && businessType && (
              <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
                <h3 className="text-sm font-semibold text-gray-900 mb-2">Informações do Cadastro</h3>
                <div className="space-y-1 text-sm text-gray-600">
                  <p>
                    <strong>Status:</strong>{' '}
                    <span className={businessType.isActive ? 'text-green-600' : 'text-red-600'}>
                      {businessType.isActive ? 'Ativo' : 'Inativo'}
                    </span>
                  </p>
                  <p>
                    <strong>Criado em:</strong>{' '}
                    {new Date(businessType.createdAt).toLocaleDateString('pt-BR')}
                  </p>
                  {businessType.updatedAt && (
                    <p>
                      <strong>Última atualização:</strong>{' '}
                      {new Date(businessType.updatedAt).toLocaleDateString('pt-BR')}
                    </p>
                  )}
                </div>
              </div>
            )}

            {/* Botões */}
            <div className="flex gap-3 pt-4">
              <Button
                type="submit"
                disabled={isSubmitting}
                className="flex-1"
              >
                {isSubmitting ? 'Salvando...' : isEditMode ? 'Atualizar' : 'Criar Tipo de Negócio'}
              </Button>
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate('/tipos-negocio')}
                disabled={isSubmitting}
              >
                Cancelar
              </Button>
            </div>
          </div>
        </Card>
      </form>
    </div>
  );
}
