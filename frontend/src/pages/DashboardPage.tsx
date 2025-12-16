import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { useAuthStore } from '@/store/auth.store';
import { Card, Badge, Button, Loading, Alert } from '@/components';
import { dashboardApi } from '@/api/endpoints';
import { formatCurrency, formatDate, formatNumber } from '@/utils/formatters';
import {
  Users,
  Network,
  Briefcase,
  DollarSign,
  TrendingUp,
  Clock,
  ChevronRight,
} from 'lucide-react';

export const DashboardPage = () => {
  const user = useAuthStore((state) => state.user);

  const { data, isLoading, error } = useQuery({
    queryKey: ['dashboard'],
    queryFn: dashboardApi.getDashboardData,
  });

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
          Erro ao carregar dados do dashboard. Tente novamente mais tarde.
        </Alert>
      </div>
    );
  }

  const { stats, recentBusiness, pendingPayments } = data || {
    stats: {
      totalActiveUsers: 0,
      totalActiveVectors: 0,
      totalPartners: 0,
      totalBusinessThisMonth: 0,
      pendingCommissionsAmount: 0,
      paidCommissionsThisMonth: 0,
    },
    recentBusiness: [],
    pendingPayments: [],
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">Bem-vindo, {user?.name}!</p>
      </div>

      {/* Estatísticas Principais */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {/* Usuários Ativos */}
        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm font-medium">
                Usuários Ativos
              </p>
              <p className="text-3xl font-bold text-gray-900 mt-2">
                {formatNumber(stats.totalActiveUsers)}
              </p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <Users className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </Card>

        {/* Vetores Ativos */}
        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm font-medium">
                Vetores Ativos
              </p>
              <p className="text-3xl font-bold text-gray-900 mt-2">
                {formatNumber(stats.totalActiveVectors)}
              </p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
              <Network className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </Card>

        {/* Total de Parceiros */}
        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm font-medium">
                Total de Parceiros
              </p>
              <p className="text-3xl font-bold text-gray-900 mt-2">
                {formatNumber(stats.totalPartners)}
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <Briefcase className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </Card>

        {/* Negócios do Mês */}
        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm font-medium">
                Negócios (Mês)
              </p>
              <p className="text-3xl font-bold text-gray-900 mt-2">
                {formatNumber(stats.totalBusinessThisMonth)}
              </p>
            </div>
            <div className="w-12 h-12 bg-orange-100 rounded-full flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-orange-600" />
            </div>
          </div>
        </Card>
      </div>

      {/* Comissões - Cards Financeiros */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Comissões Pendentes */}
        <Card>
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold text-gray-900">
              Comissões Pendentes
            </h3>
            <Clock className="w-5 h-5 text-orange-500" />
          </div>
          <p className="text-3xl font-bold text-orange-600">
            {formatCurrency(stats.pendingCommissionsAmount)}
          </p>
          <Link
            to="/pagamentos"
            className="mt-4 inline-flex items-center text-sm text-gray-600 hover:text-gray-900"
          >
            Ver todos os pagamentos
            <ChevronRight className="w-4 h-4 ml-1" />
          </Link>
        </Card>

        {/* Comissões Pagas no Mês */}
        <Card>
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold text-gray-900">
              Pagas no Mês
            </h3>
            <DollarSign className="w-5 h-5 text-green-500" />
          </div>
          <p className="text-3xl font-bold text-green-600">
            {formatCurrency(stats.paidCommissionsThisMonth)}
          </p>
          <Link
            to="/relatorios/financeiro"
            className="mt-4 inline-flex items-center text-sm text-gray-600 hover:text-gray-900"
          >
            Ver relatório financeiro
            <ChevronRight className="w-4 h-4 ml-1" />
          </Link>
        </Card>
      </div>

      {/* Negócios Recentes */}
      <Card title="Negócios Recentes">
        <div className="space-y-4">
          {recentBusiness.length === 0 ? (
            <p className="text-gray-500 text-center py-4">
              Nenhum negócio recente.
            </p>
          ) : (
            <>
              {recentBusiness.map((business) => (
                <div
                  key={business.id}
                  className="flex items-center justify-between p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
                >
                  <div className="flex-1">
                    <div className="flex items-center gap-3">
                      <p className="font-semibold text-gray-900">
                        {business.partnerName}
                      </p>
                      <Badge
                        variant={
                          business.status === 'active' ? 'success' : 'error'
                        }
                      >
                        {business.status === 'active' ? 'Ativo' : 'Cancelado'}
                      </Badge>
                    </div>
                    <p className="text-sm text-gray-600 mt-1">
                      {business.businessTypeName} •{' '}
                      {formatDate(business.createdAt)}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="font-bold text-gray-900">
                      {formatCurrency(business.value)}
                    </p>
                    <p className="text-sm text-gray-600">
                      Comissão: {formatCurrency(business.totalCommission)}
                    </p>
                  </div>
                  <Link
                    to={`/negocios/${business.id}`}
                    className="ml-4 text-gray-400 hover:text-gray-600"
                  >
                    <ChevronRight className="w-5 h-5" />
                  </Link>
                </div>
              ))}
              <div className="pt-4 border-t">
                <Link to="/negocios">
                  <Button variant="outline" className="w-full">
                    Ver Todos os Negócios
                  </Button>
                </Link>
              </div>
            </>
          )}
        </div>
      </Card>

      {/* Pagamentos Pendentes */}
      <Card title="Pagamentos Pendentes">
        <div className="space-y-4">
          {pendingPayments.length === 0 ? (
            <p className="text-gray-500 text-center py-4">
              Nenhum pagamento pendente.
            </p>
          ) : (
            <>
              {pendingPayments.map((payment) => (
                <div
                  key={payment.id}
                  className="flex items-center justify-between p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
                >
                  <div className="flex-1">
                    <div className="flex items-center gap-3">
                      <p className="font-semibold text-gray-900">
                        {payment.recipientName}
                      </p>
                      <Badge variant="warning">Nível {payment.level}</Badge>
                    </div>
                    <p className="text-sm text-gray-600 mt-1">
                      Negócio #{payment.businessId} •{' '}
                      {formatDate(payment.createdAt)}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="font-bold text-orange-600">
                      {formatCurrency(payment.amount)}
                    </p>
                  </div>
                </div>
              ))}
              <div className="pt-4 border-t">
                <Link to="/pagamentos">
                  <Button className="w-full">Processar Pagamentos</Button>
                </Link>
              </div>
            </>
          )}
        </div>
      </Card>

      {/* Ações Rápidas */}
      <Card title="Ações Rápidas">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <Link to="/negocios/novo">
            <Button variant="outline" className="w-full">
              + Novo Negócio
            </Button>
          </Link>
          <Link to="/parceiros/novo">
            <Button variant="outline" className="w-full">
              + Novo Parceiro
            </Button>
          </Link>
          <Link to="/parceiros/arvore">
            <Button variant="outline" className="w-full">
              Ver Árvore
            </Button>
          </Link>
          <Link to="/relatorios/negocios">
            <Button variant="outline" className="w-full">
              Relatórios
            </Button>
          </Link>
        </div>
      </Card>
    </div>
  );
};
