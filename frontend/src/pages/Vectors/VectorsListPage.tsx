import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Edit, Power, Search, GitBranch } from 'lucide-react';

import { vectorsApi } from '@/api/endpoints/vectors.api';
import { useAuthStore } from '@/store/auth.store';
import { useToast } from '@/components/common/Toast';
import { useI18n } from '@/hooks/useI18n';
import { Permission } from '@/types/auth.types';

import { Table } from '@/components/common/Table';
import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Badge } from '@/components/common/Badge';
import { Pagination } from '@/components/common/Pagination';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { EmptyState } from '@/components/common/EmptyState';

export function VectorsListPage() {
  const { t } = useI18n();
  const { showToast } = useToast();
  const queryClient = useQueryClient();
  const { user: currentUser } = useAuthStore();

  // Estado dos filtros
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<'active' | 'inactive' | ''>('');

  // Estado do dialog de confirmação
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    vectorId: string;
    vectorName: string;
    action: 'activate' | 'deactivate';
  }>({
    isOpen: false,
    vectorId: '',
    vectorName: '',
    action: 'activate',
  });

  // Verificar se é AdminGlobal
  const isAdminGlobal = currentUser?.permission === Permission.AdminGlobal;

  // Query para listar vetores
  const { data: vectorsResponse, isLoading, error } = useQuery({
    queryKey: ['vectors', page, search, statusFilter],
    queryFn: () =>
      vectorsApi.list({
        page,
        pageSize: 20,
        search: search || undefined,
        isActive: statusFilter === 'active' ? true : statusFilter === 'inactive' ? false : undefined,
      }),
  });

  const vectors = vectorsResponse?.items || [];
  const totalPages = vectorsResponse?.totalPages || 1;

  // Mutation para ativar/inativar vetor
  const toggleActiveMutation = useMutation({
    mutationFn: vectorsApi.toggleActive,
    onSuccess: () => {
      showToast(
        'success',
        t(confirmDialog.action === 'activate' ? 'vectors.vectorActivated' : 'vectors.vectorDeactivated')
      );
      setConfirmDialog({ isOpen: false, vectorId: '', vectorName: '', action: 'activate' });
      queryClient.invalidateQueries({ queryKey: ['vectors'] });
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || t('vectors.vectorError');
      showToast('error', message);
    },
  });

  // Handler para abrir dialog de confirmação
  const handleToggleActive = (vector: any) => {
    setConfirmDialog({
      isOpen: true,
      vectorId: vector.id,
      vectorName: vector.name,
      action: vector.isActive ? 'deactivate' : 'activate',
    });
  };

  // Handler para confirmar toggle
  const handleConfirmToggle = () => {
    toggleActiveMutation.mutate(confirmDialog.vectorId);
  };

  // Colunas da tabela
  const columns = [
    {
      key: 'name',
      header: t('vectors.table.nameEmail'),
      render: (vector: any) => (
        <div>
          <div className="font-medium text-gray-900">{vector.name}</div>
          <div className="text-sm text-gray-500">{vector.email}</div>
        </div>
      ),
    },
    {
      key: 'partnersCount',
      header: t('vectors.table.partnersCount'),
      render: (vector: any) => (
        <span className="text-gray-900 font-medium">{vector.partnersCount || 0}</span>
      ),
    },
    {
      key: 'isActive',
      header: t('vectors.table.status'),
      render: (vector: any) => (
        <Badge variant={vector.isActive ? 'success' : 'default'}>
          {vector.isActive ? t('common.active') : t('common.inactive')}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: t('vectors.table.actions'),
      render: (vector: any) => (
        <div className="flex items-center gap-2">
          {/* Botão Ver Árvore */}
          <Link to={`/parceiros/arvore?vectorId=${vector.id}`}>
            <Button variant="ghost" size="sm" title={t('vectors.actions.viewTree')}>
              <GitBranch className="w-4 h-4" />
            </Button>
          </Link>

          {/* Botão Editar (apenas AdminGlobal) */}
          {isAdminGlobal && (
            <Link to={`/vetores/${vector.id}/editar`}>
              <Button variant="ghost" size="sm" title={t('vectors.actions.edit')}>
                <Edit className="w-4 h-4" />
              </Button>
            </Link>
          )}

          {/* Botão Ativar/Inativar (apenas AdminGlobal) */}
          {isAdminGlobal && (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleToggleActive(vector)}
              title={vector.isActive ? t('vectors.actions.deactivate') : t('vectors.actions.activate')}
              className={vector.isActive ? 'text-red-600 hover:text-red-700' : 'text-green-600 hover:text-green-700'}
            >
              <Power className="w-4 h-4" />
            </Button>
          )}
        </div>
      ),
    },
  ];

  // Loading state
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Loading />
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <Alert type="error">
        {t('vectors.errorLoading')}
      </Alert>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{t('vectors.title')}</h1>
          <p className="text-sm text-gray-500 mt-1">
            {t('vectors.subtitle')}
          </p>
        </div>
        {isAdminGlobal && (
          <Link to="/vectors/new">
            <Button>
              <Plus className="w-4 h-4 mr-2" />
              {t('vectors.newVector')}
            </Button>
          </Link>
        )}
      </div>

      {/* Filtros */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {/* Busca por Nome */}
        <div className="md:col-span-2">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
            <Input
              placeholder={t('vectors.searchPlaceholder')}
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1); // Reset para primeira página ao buscar
              }}
              className="pl-10"
            />
          </div>
        </div>

        {/* Filtro de Status */}
        <div>
          <select
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as 'active' | 'inactive' | '');
              setPage(1);
            }}
            className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-black focus:border-transparent"
          >
            <option value="">{t('common.allStatuses')}</option>
            <option value="active">{t('common.active')}</option>
            <option value="inactive">{t('common.inactive')}</option>
          </select>
        </div>
      </div>

      {/* Mensagem de permissão para AdminVetor */}
      {!isAdminGlobal && (
        <Alert type="info">
          {t('vectors.viewOnlyYourVector')}
        </Alert>
      )}

      {/* Tabela */}
      {vectors.length === 0 ? (
        <EmptyState
          title={t('vectors.noVectorFound')}
          description={
            search || statusFilter
              ? t('common.adjustFilters')
              : t('common.createFirst')
          }
          action={
            isAdminGlobal && !search && !statusFilter ? (
              <Link to="/vetores/novo">
                <Button>
                  <Plus className="w-4 h-4 mr-2" />
                  {t('vectors.createFirstVector')}
                </Button>
              </Link>
            ) : undefined
          }
        />
      ) : (
        <div className="bg-white rounded-lg shadow">
          <Table
            columns={columns}
            data={vectors}
            keyExtractor={(vector) => vector.id}
          />
        </div>
      )}

      {/* Paginação */}
      {totalPages > 1 && (
        <div className="flex justify-center">
          <Pagination
            currentPage={page}
            totalPages={totalPages}
            onPageChange={setPage}
          />
        </div>
      )}

      {/* Dialog de Confirmação */}
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        title={confirmDialog.action === 'activate' ? t('vectors.dialog.activateTitle') : t('vectors.dialog.deactivateTitle')}
        message={
          confirmDialog.action === 'activate'
            ? t('vectors.dialog.activateMessage', { name: confirmDialog.vectorName })
            : t('vectors.dialog.deactivateMessage', { name: confirmDialog.vectorName })
        }
        confirmText={confirmDialog.action === 'activate' ? t('vectors.dialog.confirmActivate') : t('vectors.dialog.confirmDeactivate')}
        cancelText={t('vectors.dialog.cancel')}
        isLoading={toggleActiveMutation.isPending}
        onClose={() =>
          setConfirmDialog({ isOpen: false, vectorId: '', vectorName: '', action: 'activate' })
        }
        onConfirm={handleConfirmToggle}
        variant={confirmDialog.action === 'activate' ? 'info' : 'warning'}
      />
    </div>
  );
}
