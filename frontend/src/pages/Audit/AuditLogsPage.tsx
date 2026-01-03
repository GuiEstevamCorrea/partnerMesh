import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Search, Eye, FileText, Calendar } from 'lucide-react';
import {
  Button,
  Input,
  Table,
  Badge,
  Pagination,
  Loading,
  Alert,
  Card,
  Modal,
  Select,
} from '@/components';
import { auditApi, usersApi } from '@/api/endpoints';
import { AuditLog, Permission } from '@/types';
import { useAuthStore } from '@/store/auth.store';
import { formatDate } from '@/utils/formatters';

type ActionType = 'Login' | 'Logout' | 'Create' | 'Update' | 'Delete' | 'Payment' | 'Cancel' | '';
type EntityType = 'User' | 'Vector' | 'Partner' | 'Business' | 'Payment' | '';

const ACTION_COLORS: Record<Exclude<ActionType, ''>, string> = {
  Login: 'blue',
  Logout: 'gray',
  Create: 'green',
  Update: 'yellow',
  Delete: 'red',
  Payment: 'purple',
  Cancel: 'orange',
};

const ENTITY_LABELS: Record<Exclude<EntityType, ''>, string> = {
  User: 'Usuário',
  Vector: 'Vetor',
  Partner: 'Parceiro',
  Business: 'Negócio',
  Payment: 'Pagamento',
};

export const AuditLogsPage = () => {
  const currentUser = useAuthStore((state) => state.user);

  // Estados de filtros
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [userFilter, setUserFilter] = useState<string>('');
  const [actionFilter, setActionFilter] = useState<ActionType>('');
  const [entityFilter, setEntityFilter] = useState<EntityType>('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');

  // Estado do modal de detalhes
  const [selectedLog, setSelectedLog] = useState<AuditLog | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Verificar permissão
  const isAdminGlobal = currentUser?.permission === Permission.AdminGlobal;

  // Query para buscar logs de auditoria
  const { data, isLoading, error } = useQuery({
    queryKey: ['audit-logs', page, search, userFilter, actionFilter, entityFilter, startDate, endDate],
    queryFn: () =>
      auditApi.list({
        page,
        pageSize: 50,
        search,
        sortBy: 'createdAt',
        sortOrder: 'desc',
      }),
    enabled: isAdminGlobal,
  });

  // Query para buscar usuários (para o filtro)
  const { data: usersData } = useQuery({
    queryKey: ['users-select'],
    queryFn: () => usersApi.list({ pageSize: 1000 }),
    enabled: isAdminGlobal,
  });

  // Aplicar filtros no frontend
  const filteredLogs = data?.items?.filter((log) => {
    if (userFilter && log.userId !== userFilter) return false;
    if (actionFilter && !log.action.includes(actionFilter)) return false;
    if (entityFilter && log.entity !== entityFilter) return false;
    if (startDate && new Date(log.createdAt) < new Date(startDate)) return false;
    if (endDate && new Date(log.createdAt) > new Date(endDate)) return false;
    return true;
  }) || [];

  const handleViewDetails = (log: AuditLog) => {
    setSelectedLog(log);
    setIsModalOpen(true);
  };

  const handleResetFilters = () => {
    setSearch('');
    setUserFilter('');
    setActionFilter('');
    setEntityFilter('');
    setStartDate('');
    setEndDate('');
    setPage(1);
  };

  const getActionBadgeColor = (action: string): string => {
    for (const [key, value] of Object.entries(ACTION_COLORS)) {
      if (action.includes(key)) return value;
    }
    return 'gray';
  };

  const getActionLabel = (action: string): string => {
    for (const key of Object.keys(ACTION_COLORS)) {
      if (action.includes(key)) return key;
    }
    return action;
  };

  // Se não for AdminGlobal, exibir mensagem de permissão negada
  if (!isAdminGlobal) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-3xl font-bold text-gray-900">Logs de Auditoria</h1>
        </div>
        <Alert type="warning">
          Você não tem permissão para acessar os logs de auditoria. Esta funcionalidade está disponível apenas para Administradores Globais.
        </Alert>
      </div>
    );
  }

  if (isLoading) return <Loading />;
  if (error) return <Alert type="error">Erro ao carregar logs: {String(error)}</Alert>;

  const columns = [
    {
      key: 'createdAt',
      header: 'Data/Hora',
      render: (log: AuditLog) => (
        <div className="text-sm">
          <div className="font-medium text-gray-900">{formatDate(log.createdAt)}</div>
          <div className="text-gray-500">{new Date(log.createdAt).toLocaleTimeString('pt-BR')}</div>
        </div>
      ),
    },
    {
      key: 'userName',
      header: 'Usuário',
      render: (log: AuditLog) => (
        <div className="font-medium text-gray-900 text-sm">
          {log.userName}
        </div>
      ),
    },
    {
      key: 'action',
      header: 'Ação',
      render: (log: AuditLog) => (
        <Badge variant={getActionBadgeColor(log.action) as any}>
          {getActionLabel(log.action)}
        </Badge>
      ),
    },
    {
      key: 'entity',
      header: 'Entidade',
      render: (log: AuditLog) => (
        <div className="text-sm">
          <div className="font-medium text-gray-900">
            {ENTITY_LABELS[log.entity as keyof typeof ENTITY_LABELS] || log.entity}
          </div>
          {log.entityName && (
            <div className="text-xs text-gray-500">{log.entityName}</div>
          )}
        </div>
      ),
    },
    {
      key: 'actions',
      header: 'Detalhes',
      render: (log: AuditLog) => (
        <Button
          variant="ghost"
          size="sm"
          onClick={() => handleViewDetails(log)}
        >
          <Eye className="w-4 h-4 mr-1" />
          Ver
        </Button>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <FileText className="w-8 h-8 text-blue-600" />
          <h1 className="text-3xl font-bold text-gray-900">Logs de Auditoria</h1>
        </div>
      </div>

      {/* Filtros */}
      <Card>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div className="lg:col-span-3">
            <Input
              label="Busca Livre"
              placeholder="Buscar em detalhes..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1);
              }}
              icon={<Search className="w-4 h-4" />}
            />
          </div>

          <Select
            label="Usuário"
            value={userFilter}
            onChange={(e) => {
              setUserFilter(e.target.value);
              setPage(1);
            }}
            placeholder="Todos os usuários"
            options={[
              ...(usersData?.items?.map((user) => ({
                value: user.id,
                label: user.name,
              })) || []),
            ]}
          />

          <Select
            label="Ação"
            value={actionFilter}
            onChange={(e) => {
              setActionFilter(e.target.value as ActionType);
              setPage(1);
            }}
            placeholder="Todas as ações"
            options={[
              { value: 'Login', label: 'Login' },
              { value: 'Logout', label: 'Logout' },
              { value: 'Create', label: 'Create' },
              { value: 'Update', label: 'Update' },
              { value: 'Delete', label: 'Delete' },
              { value: 'Payment', label: 'Payment' },
              { value: 'Cancel', label: 'Cancel' },
            ]}
          />

          <Select
            label="Entidade"
            value={entityFilter}
            onChange={(e) => {
              setEntityFilter(e.target.value as EntityType);
              setPage(1);
            }}
            placeholder="Todas as entidades"
            options={[
              { value: 'User', label: 'Usuário' },
              { value: 'Vector', label: 'Vetor' },
              { value: 'Partner', label: 'Parceiro' },
              { value: 'Business', label: 'Negócio' },
              { value: 'Payment', label: 'Pagamento' },
            ]}
          />

          <Input
            label="Data Início"
            type="date"
            value={startDate}
            onChange={(e) => {
              setStartDate(e.target.value);
              setPage(1);
            }}
            icon={<Calendar className="w-4 h-4" />}
          />

          <Input
            label="Data Fim"
            type="date"
            value={endDate}
            onChange={(e) => {
              setEndDate(e.target.value);
              setPage(1);
            }}
            icon={<Calendar className="w-4 h-4" />}
          />

          <div className="flex items-end">
            <Button
              variant="outline"
              onClick={handleResetFilters}
              className="w-full"
            >
              Limpar Filtros
            </Button>
          </div>
        </div>
      </Card>

      {/* Tabela */}
      <Card>
        <div className="mb-4">
          <h2 className="text-lg font-semibold text-gray-900">
            Registros de Auditoria ({filteredLogs.length})
          </h2>
          <p className="text-sm text-gray-500">
            Exibindo {filteredLogs.length} de {data?.totalItems || 0} logs
          </p>
        </div>

        {filteredLogs.length === 0 ? (
          <Alert type="info">
            Nenhum log encontrado com os filtros aplicados.
          </Alert>
        ) : (
          <>
            <Table<AuditLog>
              data={filteredLogs}
              columns={columns}
              emptyMessage="Nenhum log encontrado"
            />

            {data && data.totalItems > 50 && (
              <div className="mt-4">
                <Pagination
                  currentPage={page}
                  totalPages={data.totalPages}
                  onPageChange={setPage}
                />
              </div>
            )}
          </>
        )}
      </Card>

      {/* Modal de Detalhes */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Detalhes do Log de Auditoria"
      >
        {selectedLog && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700">Data/Hora</label>
                <p className="text-sm text-gray-900">
                  {formatDate(selectedLog.createdAt)} às{' '}
                  {new Date(selectedLog.createdAt).toLocaleTimeString('pt-BR')}
                </p>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700">Usuário</label>
                <p className="text-sm text-gray-900">{selectedLog.userName}</p>
                <p className="text-xs text-gray-500">ID: {selectedLog.userId}</p>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700">Ação</label>
                <div className="mt-1">
                  <Badge variant={getActionBadgeColor(selectedLog.action) as any}>
                    {selectedLog.action}
                  </Badge>
                </div>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700">Entidade</label>
                <p className="text-sm text-gray-900">
                  {ENTITY_LABELS[selectedLog.entity as keyof typeof ENTITY_LABELS] || selectedLog.entity}
                </p>
                <p className="text-xs text-gray-500">ID: {selectedLog.entityId || '-'}</p>
              </div>

              {selectedLog.ipAddress && (
                <div>
                  <label className="text-sm font-medium text-gray-700">Endereço IP</label>
                  <p className="text-sm text-gray-900">{selectedLog.ipAddress}</p>
                </div>
              )}

              {selectedLog.userAgent && (
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700">User Agent</label>
                  <p className="text-xs text-gray-600 font-mono break-all">
                    {selectedLog.userAgent}
                  </p>
                </div>
              )}
            </div>

            {selectedLog.details && (
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-2">
                  Detalhes (Payload)
                </label>
                <pre className="text-xs bg-gray-100 p-4 rounded-lg overflow-auto max-h-96 border border-gray-200">
                  {JSON.stringify(JSON.parse(selectedLog.details), null, 2)}
                </pre>
              </div>
            )}

            <div className="flex justify-end pt-4 border-t">
              <Button onClick={() => setIsModalOpen(false)}>Fechar</Button>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};
