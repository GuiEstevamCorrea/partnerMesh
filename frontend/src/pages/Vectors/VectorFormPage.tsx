import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import { ArrowLeft, Save, Network } from 'lucide-react';

import { vectorsApi } from '@/api/endpoints/vectors.api';
import { useToast } from '@/components/common/Toast';

import { Input } from '@/components/common/Input';
import { Button } from '@/components/common/Button';
import { Card } from '@/components/common/Card';
import { Alert } from '@/components/common/Alert';
import { Loading } from '@/components/common/Loading';

// Schema de validação com Zod
const vectorFormSchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório'),
  email: z.string().email('Email inválido').min(1, 'Email é obrigatório'),
  login: z.string().min(1, 'Login é obrigatório'),
  isActive: z.boolean().default(true),
});

type VectorFormData = z.infer<typeof vectorFormSchema>;

export function VectorFormPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();

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
    resolver: zodResolver(vectorFormSchema),
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
      showToast('success', 'Vetor criado com sucesso!');
      navigate('/vetores');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao criar vetor';
      showToast('error', message);
    },
  });

  // Mutation para atualizar vetor
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: any }) => vectorsApi.update(id, data),
    onSuccess: () => {
      showToast('success', 'Vetor atualizado com sucesso!');
      navigate('/vetores');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao atualizar vetor';
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
          Erro ao carregar vetor. Verifique se o ID é válido e tente novamente.
        </Alert>
        <Button variant="outline" onClick={() => navigate('/vetores')}>
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar para Lista
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
              {isEditMode ? 'Editar Vetor' : 'Novo Vetor'}
            </h1>
            <p className="text-sm text-gray-500 mt-1">
              {isEditMode
                ? 'Atualize as informações do vetor'
                : 'Preencha os dados para criar um novo vetor'}
            </p>
          </div>
        </div>
        <Network className="w-8 h-8 text-gray-400" />
      </div>

      {/* Avisos Importantes */}
      {!isEditMode && (
        <Alert type="info">
          <strong>Atenção:</strong> Ao criar um vetor, você precisará posteriormente criar um usuário com perfil AdminVetor para gerenciá-lo.
        </Alert>
      )}

      {/* Form */}
      <Card>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Informações Básicas */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">
              Informações do Vetor
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Nome */}
              <div>
                <Input
                  label="Nome"
                  placeholder="Digite o nome do vetor"
                  error={errors.name?.message}
                  {...register('name')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  Nome único que identifica o vetor no sistema
                </p>
              </div>

              {/* Email */}
              <div>
                <Input
                  label="Email"
                  type="email"
                  placeholder="contato@vetor.com"
                  error={errors.email?.message}
                  {...register('email')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  Email de contato do vetor (deve ser único)
                </p>
              </div>

              {/* Login */}
              <div>
                <Input
                  label="Login"
                  placeholder="Digite o login de acesso"
                  error={errors.login?.message}
                  {...register('login')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  Login único para acesso administrativo do vetor
                </p>
              </div>
            </div>
          </div>

          {/* Status */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">Status</h2>

            <div className="flex items-center gap-3">
              <input
                type="checkbox"
                id="isActive"
                className="w-4 h-4 text-black border-gray-300 rounded focus:ring-black"
                {...register('isActive')}
              />
              <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
                Vetor ativo
              </label>
            </div>
            <p className="text-xs text-gray-500">
              Vetores inativos não podem ter novos parceiros ou negócios cadastrados
            </p>
          </div>

          {/* Informações sobre Administrador (apenas modo criação) */}
          {!isEditMode && (
            <Alert type="warning">
              <strong>Importante:</strong> Após criar o vetor, não esqueça de:
              <ul className="list-disc list-inside mt-2 space-y-1">
                <li>Criar um usuário com perfil <strong>AdminVetor</strong> e associá-lo a este vetor</li>
                <li>Garantir que haja pelo menos um AdminVetor ativo para gerenciar o vetor</li>
              </ul>
            </Alert>
          )}

          {/* Informações sobre Parceiros (modo edição) */}
          {isEditMode && vector && (
            <div className="bg-gray-50 rounded-lg p-4">
              <h3 className="text-sm font-semibold text-gray-900 mb-2">Informações do Vetor</h3>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-gray-500">Login:</span>
                  <span className="ml-2 font-medium text-gray-900">{vector.login}</span>
                </div>
                <div>
                  <span className="text-gray-500">Status Atual:</span>
                  <span className={`ml-2 font-medium ${vector.isActive ? 'text-green-600' : 'text-red-600'}`}>
                    {vector.isActive ? 'Ativo' : 'Inativo'}
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
              Cancelar
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting}
              isLoading={isSubmitting}
            >
              <Save className="w-4 h-4 mr-2" />
              {isEditMode ? 'Salvar Alterações' : 'Criar Vetor'}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
