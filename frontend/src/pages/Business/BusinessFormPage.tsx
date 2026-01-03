import { useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import { ArrowLeft, Save, Calculator, AlertCircle } from 'lucide-react';

import { businessApi } from '@/api/endpoints/business.api';
import { partnersApi } from '@/api/endpoints/partners.api';
import { businessTypesApi } from '@/api/endpoints/businessTypes.api';
import { useToast } from '@/components/common/Toast';
import { formatCurrency } from '@/utils/formatters';

import { Input } from '@/components/common/Input';
import { Button } from '@/components/common/Button';
import { Card } from '@/components/common/Card';
import { Alert } from '@/components/common/Alert';
import { Loading } from '@/components/common/Loading';

// Schema de validação para criação
const createBusinessSchema = z.object({
  partnerIds: z.array(z.string()).min(1, 'Pelo menos um parceiro deve ser selecionado'),
  businessTypeId: z.string().min(1, 'Tipo de negócio é obrigatório'),
  value: z.number().min(0.01, 'Valor deve ser maior que zero'),
  date: z.string().min(1, 'Data é obrigatória'),
  observations: z.string().optional(),
});

// Schema de validação para edição (apenas observações)
const updateBusinessSchema = z.object({
  observations: z.string().optional(),
});

type CreateBusinessFormData = z.infer<typeof createBusinessSchema>;
type UpdateBusinessFormData = z.infer<typeof updateBusinessSchema>;
type BusinessFormData = CreateBusinessFormData | UpdateBusinessFormData;

export function BusinessFormPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const isEditMode = !!id;

  // Estado para parceiros selecionados (múltipla seleção)
  const [selectedPartnerIds, setSelectedPartnerIds] = useState<string[]>([]);

  // Data de hoje formatada para o input date (YYYY-MM-DD)
  const today = new Date().toISOString().split('T')[0];

  // Query para carregar negócio em modo de edição
  const { data: business, isLoading: isLoadingBusiness } = useQuery({
    queryKey: ['business', id],
    queryFn: () => businessApi.getById(id!),
    enabled: isEditMode,
  });

  // Query para carregar parceiros (apenas em modo criação)
  const { data: partnersData, isLoading: isLoadingPartners } = useQuery({
    queryKey: ['partners-for-business'],
    queryFn: () => partnersApi.list({ pageSize: 1000 }),
    enabled: !isEditMode,
  });

  // Query para carregar tipos de negócio (apenas em modo criação)
  const { data: typesData, isLoading: isLoadingTypes, error: typesError } = useQuery({
    queryKey: ['business-types-for-business'],
    queryFn: () => businessTypesApi.list({ pageSize: 100 }),
    enabled: !isEditMode,
  });
  
  console.log('📋 BusinessFormPage - Query state:', {
    isEditMode,
    queryEnabled: !isEditMode,
    isLoadingTypes,
    hasError: !!typesError,
    typesError,
    typesData,
    hasItems: !!typesData?.items,
    itemsCount: typesData?.items?.length || 0
  });

  // Form setup com schema condicional
  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<BusinessFormData>({
    resolver: zodResolver(isEditMode ? updateBusinessSchema : createBusinessSchema),
    defaultValues: isEditMode
      ? {}
      : {
          partnerIds: [],
          date: today,
          value: 0,
          observations: '',
        },
  });

  // Watch value para calcular preview da comissão
  const watchValue = !isEditMode ? watch('value' as keyof BusinessFormData) : 0;

  // Calcular preview da comissão (10% do valor)
  const commissionPreview = useMemo(() => {
    const value = typeof watchValue === 'number' ? watchValue : parseFloat(String(watchValue)) || 0;
    return value * 0.1;
  }, [watchValue]);
  // Calcular valor por parceiro
  const valuePerPartner = useMemo(() => {
    const value = typeof watchValue === 'number' ? watchValue : parseFloat(String(watchValue)) || 0;
    const partnersCount = selectedPartnerIds.length || 1;
    return value / partnersCount;
  }, [watchValue, selectedPartnerIds.length]);
  // Preencher formulário em modo edição
  useEffect(() => {
    if (business && isEditMode) {
      setValue('observations', business.observations || '');
    }
  }, [business, isEditMode, setValue]);

  // Sincronizar parceiros selecionados com formulário
  useEffect(() => {
    if (!isEditMode) {
      setValue('partnerIds' as keyof BusinessFormData, selectedPartnerIds);
    }
  }, [selectedPartnerIds, setValue, isEditMode]);

  // Handler para seleção/desseleção de parceiros
  const handlePartnerToggle = (partnerId: string) => {
    setSelectedPartnerIds(prev => {
      if (prev.includes(partnerId)) {
        return prev.filter(id => id !== partnerId);
      } else {
        return [...prev, partnerId];
      }
    });
  };

  // Mutation para criar negócio
  const createMutation = useMutation({
    mutationFn: (data: CreateBusinessFormData) => businessApi.create(data),
    onSuccess: (newBusinesses) => {
      if (newBusinesses.length === 1) {
        showToast('success', 'Negócio criado com sucesso! Comissões calculadas automaticamente.');
        // Redirecionar para a página de detalhes do negócio
        navigate(`/negocios/${newBusinesses[0].id}`);
      } else {
        showToast('success', `${newBusinesses.length} negócios criados com sucesso! Comissões calculadas automaticamente.`);
        // Redirecionar para a lista de negócios
        navigate('/negocios');
      }
    },
    onError: (error: any) => {
      showToast('error', error.response?.data?.message || 'Erro ao criar negócio');
    },
  });

  // Mutation para atualizar negócio
  const updateMutation = useMutation({
    mutationFn: (data: UpdateBusinessFormData) => businessApi.update(id!, data),
    onSuccess: () => {
      showToast('success', 'Negócio atualizado com sucesso');
      navigate('/negocios');
    },
    onError: (error: any) => {
      showToast('error', error.response?.data?.message || 'Erro ao atualizar negócio');
    },
  });

  const onSubmit = (data: BusinessFormData) => {
    if (isEditMode) {
      updateMutation.mutate(data as UpdateBusinessFormData);
    } else {
      createMutation.mutate(data as CreateBusinessFormData);
    }
  };

  // Loading states
  if (isEditMode && isLoadingBusiness) {
    return <Loading />;
  }

  if (!isEditMode && (isLoadingPartners || isLoadingTypes)) {
    return <Loading />;
  }

  const activePartners = partnersData?.items?.filter((p) => p.isActive) || [];
  const activeTypes = typesData?.items?.filter((t) => t.isActive) || [];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="outline" onClick={() => navigate('/negocios')}>
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar
        </Button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditMode ? 'Editar Negócio' : 'Novo Negócio'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditMode
              ? 'Edite as observações do negócio'
              : 'Preencha os dados para criar um novo negócio'}
          </p>
        </div>
      </div>

      {/* Alert informativo */}
      {!isEditMode && (
        <Alert type="info">
          <AlertCircle className="w-4 h-4" />
          <div>
            <p className="font-medium">Cálculo Automático de Comissões</p>
            <p className="text-sm mt-1">
              Você pode selecionar um ou múltiplos parceiros para um negócio. Se múltiplos parceiros
              forem selecionados, o valor será dividido igualmente entre eles e será criado um negócio
              individual para cada parceiro. A comissão total será de 10% do valor de cada negócio,
              distribuída automaticamente para até 3 níveis da rede de cada parceiro.
            </p>
          </div>
        </Alert>
      )}

      {/* Alert para modo edição */}
      {isEditMode && (
        <Alert type="warning">
          <AlertCircle className="w-4 h-4" />
          <div>
            <p className="font-medium">Edição Limitada</p>
            <p className="text-sm mt-1">
              As comissões já foram calculadas. Apenas as observações podem ser editadas. Para
              alterar outros dados, é necessário cancelar o negócio e criar um novo.
            </p>
          </div>
        </Alert>
      )}

      {/* Informações do negócio (modo edição) */}
      {isEditMode && business && (
        <Card>
          <h3 className="text-lg font-semibold mb-4">Dados do Negócio</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">ID</label>
              <p className="text-gray-900 font-mono">#{business.id.slice(0, 8)}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Status</label>
              <span
                className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                  business.status === 'Active'
                    ? 'bg-green-100 text-green-800'
                    : 'bg-red-100 text-red-800'
                }`}
              >
                {business.status === 'Active' ? 'Ativo' : 'Cancelado'}
              </span>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Parceiro</label>
              <p className="text-gray-900">{business.partnerName}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Tipo de Negócio
              </label>
              <p className="text-gray-900">{business.businessTypeName}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Valor</label>
              <p className="text-gray-900 font-semibold text-green-600">
                {formatCurrency(business.value)}
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Comissão Total
              </label>
              <p className="text-gray-900 font-semibold text-blue-600">
                {formatCurrency(business.totalCommission || 0)}
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Data</label>
              <p className="text-gray-900">
                {new Date(business.date).toLocaleDateString('pt-BR')}
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Data de Criação
              </label>
              <p className="text-gray-900">
                {new Date(business.createdAt).toLocaleDateString('pt-BR')}
              </p>
            </div>
          </div>
        </Card>
      )}

      {/* Formulário */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <Card>
          <h3 className="text-lg font-semibold mb-4">
            {isEditMode ? 'Observações' : 'Dados do Negócio'}
          </h3>

          <div className="space-y-4">
            {/* Campos apenas em modo criação */}
            {!isEditMode && (
              <>
                {/* Parceiros */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Parceiros Envolvidos <span className="text-red-500">*</span>
                  </label>
                  <div className="border border-gray-300 rounded-lg p-3 max-h-60 overflow-y-auto">
                    {activePartners.length > 0 ? (
                      <div className="space-y-2">
                        {activePartners.map((partner) => (
                          <label key={partner.id} className="flex items-center space-x-3 cursor-pointer hover:bg-gray-50 p-2 rounded">
                            <input
                              type="checkbox"
                              checked={selectedPartnerIds.includes(partner.id)}
                              onChange={() => handlePartnerToggle(partner.id)}
                              className="h-4 w-4 text-black focus:ring-black border-gray-300 rounded"
                            />
                            <div className="flex-1">
                              <div className="text-sm font-medium text-gray-900">{partner.name}</div>
                              <div className="text-xs text-gray-500">Nível {partner.level}</div>
                            </div>
                          </label>
                        ))}
                      </div>
                    ) : (
                      <p className="text-sm text-gray-500">Nenhum parceiro ativo disponível</p>
                    )}
                  </div>
                  {!isEditMode && 'partnerIds' in errors && errors.partnerIds && (
                    <p className="text-red-500 text-sm mt-1">{String(errors.partnerIds.message)}</p>
                  )}
                  <div className="mt-2">
                    <p className="text-sm text-gray-600">
                      {selectedPartnerIds.length === 0 
                        ? 'Selecione os parceiros responsáveis pelo negócio'
                        : `${selectedPartnerIds.length} parceiro(s) selecionado(s)`
                      }
                    </p>
                    {selectedPartnerIds.length > 1 && (
                      <p className="text-xs text-blue-600 mt-1">
                        O valor será dividido igualmente entre os parceiros selecionados
                      </p>
                    )}
                  </div>
                </div>

                {/* Tipo de Negócio */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Tipo de Negócio <span className="text-red-500">*</span>
                  </label>
                  <select
                    {...register('businessTypeId' as keyof BusinessFormData)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black"
                  >
                    <option value="">Selecione um tipo</option>
                    {activeTypes.map((type) => (
                      <option key={type.id} value={type.id}>
                        {type.name}
                      </option>
                    ))}
                  </select>
                  {!isEditMode && 'businessTypeId' in errors && errors.businessTypeId && (
                    <p className="text-red-500 text-sm mt-1">
                      {String(errors.businessTypeId.message)}
                    </p>
                  )}
                  <p className="text-sm text-gray-600 mt-1">
                    Categoria ou tipo do negócio realizado
                  </p>
                </div>

                {/* Valor e Data na mesma linha */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {/* Valor */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Valor (R$) <span className="text-red-500">*</span>
                    </label>
                    <Input
                      type="number"
                      step="0.01"
                      min="0.01"
                      placeholder="0.00"
                      {...register('value' as keyof BusinessFormData, { valueAsNumber: true })}
                      error={!isEditMode && 'value' in errors ? String(errors.value?.message) : undefined}
                    />
                    <p className="text-sm text-gray-600 mt-1">Valor total do negócio em reais</p>
                  </div>

                  {/* Data */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Data <span className="text-red-500">*</span>
                    </label>
                    <Input
                      type="date"
                      {...register('date' as keyof BusinessFormData)}
                      error={!isEditMode && 'date' in errors ? String(errors.date?.message) : undefined}
                    />
                    <p className="text-sm text-gray-600 mt-1">Data da realização do negócio</p>
                  </div>
                </div>
              </>
            )}

            {/* Observações (disponível em ambos os modos) */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Observações {!isEditMode && '(opcional)'}
              </label>
              <textarea
                {...register('observations')}
                rows={4}
                placeholder="Adicione observações sobre o negócio..."
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black resize-none"
              />
              {errors.observations && (
                <p className="text-red-500 text-sm mt-1">
                  {String(errors.observations.message)}
                </p>
              )}
              <p className="text-sm text-gray-600 mt-1">
                Informações adicionais sobre o negócio
              </p>
            </div>
          </div>
        </Card>

        {/* Preview da Comissão (apenas em modo criação) */}
        {!isEditMode && (
          <Card className="bg-blue-50 border-blue-200">
            <div className="flex items-start gap-3">
              <Calculator className="w-5 h-5 text-blue-600 mt-0.5" />
              <div className="flex-1">
                <h3 className="text-lg font-semibold text-blue-900 mb-2">
                  Preview de Comissão
                </h3>
                <div className="space-y-2">
                  <div className="flex justify-between items-center">
                    <span className="text-blue-700">Valor Total do Negócio:</span>
                    <span className="text-blue-900 font-semibold text-lg">
                      {formatCurrency(typeof watchValue === 'number' ? watchValue : parseFloat(String(watchValue)) || 0)}
                    </span>
                  </div>
                  
                  {selectedPartnerIds.length > 1 && (
                    <>
                      <div className="flex justify-between items-center">
                        <span className="text-blue-700">Parceiros Selecionados:</span>
                        <span className="text-blue-900 font-medium">
                          {selectedPartnerIds.length}
                        </span>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-blue-700">Valor por Parceiro:</span>
                        <span className="text-blue-900 font-semibold">
                          {formatCurrency(valuePerPartner)}
                        </span>
                      </div>
                    </>
                  )}
                  
                  <div className="flex justify-between items-center border-t border-blue-300 pt-2">
                    <span className="text-blue-700 font-medium">Comissão Total (10%):</span>
                    <span className="text-blue-900 font-bold text-xl">
                      {formatCurrency(commissionPreview)}
                    </span>
                  </div>
                  
                  {selectedPartnerIds.length > 1 && (
                    <div className="flex justify-between items-center">
                      <span className="text-blue-700">Comissão por Parceiro:</span>
                      <span className="text-blue-900 font-bold">
                        {formatCurrency(commissionPreview / selectedPartnerIds.length)}
                      </span>
                    </div>
                  )}
                </div>
                <p className="text-sm text-blue-700 mt-3">
                  {selectedPartnerIds.length > 1 
                    ? `Será criado um negócio para cada parceiro selecionado. A comissão será calculada individualmente para cada negócio de ${formatCurrency(valuePerPartner)}.`
                    : 'A comissão será distribuída automaticamente entre os parceiros da rede (até 3 níveis) após a criação do negócio.'
                  }
                </p>
              </div>
            </div>
          </Card>
        )}

        {/* Botões de ação */}
        <div className="flex gap-4">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate('/negocios')}
            disabled={isSubmitting}
          >
            Cancelar
          </Button>
          <Button
            type="submit"
            disabled={isSubmitting || (isEditMode && business?.status === 'Cancelled') || (!isEditMode && selectedPartnerIds.length === 0)}
          >
            <Save className="w-4 h-4 mr-2" />
            {isSubmitting 
              ? 'Salvando...' 
              : isEditMode 
                ? 'Salvar Alterações' 
                : selectedPartnerIds.length > 1 
                  ? `Criar ${selectedPartnerIds.length} Negócios` 
                  : 'Criar Negócio'
            }
          </Button>
        </div>

        {isEditMode && business?.status === 'Cancelled' && (
          <Alert type="error">
            Negócios cancelados não podem ser editados.
          </Alert>
        )}
      </form>
    </div>
  );
}
