import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Search, Plus, Edit2, Power } from 'lucide-react';

import { businessTypesApi } from '@/api/endpoints/businessTypes.api';
import { BusinessType } from '@/types/business.types';
import { useToast } from '@/components/common/Toast';
import { useAuthStore } from '@/store/auth.store';
import { Permission } from '@/types/auth.types';

import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Card } from '@/components/common/Card';
import { Badge } from '@/components/common/Badge';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { Pagination } from '@/components/common/Pagination';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { Table, Column } from '@/components/common/Table';

export function BusinessTypesListPage() {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const queryClient = useQueryClient();
  const { user } = useAuthStore();

  // Estado dos filtros
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'active' | 'inactive'>('all');

  // Estado do diálogo de confirmação
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    businessTypeId: string | null;
    businessTypeName: string;
    currentStatus: boolean;
  }>({
    isOpen: false,
    businessTypeId: null,
    businessTypeName: '',
    currentStatus: false,
  });

  // Query: Lista de tipos de negócio
  const {
    data,
    isLoading,
    error,
  } = useQuery({
    queryKey: ['business-types', page, search, statusFilter],
    queryFn: () => {
      const params: any = {
        page,
        pageSize: 20,
      };

      if (search) {
        params.search = search;
      }

      if (statusFilter !== 'all') {
        params.isActive = statusFilter === 'active';
      }

      return businessTypesApi.list(params);
    },
  });

  // Mutation: Toggle Active
  const toggleActiveMutation = useMutation({
    mutationFn: (id: string) => businessTypesApi.toggleActive(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['business-types'] });
      const action = confirmDialog.currentStatus ? 'inativado' : 'ativado';
      showToast('success', `Tipo de negócio ${action} com sucesso!`);
      setConfirmDialog({ isOpen: false, businessTypeId: null, businessTypeName: '', currentStatus: false });
    },
    onError: () => {
      showToast('error', 'Erro ao alterar status do tipo de negócio');
    },
  });

  const businessTypes = data?.items || [];
  const totalPages = data ? Math.ceil(data.totalItems / 20) : 0;

  // Verificar se o usuário tem permissão para criar/editar
  const isAdminGlobal = user?.permission === Permission.AdminGlobal;
  const isAdminVetor = user?.permission === Permission.AdminVetor;
  const canManage = isAdminGlobal || isAdminVetor;

  // Handlers
  const handleSearchChange = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  const handleStatusFilterChange = (value: string) => {
    setStatusFilter(value as 'all' | 'active' | 'inactive');
    setPage(1);
  };

  const handleToggleActive = (businessType: BusinessType) => {
    setConfirmDialog({
      isOpen: true,
      businessTypeId: businessType.id,
      businessTypeName: businessType.name,
      currentStatus: businessType.isActive,
    });
  };

  const handleConfirmToggle = () => {
    if (confirmDialog.businessTypeId) {
      toggleActiveMutation.mutate(confirmDialog.businessTypeId);
    }
  };

  // Colunas da tabela
  const columns: Column<BusinessType>[] = [
    {
      key: 'name',
      header: 'Nome',
      render: (businessType) => (
        <div>
          <p className="font-medium text-gray-900">{businessType.name}</p>
          {businessType.description && (
            <p className="text-sm text-gray-600 mt-0.5">{businessType.description}</p>
          )}
        </div>
      ),
    },
    {
      key: 'isActive',
      header: 'Status',
      render: (businessType) => (
        <Badge variant={businessType.isActive ? 'success' : 'error'}>
          {businessType.isActive ? 'Ativo' : 'Inativo'}
        </Badge>
      ),
    },
    {
      key: 'createdAt',
      header: 'Data Cadastro',
      render: (businessType) => new Date(businessType.createdAt).toLocaleDateString('pt-BR'),
    },
  ];

  // Adicionar coluna de ações se tiver permissão
  if (canManage) {
    columns.push({
      key: 'actions',
      header: 'Ações',
      render: (businessType) => (
        <div className="flex items-center gap-2">
          <button
            onClick={() => navigate(`/tipos-negocio/${businessType.id}/editar`)}
            className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded transition-colors"
            title="Editar"
          >
            <Edit2 className="w-4 h-4" />
          </button>
          <button
            onClick={() => handleToggleActive(businessType)}
            className={`p-2 rounded transition-colors ${
              businessType.isActive
                ? 'text-red-600 hover:text-red-700 hover:bg-red-50'
                : 'text-green-600 hover:text-green-700 hover:bg-green-50'
            }`}
            title={businessType.isActive ? 'Inativar' : 'Ativar'}
          >
            <Power className="w-4 h-4" />
          </button>
        </div>
      ),
    });
  }

  if (isLoading) {
    return <Loading />;
  }

  if (error) {
    return (
      <div className="p-6">
        <Alert type="error">
          <p>Erro ao carregar tipos de negócio. Tente novamente.</p>
        </Alert>
      </div>
    );
  }

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Tipos de Negócio</h1>
          <p className="text-gray-600 mt-1">
            Gerencie os tipos de negócio disponíveis no sistema
          </p>
        </div>
        {canManage && (
          <Button
            onClick={() => navigate('/tipos-negocio/novo')}
            className="flex items-center gap-2"
          >
            <Plus className="w-4 h-4" />
            Novo Tipo
          </Button>
        )}
      </div>

      {/* Filtros */}
      <Card>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
            <Input
              type="text"
              placeholder="Buscar por nome..."
              value={search}
              onChange={(e) => handleSearchChange(e.target.value)}
              className="pl-10"
            />
          </div>

          <select
            value={statusFilter}
            onChange={(e) => handleStatusFilterChange(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black focus:border-transparent"
          >
            <option value="all">Todos os status</option>
            <option value="active">Apenas ativos</option>
            <option value="inactive">Apenas inativos</option>
          </select>
        </div>
      </Card>

      {/* Mensagem informativa para Operador */}
      {!canManage && (
        <Alert type="info">
          <p>
            Você possui permissão apenas para visualizar os tipos de negócio. Para criar ou editar, 
            entre em contato com um administrador.
          </p>
        </Alert>
      )}

      {/* Tabela */}
      <Card>
        {businessTypes.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-gray-500 mb-4">Nenhum tipo de negócio encontrado.</p>
            {canManage && (
              <Button onClick={() => navigate('/tipos-negocio/novo')}>
                Criar Primeiro Tipo de Negócio
              </Button>
            )}
          </div>
        ) : (
          <>
            <Table data={businessTypes} columns={columns} />
            {totalPages > 1 && (
              <div className="mt-4">
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

      {/* Diálogo de Confirmação */}
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={() => setConfirmDialog({ isOpen: false, businessTypeId: null, businessTypeName: '', currentStatus: false })}
        onConfirm={handleConfirmToggle}
        title={confirmDialog.currentStatus ? 'Inativar Tipo de Negócio' : 'Ativar Tipo de Negócio'}
        message={
          confirmDialog.currentStatus
            ? `Tem certeza que deseja inativar o tipo de negócio "${confirmDialog.businessTypeName}"? Tipos inativos não poderão ser utilizados em novos negócios.`
            : `Tem certeza que deseja ativar o tipo de negócio "${confirmDialog.businessTypeName}"?`
        }
        confirmText={confirmDialog.currentStatus ? 'Inativar' : 'Ativar'}
        variant={confirmDialog.currentStatus ? 'warning' : 'info'}
        isLoading={toggleActiveMutation.isPending}
      />
    </div>
  );
}
