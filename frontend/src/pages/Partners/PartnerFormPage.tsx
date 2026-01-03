import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import { ArrowLeft, Save, Users } from 'lucide-react';

import { partnersApi } from '@/api/endpoints/partners.api';
import { vectorsApi } from '@/api/endpoints/vectors.api';
import { useToast } from '@/components/common/Toast';
import { useAuthStore } from '@/store/auth.store';

import { Input } from '@/components/common/Input';
import { Button } from '@/components/common/Button';
import { Card } from '@/components/common/Card';
import { Alert } from '@/components/common/Alert';
import { Loading } from '@/components/common/Loading';

// Schema de validação com Zod
const partnerFormSchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório'),
  contact: z.string().min(1, 'Contato é obrigatório'),
  vectorId: z.string().optional(),
  recommenderId: z.string().optional(),
  recommenderType: z.enum(['Partner', 'Vector']).optional(),
  isActive: z.boolean().default(true),
});

type PartnerFormData = z.infer<typeof partnerFormSchema>;

export function PartnerFormPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const { user } = useAuthStore();

  const isEditMode = !!id;
  const isAdminGlobal = user?.permission === 'AdminGlobal';
  const [showRecommenderWarning, setShowRecommenderWarning] = useState(false);

  // Query para carregar parceiro em modo de edição
  const { data: partner, isLoading: isLoadingPartner, error: partnerError } = useQuery({
    queryKey: ['partner', id],
    queryFn: () => partnersApi.getById(id!),
    enabled: isEditMode,
  });

  // Query para carregar lista de parceiros possíveis (recomendadores)
  const { data: partnersData, isLoading: isLoadingPartners } = useQuery({
    queryKey: ['partners-for-recommender'],
    queryFn: () => partnersApi.list({ pageSize: 1000 }),
  });

  // Query para carregar vetores (para AdminGlobal)
  const { data: vectorsData } = useQuery({
    queryKey: ['vectors-for-recommender'],
    queryFn: () => vectorsApi.list({ pageSize: 1000 }),
  });

  // Form setup com React Hook Form + Zod
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
    watch,
  } = useForm<PartnerFormData>({
    resolver: zodResolver(partnerFormSchema),
    defaultValues: {
      name: '',
      contact: '',
      vectorId: user?.vectorId || '',
      recommenderId: '',
      recommenderType: undefined,
      isActive: true,
    },
  });

  const watchRecommenderId = watch('recommenderId');
  const watchRecommenderType = watch('recommenderType');

  // Preencher form quando carregar parceiro (modo edição)
  useEffect(() => {
    if (partner && isEditMode) {
      reset({
        name: partner.name,
        contact: partner.contact,
        vectorId: partner.vectorId || user?.vectorId || '',
        recommenderId: partner.recommenderId || '',
        recommenderType: partner.recommenderType,
        isActive: partner.isActive,
      });
    }
  }, [partner, isEditMode, reset, user]);

  // Mostrar aviso quando não selecionar recomendador
  useEffect(() => {
    setShowRecommenderWarning(!watchRecommenderId);
  }, [watchRecommenderId]);

  // Mutation para criar parceiro
  const createMutation = useMutation({
    mutationFn: partnersApi.create,
    onSuccess: () => {
      showToast('success', 'Parceiro criado com sucesso!');
      navigate('/parceiros');
    },
    onError: (error: any) => {
      console.error('Erro ao criar parceiro:', error);
      console.error('Response data:', error.response?.data);
      const message = error.response?.data?.message || error.response?.data?.Message || 'Erro ao criar parceiro';
      showToast('error', message);
    },
  });

  // Mutation para atualizar parceiro
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: any }) => partnersApi.update(id, data),
    onSuccess: () => {
      showToast('success', 'Parceiro atualizado com sucesso!');
      navigate('/parceiros');
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao atualizar parceiro';
      showToast('error', message);
    },
  });

  // Submit handler
  const onSubmit = (data: PartnerFormData) => {
    // Determinar se o contato é email ou telefone
    const isEmail = data.contact.includes('@');
    
    // Determinar o vetorId
    const vetorId = data.vectorId || user?.vectorId;
    
    if (!vetorId) {
      showToast('error', 'Selecione um vetor para o parceiro');
      return;
    }
    
    // Preparar payload
    const payload: any = {
      name: data.name,
      phoneNumber: isEmail ? '00000000000' : data.contact, // Telefone dummy quando for email
      email: isEmail ? data.contact : `${data.contact.replace(/\D/g, '')}@temp.partnermesh.com`,
      isActive: data.isActive,
    };

    // Modo criação
    if (!isEditMode) {
      payload.vetorId = vetorId;
      
      // Se tiver recomendador selecionado E for do tipo Partner
      // Quando for Vector, não enviar recommenderId (backend usará o vetor automaticamente)
      if (data.recommenderId && data.recommenderType === 'Partner') {
        payload.recommenderId = data.recommenderId;
      }

      createMutation.mutate(payload);
    } else {
      // Modo edição (apenas nome, contato e status)
      const updatePayload = {
        name: data.name,
        contact: data.contact,
        isActive: data.isActive,
      };
      updateMutation.mutate({ id: id!, data: updatePayload });
    }
  };

  // Loading state
  if (isEditMode && isLoadingPartner) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Loading />
      </div>
    );
  }

  // Error state
  if (isEditMode && partnerError) {
    return (
      <div className="space-y-4">
        <Alert type="error">
          Erro ao carregar parceiro. Verifique se o ID é válido e tente novamente.
        </Alert>
        <Button variant="outline" onClick={() => navigate('/parceiros')}>
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar para Lista
        </Button>
      </div>
    );
  }

  const availablePartners = partnersData?.items?.filter((p) => p.id !== id) || [];
  const availableVectors = vectorsData?.items || [];

  // Encontrar recomendador selecionado para exibir hierarquia
  const selectedRecommender = watchRecommenderId
    ? watchRecommenderType === 'Partner'
      ? availablePartners.find((p) => p.id === watchRecommenderId)
      : availableVectors.find((v) => v.id === watchRecommenderId)
    : null;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="outline" size="sm" onClick={() => navigate('/parceiros')}>
            <ArrowLeft className="w-4 h-4" />
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">
              {isEditMode ? 'Editar Parceiro' : 'Novo Parceiro'}
            </h1>
            <p className="text-sm text-gray-500 mt-1">
              {isEditMode
                ? 'Atualize as informações do parceiro'
                : 'Preencha os dados para criar um novo parceiro'}
            </p>
          </div>
        </div>
        <Users className="w-8 h-8 text-gray-400" />
      </div>

      {/* Avisos Importantes */}
      {!isEditMode && showRecommenderWarning && (
        <Alert type="info">
          <strong>Atenção:</strong> Nenhum recomendador selecionado. O vetor "{user?.vectorName}" será usado como recomendador (Nível 1).
        </Alert>
      )}

      {isEditMode && (
        <Alert type="warning">
          <strong>Modo Edição:</strong> O recomendador não pode ser alterado após a criação do parceiro. Apenas nome, contato e status podem ser editados.
        </Alert>
      )}

      {/* Form */}
      <Card>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Informações Básicas */}
          <div className="space-y-4">
            <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">
              Informações do Parceiro
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Nome */}
              <div>
                <Input
                  label="Nome"
                  placeholder="Digite o nome do parceiro"
                  error={errors.name?.message}
                  {...register('name')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  Nome completo ou razão social do parceiro
                </p>
              </div>

              {/* Contato */}
              <div>
                <Input
                  label="Contato"
                  placeholder="email@exemplo.com ou (11) 99999-9999"
                  error={errors.contact?.message}
                  {...register('contact')}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  Email ou telefone para contato
                </p>
              </div>
            </div>

            {/* Vetor (apenas para AdminGlobal e modo criação) */}
            {!isEditMode && isAdminGlobal && (
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Vetor <span className="text-red-500">*</span>
                </label>
                <select
                  {...register('vectorId')}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black focus:border-transparent"
                  required
                >
                  <option value="">Selecione um vetor...</option>
                  {vectorsData?.items?.map((vector: any) => (
                    <option key={vector.id} value={vector.id}>
                      {vector.name}
                    </option>
                  ))}
                </select>
                <p className="text-xs text-gray-500 mt-1">
                  Vetor ao qual o parceiro será vinculado
                </p>
                {errors.vectorId && (
                  <p className="text-xs text-red-600 mt-1">{errors.vectorId.message}</p>
                )}
              </div>
            )}
          </div>

          {/* Recomendador (apenas modo criação) */}
          {!isEditMode && (
            <div className="space-y-4">
              <h2 className="text-lg font-semibold text-gray-900 border-b pb-2">
                Recomendador (Opcional)
              </h2>

              <div className="space-y-4">
                {/* Tipo de Recomendador */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Tipo de Recomendador
                  </label>
                  <select
                    {...register('recommenderType')}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black focus:border-transparent"
                    disabled={isLoadingPartners}
                  >
                    <option value="">Nenhum (usar vetor)</option>
                    <option value="Partner">Parceiro</option>
                    <option value="Vector">Vetor</option>
                  </select>
                  <p className="text-xs text-gray-500 mt-1">
                    Selecione se o recomendador é outro parceiro ou um vetor
                  </p>
                </div>

                {/* Recomendador */}
                {watchRecommenderType && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Recomendador
                    </label>
                    <select
                      {...register('recommenderId')}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black focus:border-transparent"
                      disabled={isLoadingPartners}
                    >
                      <option value="">Selecione...</option>
                      {watchRecommenderType === 'Partner' &&
                        availablePartners.map((p) => (
                          <option key={p.id} value={p.id}>
                            {p.name} - Nível {p.level}
                          </option>
                        ))}
                      {watchRecommenderType === 'Vector' &&
                        availableVectors.map((v) => (
                          <option key={v.id} value={v.id}>
                            {v.name}
                          </option>
                        ))}
                    </select>
                    <p className="text-xs text-gray-500 mt-1">
                      {watchRecommenderType === 'Partner'
                        ? 'Parceiro que recomendou este novo parceiro'
                        : 'Vetor que será o recomendador direto'}
                    </p>
                  </div>
                )}

                {/* Hierarquia do Recomendador */}
                {selectedRecommender && (
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                    <h3 className="text-sm font-semibold text-blue-900 mb-2">
                      Hierarquia do Recomendador
                    </h3>
                    <div className="text-sm text-blue-800">
                      <div className="flex items-center gap-2">
                        <span className="font-medium">{selectedRecommender.name}</span>
                        {watchRecommenderType === 'Partner' && 'level' in selectedRecommender && (
                          <span className="text-xs bg-blue-200 px-2 py-1 rounded">
                            Nível {selectedRecommender.level}
                          </span>
                        )}
                      </div>
                      <p className="text-xs mt-2 text-blue-600">
                        {watchRecommenderType === 'Partner'
                          ? `O novo parceiro será nível ${(selectedRecommender as any).level + 1}`
                          : 'O novo parceiro será nível 1 (diretamente vinculado ao vetor)'}
                      </p>
                    </div>
                  </div>
                )}

                <Alert type="warning">
                  <strong>Importante:</strong>
                  <ul className="list-disc list-inside mt-2 space-y-1 text-sm">
                    <li>O recomendador não pode ser alterado após a criação</li>
                    <li>O recomendador deve estar ativo</li>
                    <li>Não é permitido criar ciclos na árvore de recomendações</li>
                  </ul>
                </Alert>
              </div>
            </div>
          )}

          {/* Informações do Recomendador (modo edição) */}
          {isEditMode && partner && (
            <div className="bg-gray-50 rounded-lg p-4">
              <h3 className="text-sm font-semibold text-gray-900 mb-2">
                Informações da Rede
              </h3>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-gray-500">Recomendador:</span>
                  <span className="ml-2 font-medium text-gray-900">
                    {partner.recommenderName || 'Vetor'}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">Nível:</span>
                  <span className="ml-2 font-medium text-gray-900">
                    {partner.level}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">Recomendados:</span>
                  <span className="ml-2 font-medium text-gray-900">
                    {partner.totalRecommended}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">Vetor:</span>
                  <span className="ml-2 font-medium text-gray-900">
                    {partner.vectorName}
                  </span>
                </div>
              </div>
            </div>
          )}

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
                Parceiro ativo
              </label>
            </div>
            <p className="text-xs text-gray-500">
              Parceiros inativos não podem ter novos negócios cadastrados nem podem ser recomendadores
            </p>
          </div>

          {/* Actions */}
          <div className="flex items-center justify-end gap-3 pt-4 border-t">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/parceiros')}
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
              {isEditMode ? 'Salvar Alterações' : 'Criar Parceiro'}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
