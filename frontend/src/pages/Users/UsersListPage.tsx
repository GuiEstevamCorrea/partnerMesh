import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { Plus, Edit, Power, Search } from 'lucide-react';
import {
  Button,
  Input,
  Table,
  Badge,
  Pagination,
  Loading,
  Alert,
  ConfirmDialog,
} from '@/components';
import { usersApi, vectorsApi } from '@/api/endpoints';
import { User, Permission } from '@/types';
import { useAuthStore } from '@/store/auth.store';
import { useToast } from '@/components/common/Toast';

const PERMISSION_LABELS: Record<Permission, string> = {
  [Permission.AdminGlobal]: 'Admin Global',
  [Permission.AdminVetor]: 'Admin Vetor',
  [Permission.Operador]: 'Operador',
  [Permission.Parceiro]: 'Parceiro',
};

export const UsersListPage = () => {
  const queryClient = useQueryClient();
  const currentUser = useAuthStore((state) => state.user);
  const { showToast } = useToast();

  // Estados de filtros
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [permissionFilter, setPermissionFilter] = useState<Permission | ''>('');
  const [vectorFilter, setVectorFilter] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<string>('');

  // Estado do dialog de confirmação
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    userId: string;
    userName: string;
    action: 'activate' | 'deactivate';
  }>({
    isOpen: false,
    userId: '',
    userName: '',
    action: 'activate',
  });

  // Query para buscar usuários
  const { data, isLoading, error } = useQuery({
    queryKey: ['users', page, search, permissionFilter, vectorFilter, statusFilter],
    queryFn: () =>
      usersApi.list({
        page,
        pageSize: 20,
        search,
        permission: permissionFilter || undefined,
        vectorId: vectorFilter || undefined,
        isActive: statusFilter ? statusFilter === 'active' : undefined,
      }),
  });

  // Query para buscar vetores (para o filtro)
  const { data: vectorsData } = useQuery({
    queryKey: ['vectors'],
    queryFn: () => vectorsApi.list({ pageSize: 100 }),
    enabled: currentUser?.permission === Permission.AdminGlobal,
  });

  // Mutation para ativar/inativar usuário
  const toggleActiveMutation = useMutation({
    mutationFn: ({ userId, action }: { userId: string; action: 'activate' | 'deactivate' }) => 
      action === 'activate' ? usersApi.activate(userId) : usersApi.deactivate(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      showToast(
        'success',
        `Usuário ${confirmDialog.action === 'activate' ? 'ativado' : 'inativado'} com sucesso!`
      );
      setConfirmDialog({ isOpen: false, userId: '', userName: '', action: 'activate' });
    },
    onError: () => {
      showToast('error', 'Erro ao alterar status do usuário. Tente novamente.');
    },
  });

  const handleToggleActive = (user: User) => {
    setConfirmDialog({
      isOpen: true,
      userId: user.id,
      userName: user.name,
      action: user.isActive ? 'deactivate' : 'activate',
    });
  };

  const handleConfirmToggle = () => {
    toggleActiveMutation.mutate({ 
      userId: confirmDialog.userId, 
      action: confirmDialog.action 
    });
  };

  const handleResetFilters = () => {
    setSearch('');
    setPermissionFilter('');
    setVectorFilter('');
    setStatusFilter('');
    setPage(1);
  };

  if (isLoading) {
    return (
      <div className="p-6">
        <Loading />
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-6">
        <Alert type="error">
          Erro ao carregar lista de usuários. Tente novamente mais tarde.
        </Alert>
      </div>
    );
  }

  const { items: users = [], totalPages = 0, totalItems = 0 } = data || {};

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Usuários</h1>
          <p className="text-gray-600 mt-1">
            Gerencie os usuários do sistema ({totalItems} {totalItems === 1 ? 'usuário' : 'usuários'})
          </p>
        </div>
        <Link to="/usuarios/novo">
          <Button>
            <Plus className="w-4 h-4 mr-2" />
            Novo Usuário
          </Button>
        </Link>
      </div>

      {/* Filtros */}
      <div className="bg-white p-4 rounded-lg border border-gray-200 space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {/* Busca por nome/email */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Buscar
            </label>
            <Input
              placeholder="Nome ou email..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1);
              }}
              icon={<Search className="w-4 h-4" />}
            />
          </div>

          {/* Filtro de perfil */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Perfil
            </label>
            <select
              className="w-full px-3 py-2 border-2 border-gray-300 rounded-md text-gray-900 focus:outline-none focus:ring-2 focus:ring-black focus:border-black"
              value={permissionFilter}
              onChange={(e) => {
                setPermissionFilter(e.target.value as Permission | '');
                setPage(1);
              }}
            >
              <option value="">Todos os perfis</option>
              <option value={Permission.AdminGlobal}>Admin Global</option>
              <option value={Permission.AdminVetor}>Admin Vetor</option>
              <option value={Permission.Operador}>Operador</option>
            </select>
          </div>

          {/* Filtro de vetor (apenas AdminGlobal) */}
          {currentUser?.permission === Permission.AdminGlobal && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Vetor
              </label>
              <select
                className="w-full px-3 py-2 border-2 border-gray-300 rounded-md text-gray-900 focus:outline-none focus:ring-2 focus:ring-black focus:border-black"
                value={vectorFilter}
                onChange={(e) => {
                  setVectorFilter(e.target.value);
                  setPage(1);
                }}
              >
                <option value="">Todos os vetores</option>
                {vectorsData?.items?.map((vector) => (
                  <option key={vector.id} value={vector.id}>
                    {vector.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Filtro de status */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Status
            </label>
            <select
              className="w-full px-3 py-2 border-2 border-gray-300 rounded-md text-gray-900 focus:outline-none focus:ring-2 focus:ring-black focus:border-black"
              value={statusFilter}
              onChange={(e) => {
                setStatusFilter(e.target.value);
                setPage(1);
              }}
            >
              <option value="">Todos</option>
              <option value="active">Ativos</option>
              <option value="inactive">Inativos</option>
            </select>
          </div>
        </div>

        {/* Botão limpar filtros */}
        {(search || permissionFilter || vectorFilter || statusFilter) && (
          <div className="flex justify-end">
            <Button variant="outline" onClick={handleResetFilters}>
              Limpar Filtros
            </Button>
          </div>
        )}
      </div>

      {/* Tabela */}
      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
        {users.length === 0 ? (
          <div className="p-12 text-center">
            <p className="text-gray-500 text-lg">
              {search || permissionFilter || vectorFilter || statusFilter
                ? 'Nenhum usuário encontrado com os filtros aplicados.'
                : 'Nenhum usuário cadastrado ainda.'}
            </p>
            <Link to="/usuarios/novo">
              <Button className="mt-4">
                <Plus className="w-4 h-4 mr-2" />
                Cadastrar Primeiro Usuário
              </Button>
            </Link>
          </div>
        ) : (
          <>
            <Table
              columns={[
                {
                  key: 'name',
                  header: 'Nome',
                  render: (user: User) => (
                    <div>
                      <p className="font-medium text-gray-900">{user.name}</p>
                      <p className="text-sm text-gray-500">{user.email}</p>
                    </div>
                  ),
                },
                {
                  key: 'permission',
                  header: 'Perfil',
                  render: (user: User) => (
                    <Badge variant="info">
                      {PERMISSION_LABELS[user.permission]}
                    </Badge>
                  ),
                },
                {
                  key: 'vectorName',
                  header: 'Vetor',
                  render: (user: User) => (
                    <span className="text-gray-700">
                      {user.vectorName || '-'}
                    </span>
                  ),
                },
                {
                  key: 'isActive',
                  header: 'Status',
                  render: (user: User) => (
                    <Badge variant={user.isActive ? 'success' : 'error'}>
                      {user.isActive ? 'Ativo' : 'Inativo'}
                    </Badge>
                  ),
                },
                {
                  key: 'actions',
                  header: 'Ações',
                  render: (user: User) => (
                    <div className="flex items-center gap-2">
                      <Link to={`/usuarios/${user.id}/editar`}>
                        <Button variant="outline" size="sm">
                          <Edit className="w-4 h-4" />
                        </Button>
                      </Link>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleToggleActive(user)}
                        disabled={user.id === currentUser?.id}
                        title={
                          user.id === currentUser?.id
                            ? 'Você não pode alterar seu próprio status'
                            : user.isActive
                            ? 'Inativar usuário'
                            : 'Ativar usuário'
                        }
                      >
                        <Power
                          className={`w-4 h-4 ${
                            user.isActive ? 'text-red-600' : 'text-green-600'
                          }`}
                        />
                      </Button>
                    </div>
                  ),
                },
              ]}
              data={users}
              keyExtractor={(user) => user.id}
            />

            {/* Paginação */}
            {totalPages > 1 && (
              <div className="p-4 border-t border-gray-200">
                <Pagination
                  currentPage={page}
                  totalPages={totalPages}
                  onPageChange={setPage}
                />
              </div>
            )}
          </>
        )}
      </div>

      {/* Dialog de confirmação */}
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        title={`${confirmDialog.action === 'activate' ? 'Ativar' : 'Inativar'} Usuário`}
        message={`Tem certeza que deseja ${
          confirmDialog.action === 'activate' ? 'ativar' : 'inativar'
        } o usuário "${confirmDialog.userName}"?`}
        confirmText={confirmDialog.action === 'activate' ? 'Ativar' : 'Inativar'}
        cancelText="Cancelar"
        onConfirm={handleConfirmToggle}
        onClose={() =>
          setConfirmDialog({ isOpen: false, userId: '', userName: '', action: 'activate' })
        }
        isLoading={toggleActiveMutation.isPending}
        variant={confirmDialog.action === 'activate' ? 'info' : 'warning'}
      />
    </div>
  );
};
