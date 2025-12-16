import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Plus, Edit, Power, Search, GitBranch } from 'lucide-react';

import { vectorsApi } from '@/api/endpoints/vectors.api';
import { useAuthStore } from '@/store/auth.store';
import { useToast } from '@/components/common/Toast';
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
        `Vetor ${confirmDialog.action === 'activate' ? 'ativado' : 'inativado'} com sucesso!`
      );
      setConfirmDialog({ isOpen: false, vectorId: '', vectorName: '', action: 'activate' });
      queryClient.invalidateQueries({ queryKey: ['vectors'] });
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao atualizar vetor';
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
      header: 'Nome/Email',
      render: (vector: any) => (
        <div>
          <div className="font-medium text-gray-900">{vector.name}</div>
          <div className="text-sm text-gray-500">{vector.email}</div>
        </div>
      ),
    },
    {
      key: 'partnersCount',
      header: 'Qtd Parceiros',
      render: (vector: any) => (
        <span className="text-gray-900 font-medium">{vector.partnersCount || 0}</span>
      ),
    },
    {
      key: 'isActive',
      header: 'Status',
      render: (vector: any) => (
        <Badge variant={vector.isActive ? 'success' : 'default'}>
          {vector.isActive ? 'Ativo' : 'Inativo'}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: 'Ações',
      render: (vector: any) => (
        <div className="flex items-center gap-2">
          {/* Botão Ver Árvore */}
          <Link to={`/parceiros/arvore?vectorId=${vector.id}`}>
            <Button variant="ghost" size="sm" title="Ver Árvore de Parceiros">
              <GitBranch className="w-4 h-4" />
            </Button>
          </Link>

          {/* Botão Editar (apenas AdminGlobal) */}
          {isAdminGlobal && (
            <Link to={`/vetores/${vector.id}/editar`}>
              <Button variant="ghost" size="sm" title="Editar">
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
              title={vector.isActive ? 'Inativar' : 'Ativar'}
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
        Erro ao carregar vetores. Tente novamente mais tarde.
      </Alert>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Vetores</h1>
          <p className="text-sm text-gray-500 mt-1">
            Gerencie os vetores do sistema
          </p>
        </div>
        {isAdminGlobal && (
          <Link to="/vetores/novo">
            <Button>
              <Plus className="w-4 h-4 mr-2" />
              Novo Vetor
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
              placeholder="Buscar por nome ou email..."
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
            <option value="">Todos os status</option>
            <option value="active">Ativos</option>
            <option value="inactive">Inativos</option>
          </select>
        </div>
      </div>

      {/* Mensagem de permissão para AdminVetor */}
      {!isAdminGlobal && (
        <Alert type="info">
          Você está visualizando apenas seu vetor. Apenas AdminGlobal pode criar e gerenciar múltiplos vetores.
        </Alert>
      )}

      {/* Tabela */}
      {vectors.length === 0 ? (
        <EmptyState
          title="Nenhum vetor encontrado"
          description={
            search || statusFilter
              ? 'Tente ajustar os filtros de busca'
              : 'Comece criando seu primeiro vetor'
          }
          action={
            isAdminGlobal && !search && !statusFilter ? (
              <Link to="/vetores/novo">
                <Button>
                  <Plus className="w-4 h-4 mr-2" />
                  Criar Primeiro Vetor
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
        title={`${confirmDialog.action === 'activate' ? 'Ativar' : 'Inativar'} Vetor`}
        message={`Tem certeza que deseja ${
          confirmDialog.action === 'activate' ? 'ativar' : 'inativar'
        } o vetor "${confirmDialog.vectorName}"?${
          confirmDialog.action === 'deactivate'
            ? ' Usuários e parceiros associados serão afetados.'
            : ''
        }`}
        confirmText={confirmDialog.action === 'activate' ? 'Ativar' : 'Inativar'}
        cancelText="Cancelar"
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
