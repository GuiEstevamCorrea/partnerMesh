import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Search, Plus, Eye, FileText, Edit2, XCircle, DollarSign } from 'lucide-react';

import { businessApi } from '@/api/endpoints/business.api';
import { partnersApi } from '@/api/endpoints/partners.api';
import { businessTypesApi } from '@/api/endpoints/businessTypes.api';
import { Business } from '@/types/business.types';
import { useToast } from '@/components/common/Toast';
import { useAuthStore } from '@/store/auth.store';
import { Permission } from '@/types/auth.types';
import { formatCurrency, formatDate } from '@/utils/formatters';

import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Card } from '@/components/common/Card';
import { Badge } from '@/components/common/Badge';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { Pagination } from '@/components/common/Pagination';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { Table, Column } from '@/components/common/Table';

export function BusinessListPage() {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const queryClient = useQueryClient();
  const { user } = useAuthStore();

  // Estado dos filtros
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [partnerFilter, setPartnerFilter] = useState<string>('');
  const [typeFilter, setTypeFilter] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [dateStart, setDateStart] = useState<string>('');
  const [dateEnd, setDateEnd] = useState<string>('');
  const [minValue, setMinValue] = useState<string>('');
  const [maxValue, setMaxValue] = useState<string>('');

  // Estado do ConfirmDialog
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    businessId: string;
    businessDescription: string;
  }>({
    isOpen: false,
    businessId: '',
    businessDescription: '',
  });

  const isAdminGlobal = user?.permission === Permission.AdminGlobal;
  const canManage = isAdminGlobal || user?.permission === Permission.AdminVetor;

  // Query para listar negócios
  const { data, isLoading, error } = useQuery({
    queryKey: ['business', page, search, partnerFilter, typeFilter, statusFilter, dateStart, dateEnd, minValue, maxValue],
    queryFn: () => {
      const params: any = {
        page,
        pageSize: 20,
      };

      if (search) params.search = search;
      if (partnerFilter) params.partnerId = partnerFilter;
      if (typeFilter) params.businessTypeId = typeFilter;
      if (statusFilter !== 'all') {
        params.status = statusFilter === 'active' ? 'Active' : 'Cancelled';
      }
      if (dateStart) params.startDate = dateStart;
      if (dateEnd) params.endDate = dateEnd;
      if (minValue) params.minValue = parseFloat(minValue);
      if (maxValue) params.maxValue = parseFloat(maxValue);

      return businessApi.list(params);
    },
  });

  // Query para carregar parceiros (para o filtro)
  const { data: partnersData } = useQuery({
    queryKey: ['partners-for-filter'],
    queryFn: () => partnersApi.list({ pageSize: 1000 }),
  });

  // Query para carregar tipos de negócio (para o filtro)
  const { data: typesData } = useQuery({
    queryKey: ['business-types-for-filter'],
    queryFn: () => businessTypesApi.list({ pageSize: 1000 }),
  });

  // Mutation para cancelar negócio
  const cancelMutation = useMutation({
    mutationFn: (businessId: string) => businessApi.cancel(businessId),
    onSuccess: () => {
      showToast('success', 'Negócio cancelado com sucesso');
      queryClient.invalidateQueries({ queryKey: ['business'] });
      setConfirmDialog({ isOpen: false, businessId: '', businessDescription: '' });
    },
    onError: (error: any) => {
      showToast('error', error.response?.data?.message || 'Erro ao cancelar negócio');
    },
  });

  const handleCancelClick = (business: Business) => {
    setConfirmDialog({
      isOpen: true,
      businessId: business.id,
      businessDescription: `${business.partnerName} - ${business.businessTypeName} - ${formatCurrency(business.value)}`,
    });
  };

  const handleConfirmCancel = () => {
    cancelMutation.mutate(confirmDialog.businessId);
  };

  const handleSearchChange = (value: string) => {
    setSearch(value);
    setPage(1); // Reset para primeira página ao filtrar
  };

  const handleFilterChange = () => {
    setPage(1); // Reset para primeira página ao mudar filtros
  };

  const columns: Column<Business>[] = [
    {
      key: 'id',
      header: 'ID',
      render: (business) => (
        <span className="text-sm font-mono text-gray-600">
          #{business.id.slice(0, 8)}
        </span>
      ),
    },
    {
      key: 'partnerName',
      header: 'Parceiro',
      render: (business) => (
        <div className={business.status === 'Cancelled' ? 'text-gray-400' : ''}>
          <div className="font-medium">{business.partnerName}</div>
        </div>
      ),
    },
    {
      key: 'businessTypeName',
      header: 'Tipo',
      render: (business) => (
        <span className={business.status === 'Cancelled' ? 'text-gray-400' : ''}>
          {business.businessTypeName}
        </span>
      ),
    },
    {
      key: 'value',
      header: 'Valor',
      render: (business) => (
        <span className={`font-medium ${business.status === 'Cancelled' ? 'text-gray-400' : 'text-green-600'}`}>
          {formatCurrency(business.value)}
        </span>
      ),
    },
    {
      key: 'date',
      header: 'Data',
      render: (business) => (
        <span className={business.status === 'Cancelled' ? 'text-gray-400' : ''}>
          {formatDate(business.date)}
        </span>
      ),
    },
    {
      key: 'totalCommission',
      header: 'Comissão Total',
      render: (business) => (
        <span className={`font-medium ${business.status === 'Cancelled' ? 'text-gray-400' : 'text-blue-600'}`}>
          {formatCurrency(business.commission?.totalValue || 0)}
        </span>
      ),
    },
    {
      key: 'status',
      header: 'Status',
      render: (business) => (
        <Badge variant={business.status === 'Active' ? 'success' : 'error'}>
          {business.status === 'Active' ? 'Ativo' : 'Cancelado'}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: 'Ações',
      render: (business) => (
        <div className="flex gap-2">
          <button
            onClick={() => navigate(`/negocios/${business.id}`)}
            className="p-1 text-blue-600 hover:text-blue-800"
            title="Ver Detalhes"
          >
            <Eye className="w-4 h-4" />
          </button>
          <button
            onClick={() => navigate(`/negocios/${business.id}/pagamentos`)}
            className="p-1 text-green-600 hover:text-green-800"
            title="Ver Pagamentos"
          >
            <DollarSign className="w-4 h-4" />
          </button>
          {canManage && business.status === 'Active' && (
            <>
              <button
                onClick={() => navigate(`/negocios/${business.id}/editar`)}
                className="p-1 text-gray-600 hover:text-gray-800"
                title="Editar"
              >
                <Edit2 className="w-4 h-4" />
              </button>
              <button
                onClick={() => handleCancelClick(business)}
                className="p-1 text-red-600 hover:text-red-800"
                title="Cancelar"
              >
                <XCircle className="w-4 h-4" />
              </button>
            </>
          )}
        </div>
      ),
    },
  ];

  if (isLoading) {
    return <Loading />;
  }

  if (error) {
    return (
      <Alert type="error">
        Erro ao carregar negócios. Tente novamente mais tarde.
      </Alert>
    );
  }

  const businesses = data?.items || [];
  const totalPages = data?.totalPages || 1;
  const isEmpty = businesses.length === 0;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Negócios</h1>
          <p className="text-gray-600 mt-1">
            Gerencie todos os negócios e suas comissões
          </p>
        </div>
        {canManage && (
          <Button onClick={() => navigate('/negocios/novo')}>
            <Plus className="w-4 h-4 mr-2" />
            Novo Negócio
          </Button>
        )}
      </div>

      {/* Info para Operador */}
      {!canManage && (
        <Alert type="info">
          Você pode visualizar negócios, mas não pode criar ou editar.
        </Alert>
      )}

      {/* Filtros */}
      <Card>
        <div className="space-y-4">
          {/* Linha 1: Busca e Parceiro */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
              <Input
                type="text"
                placeholder="Buscar por parceiro ou tipo..."
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                className="pl-10"
              />
            </div>

            <select
              value={partnerFilter}
              onChange={(e) => {
                setPartnerFilter(e.target.value);
                handleFilterChange();
              }}
              className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black"
            >
              <option value="">Todos os parceiros</option>
              {partnersData?.items?.filter(p => p.isActive).map((partner) => (
                <option key={partner.id} value={partner.id}>
                  {partner.name}
                </option>
              ))}
            </select>
          </div>

          {/* Linha 2: Tipo e Status */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <select
              value={typeFilter}
              onChange={(e) => {
                setTypeFilter(e.target.value);
                handleFilterChange();
              }}
              className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black"
            >
              <option value="">Todos os tipos</option>
              {typesData?.items?.filter(t => t.isActive).map((type) => (
                <option key={type.id} value={type.id}>
                  {type.name}
                </option>
              ))}
            </select>

            <select
              value={statusFilter}
              onChange={(e) => {
                setStatusFilter(e.target.value);
                handleFilterChange();
              }}
              className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black"
            >
              <option value="all">Todos os status</option>
              <option value="active">Apenas ativos</option>
              <option value="cancelled">Apenas cancelados</option>
            </select>
          </div>

          {/* Linha 3: Data Início e Fim */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Data Início
              </label>
              <Input
                type="date"
                value={dateStart}
                onChange={(e) => {
                  setDateStart(e.target.value);
                  handleFilterChange();
                }}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Data Fim
              </label>
              <Input
                type="date"
                value={dateEnd}
                onChange={(e) => {
                  setDateEnd(e.target.value);
                  handleFilterChange();
                }}
              />
            </div>
          </div>

          {/* Linha 4: Valor Mínimo e Máximo */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Valor Mínimo (R$)
              </label>
              <Input
                type="number"
                placeholder="0.00"
                step="0.01"
                value={minValue}
                onChange={(e) => {
                  setMinValue(e.target.value);
                  handleFilterChange();
                }}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Valor Máximo (R$)
              </label>
              <Input
                type="number"
                placeholder="0.00"
                step="0.01"
                value={maxValue}
                onChange={(e) => {
                  setMaxValue(e.target.value);
                  handleFilterChange();
                }}
              />
            </div>
          </div>

          {/* Botão Limpar Filtros */}
          {(search || partnerFilter || typeFilter || statusFilter !== 'all' || dateStart || dateEnd || minValue || maxValue) && (
            <div className="flex justify-end">
              <Button
                variant="outline"
                onClick={() => {
                  setSearch('');
                  setPartnerFilter('');
                  setTypeFilter('');
                  setStatusFilter('all');
                  setDateStart('');
                  setDateEnd('');
                  setMinValue('');
                  setMaxValue('');
                  setPage(1);
                }}
              >
                Limpar Filtros
              </Button>
            </div>
          )}
        </div>
      </Card>

      {/* Tabela */}
      <Card>
        {isEmpty ? (
          <div className="text-center py-12">
            <FileText className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">
              Nenhum negócio encontrado
            </h3>
            <p className="text-gray-600 mb-4">
              {search || partnerFilter || typeFilter || statusFilter !== 'all' || dateStart || dateEnd || minValue || maxValue
                ? 'Tente ajustar os filtros para encontrar negócios.'
                : 'Comece criando seu primeiro negócio.'}
            </p>
            {canManage && !search && !partnerFilter && !typeFilter && statusFilter === 'all' && !dateStart && !dateEnd && !minValue && !maxValue && (
              <Button onClick={() => navigate('/negocios/novo')}>
                <Plus className="w-4 h-4 mr-2" />
                Criar Primeiro Negócio
              </Button>
            )}
          </div>
        ) : (
          <>
            <Table
              columns={columns}
              data={businesses}
            />

            {/* Paginação */}
            {totalPages > 1 && (
              <div className="mt-4 flex justify-center">
                <Pagination
                  currentPage={page}
                  totalPages={totalPages}
                  onPageChange={setPage}
                />
              </div>
            )}
          </>
        )}
      </Card>

      {/* ConfirmDialog para cancelar */}
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={() =>
          setConfirmDialog({ isOpen: false, businessId: '', businessDescription: '' })
        }
        onConfirm={handleConfirmCancel}
        title="Cancelar Negócio"
        message={`Tem certeza que deseja cancelar o negócio "${confirmDialog.businessDescription}"? Esta ação irá cancelar todos os pagamentos pendentes. Pagamentos já realizados não serão afetados.`}
        confirmText="Cancelar Negócio"
        variant="danger"
        isLoading={cancelMutation.isPending}
      />
    </div>
  );
}
