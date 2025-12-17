import { useQuery } from '@tanstack/react-query';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Clock, User as UserIcon, FileText } from 'lucide-react';
import { Button, Card, Badge, Loading, Alert } from '@/components';
import { auditApi } from '@/api/endpoints';
import { AuditLog, Permission } from '@/types';
import { useAuthStore } from '@/store/auth.store';
import { formatDate } from '@/utils/formatters';

type EntityType = 'User' | 'Vector' | 'Partner' | 'Business' | 'Payment';

const ACTION_COLORS: Record<string, string> = {
  Login: 'blue',
  Logout: 'gray',
  Create: 'green',
  Update: 'yellow',
  Delete: 'red',
  Payment: 'purple',
  Cancel: 'orange',
};

const ENTITY_LABELS: Record<EntityType, string> = {
  User: 'Usuário',
  Vector: 'Vetor',
  Partner: 'Parceiro',
  Business: 'Negócio',
  Payment: 'Pagamento',
};

export const AuditTimelinePage = () => {
  const { entityType, entityId } = useParams<{ entityType: string; entityId: string }>();
  const navigate = useNavigate();
  const currentUser = useAuthStore((state) => state.user);

  // Verificar permissão
  const isAdminGlobal = currentUser?.permission === Permission.AdminGlobal;

  // Query para buscar logs da entidade
  const { data, isLoading, error } = useQuery({
    queryKey: ['audit-timeline', entityType, entityId],
    queryFn: async () => {
      const response = await auditApi.list({
        pageSize: 1000,
        sortBy: 'createdAt',
        sortOrder: 'desc',
      });
      // Filtrar logs da entidade específica
      return response.items.filter(
        (log) => log.entity === entityType && log.entityId === entityId
      );
    },
    enabled: isAdminGlobal && !!entityType && !!entityId,
  });

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

  const parseDetails = (details?: string) => {
    if (!details) return null;
    try {
      return JSON.parse(details);
    } catch {
      return null;
    }
  };

  const renderChanges = (log: AuditLog) => {
    const details = parseDetails(log.details);
    if (!details) return null;

    // Se tiver campos específicos de mudança
    if (details.changes) {
      return (
        <div className="mt-3 space-y-2">
          <div className="text-xs font-semibold text-gray-700">Mudanças:</div>
          {Object.entries(details.changes).map(([field, change]: [string, any]) => (
            <div key={field} className="text-xs bg-gray-50 p-2 rounded border border-gray-200">
              <span className="font-medium text-gray-700">{field}:</span>
              <div className="flex items-center gap-2 mt-1">
                <span className="text-red-600 line-through">{String(change.old)}</span>
                <span className="text-gray-400">→</span>
                <span className="text-green-600 font-medium">{String(change.new)}</span>
              </div>
            </div>
          ))}
        </div>
      );
    }

    // Se for uma ação de criação ou outros detalhes relevantes
    if (log.action.includes('Create') || log.action.includes('Payment')) {
      return (
        <div className="mt-3">
          <div className="text-xs font-semibold text-gray-700 mb-1">Detalhes:</div>
          <div className="text-xs bg-gray-50 p-2 rounded border border-gray-200 font-mono overflow-auto max-h-40">
            {JSON.stringify(details, null, 2)}
          </div>
        </div>
      );
    }

    return null;
  };

  // Se não for AdminGlobal, exibir mensagem de permissão negada
  if (!isAdminGlobal) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate(-1)}>
            <ArrowLeft className="w-4 h-4" />
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">Timeline de Auditoria</h1>
        </div>
        <Alert type="warning">
          Você não tem permissão para acessar a timeline de auditoria. Esta funcionalidade está disponível apenas para Administradores Globais.
        </Alert>
      </div>
    );
  }

  if (!entityType || !entityId) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate(-1)}>
            <ArrowLeft className="w-4 h-4" />
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">Timeline de Auditoria</h1>
        </div>
        <Alert type="error">
          Parâmetros inválidos. É necessário informar o tipo e ID da entidade.
        </Alert>
      </div>
    );
  }

  if (isLoading) return <Loading />;
  if (error) return <Alert type="error">Erro ao carregar timeline: {String(error)}</Alert>;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="ghost" onClick={() => navigate(-1)}>
          <ArrowLeft className="w-4 h-4" />
        </Button>
        <div className="flex items-center gap-3">
          <FileText className="w-8 h-8 text-blue-600" />
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Timeline de Auditoria</h1>
            <p className="text-sm text-gray-500">
              {ENTITY_LABELS[entityType as EntityType] || entityType} - ID: {entityId.slice(0, 8)}
            </p>
          </div>
        </div>
      </div>

      {/* Info Card */}
      <Card>
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-lg font-semibold text-gray-900">
              Histórico de {ENTITY_LABELS[entityType as EntityType] || entityType}
            </h2>
            <p className="text-sm text-gray-500">
              {data?.length || 0} {data?.length === 1 ? 'evento registrado' : 'eventos registrados'}
            </p>
          </div>
          <Button onClick={() => navigate('/auditoria')}>
            Ver Todos os Logs
          </Button>
        </div>
      </Card>

      {/* Timeline */}
      {!data || data.length === 0 ? (
        <Alert type="info">
          Nenhum evento de auditoria encontrado para esta entidade.
        </Alert>
      ) : (
        <div className="relative">
          {/* Linha vertical da timeline */}
          <div className="absolute left-8 top-0 bottom-0 w-0.5 bg-gray-200" />

          {/* Eventos */}
          <div className="space-y-6">
            {data.map((log, index) => (
              <div key={log.id} className="relative flex gap-6">
                {/* Ícone da timeline */}
                <div className="relative z-10 flex-shrink-0">
                  <div
                    className={`
                      w-16 h-16 rounded-full 
                      flex items-center justify-center
                      ${
                        getActionBadgeColor(log.action) === 'green'
                          ? 'bg-green-100 text-green-600'
                          : getActionBadgeColor(log.action) === 'blue'
                          ? 'bg-blue-100 text-blue-600'
                          : getActionBadgeColor(log.action) === 'yellow'
                          ? 'bg-yellow-100 text-yellow-600'
                          : getActionBadgeColor(log.action) === 'red'
                          ? 'bg-red-100 text-red-600'
                          : getActionBadgeColor(log.action) === 'purple'
                          ? 'bg-purple-100 text-purple-600'
                          : getActionBadgeColor(log.action) === 'orange'
                          ? 'bg-orange-100 text-orange-600'
                          : 'bg-gray-100 text-gray-600'
                      }
                    `}
                  >
                    <Clock className="w-6 h-6" />
                  </div>
                </div>

                {/* Card do evento */}
                <Card className="flex-1">
                  <div className="space-y-3">
                    {/* Header do evento */}
                    <div className="flex items-start justify-between">
                      <div className="flex items-center gap-3">
                        <Badge variant={getActionBadgeColor(log.action) as any}>
                          {getActionLabel(log.action)}
                        </Badge>
                        <div className="text-sm text-gray-500">
                          {formatDate(log.createdAt)} às{' '}
                          {new Date(log.createdAt).toLocaleTimeString('pt-BR')}
                        </div>
                      </div>
                      {index === 0 && (
                        <span className="text-xs font-semibold text-blue-600 bg-blue-50 px-2 py-1 rounded">
                          MAIS RECENTE
                        </span>
                      )}
                    </div>

                    {/* Usuário que executou a ação */}
                    <div className="flex items-center gap-2 text-sm">
                      <UserIcon className="w-4 h-4 text-gray-400" />
                      <span className="font-medium text-gray-900">{log.userName}</span>
                      <span className="text-gray-500">•</span>
                      <span className="text-xs text-gray-500">ID: {log.userId.slice(0, 8)}</span>
                    </div>

                    {/* Descrição da ação */}
                    <div className="text-sm text-gray-700">
                      <span className="font-medium">{log.action}</span>
                      {log.ipAddress && (
                        <>
                          {' de '}
                          <span className="font-mono text-xs">{log.ipAddress}</span>
                        </>
                      )}
                    </div>

                    {/* Mudanças/Detalhes */}
                    {renderChanges(log)}
                  </div>
                </Card>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Botão voltar ao final */}
      {data && data.length > 5 && (
        <div className="flex justify-center pt-6">
          <Button variant="outline" onClick={() => navigate(-1)}>
            <ArrowLeft className="w-4 h-4 mr-2" />
            Voltar
          </Button>
        </div>
      )}
    </div>
  );
};
