import { useQuery } from '@tanstack/react-query';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/store/auth.store';
import { Card } from '@/components/common/Card';
import { Badge } from '@/components/common/Badge';
import { Button } from '@/components/common/Button';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { Table } from '@/components/common/Table';
import { businessApi } from '@/api/endpoints/business.api';
import { paymentsApi } from '@/api/endpoints/payments.api';
import { partnersApi } from '@/api/endpoints/partners.api';
import { formatDate } from '@/utils/formatters';
import { Permission } from '@/types/auth.types';
import { Business } from '@/types/business.types';
import { Payment } from '@/types/payment.types';
import { useI18n } from '@/hooks/useI18n';
import {
  Users,
  Network,
  Briefcase,
  DollarSign,
  Clock,
  ChevronRight,
  Eye,
  ArrowRight,
} from 'lucide-react';

export const DashboardPage = () => {
  const navigate = useNavigate();
  const { user } = useAuthStore();
  const { t, formatCurrency } = useI18n();

  // Verificar permissões
  const isAdminVetorOrOperator =
    user?.permission === Permission.AdminVetor ||
    user?.permission === Permission.Operador;

  // Query: Parceiros ativos
  const { data: partnersData } = useQuery({
    queryKey: ['dashboard-partners'],
    queryFn: async () => {
      const data = await partnersApi.list({ page: 1, pageSize: 1000 });
      return {
        ...data,
        items: data.items.filter((p) => p.isActive),
      };
    },
  });

  // Query: Negócios do mês atual
  const startOfMonth = new Date();
  startOfMonth.setDate(1);
  startOfMonth.setHours(0, 0, 0, 0);

  const { data: businessThisMonthData } = useQuery({
    queryKey: ['dashboard-business-month'],
    queryFn: async () => {
      const data = await businessApi.list({ page: 1, pageSize: 1000 });
      return {
        ...data,
        items: data.items.filter(
          (b) => new Date(b.date) >= startOfMonth
        ),
      };
    },
  });

  // Query: Negócios recentes (últimos 10)
  const { data: recentBusinessData, isLoading: isLoadingRecentBusiness } =
    useQuery({
      queryKey: ['dashboard-recent-business'],
      queryFn: () => businessApi.list({ page: 1, pageSize: 10 }),
    });

  // Query: Pagamentos pendentes
  const { data: pendingPaymentsData, isLoading: isLoadingPendingPayments } =
    useQuery({
      queryKey: ['dashboard-pending-payments'],
      queryFn: () =>
        paymentsApi.list({ page: 1, pageSize: 10, status: 'Pending' }),
    });

  // Query: Comissões pagas no mês
  const { data: paidPaymentsData } = useQuery({
    queryKey: ['dashboard-paid-payments-month'],
    queryFn: () =>
      paymentsApi.list({
        page: 1,
        pageSize: 1000,
        status: 'Paid',
        startDate: startOfMonth.toISOString().split('T')[0],
      }),
  });

  // Calcular totais
  const totalActivePartners = partnersData?.items?.length || 0;
  const totalBusinessThisMonth = businessThisMonthData?.items?.length || 0;
  const totalPendingCommissions =
    pendingPaymentsData?.items?.reduce((sum, p) => sum + p.value, 0) || 0;
  const totalPaidCommissionsThisMonth =
    paidPaymentsData?.items?.reduce((sum, p) => sum + p.value, 0) || 0;

  const recentBusiness = recentBusinessData?.items || [];
  const pendingPayments = pendingPaymentsData?.items || [];

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">{t('dashboard.title')}</h1>
        <p className="text-gray-600 mt-1">{t('common.welcome')}, {user?.name}!</p>
      </div>

      {/* Seção: Visão Geral - Cards de Métricas */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
        {/* Parceiros Ativos */}
        <Card className="bg-blue-50 border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-blue-800">
                {t('dashboard.kpis.activePartners')}
              </p>
              <p className="text-3xl font-bold text-blue-600 mt-2">
                {totalActivePartners}
              </p>
              <Link
                to="/parceiros"
                className="text-xs text-blue-700 hover:text-blue-900 mt-2 inline-flex items-center"
              >
                {t('common.viewAll')}
                <ChevronRight className="w-3 h-3 ml-1" />
              </Link>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <Users className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </Card>

        {/* Negócios (Mês Atual) */}
        <Card className="bg-purple-50 border-purple-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-purple-800">
                {t('dashboard.kpis.businessThisMonth')}
              </p>
              <p className="text-3xl font-bold text-purple-600 mt-2">
                {totalBusinessThisMonth}
              </p>
              <Link
                to="/negocios"
                className="text-xs text-purple-700 hover:text-purple-900 mt-2 inline-flex items-center"
              >
                {t('common.viewAll')}
                <ChevronRight className="w-3 h-3 ml-1" />
              </Link>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
              <Briefcase className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </Card>

        {/* Comissões Pendentes */}
        <Card className="bg-yellow-50 border-yellow-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-yellow-800">
                {t('dashboard.commissionsPending')}
              </p>
              <p className="text-3xl font-bold text-yellow-600 mt-2">
                {formatCurrency(totalPendingCommissions)}
              </p>
              <Link
                to="/pagamentos"
                className="text-xs text-yellow-700 hover:text-yellow-900 mt-2 inline-flex items-center"
              >
                {t('dashboard.seePayments')}
                <ChevronRight className="w-3 h-3 ml-1" />
              </Link>
            </div>
            <div className="w-12 h-12 bg-yellow-100 rounded-full flex items-center justify-center">
              <Clock className="w-6 h-6 text-yellow-600" />
            </div>
          </div>
        </Card>

        {/* Comissões Pagas (Mês Atual) */}
        <Card className="bg-green-50 border-green-200">
          <div className="flex items-center justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-green-800 mb-3">
                {t('dashboard.paidCurrentMonth')}
              </p>
              <p className="text-2xl font-bold text-green-600 break-words">
                {formatCurrency(totalPaidCommissionsThisMonth)}
              </p>
              <Link
                to="/pagamentos"
                className="text-xs text-green-700 hover:text-green-900 mt-3 inline-flex items-center"
              >
                {t('dashboard.seePayments')}
                <ChevronRight className="w-3 h-3 ml-1" />
              </Link>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <DollarSign className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </Card>
      </div>

      {/* Seção: Negócios Recentes */}
      <Card className="mb-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold text-gray-900 flex items-center">
            <Briefcase className="w-5 h-5 mr-2" />
            {t('dashboard.recentBusiness')}
          </h2>
          <Link to="/negocios">
            <Button variant="secondary" size="sm">
              {t('dashboard.viewAll')}
              <ArrowRight className="w-4 h-4 ml-2" />
            </Button>
          </Link>
        </div>

        {isLoadingRecentBusiness ? (
          <Loading />
        ) : recentBusiness.length === 0 ? (
          <Alert type="info">{t('dashboard.noRecentBusiness')}</Alert>
        ) : (
          <Table
            data={recentBusiness}
            columns={[
              {
                key: 'id',
                header: 'ID',
                render: (business: Business) => (
                  <span className="text-sm font-mono text-gray-700">
                    #{business.id.substring(0, 8)}
                  </span>
                ),
              },
              {
                key: 'partner',
                header: t('dashboard.partner'),
                render: (business: Business) => (
                  <span className="font-medium text-gray-900">
                    {business.partnerName}
                  </span>
                ),
              },
              {
                key: 'type',
                header: t('dashboard.type'),
                render: (business: Business) => (
                  <span className="text-gray-700">
                    {business.businessTypeName}
                  </span>
                ),
              },
              {
                key: 'value',
                header: t('dashboard.value'),
                render: (business: Business) => (
                  <span className="font-semibold text-green-600">
                    {formatCurrency(business.value)}
                  </span>
                ),
              },
              {
                key: 'date',
                header: t('dashboard.date'),
                render: (business: Business) => (
                  <span className="text-gray-700">
                    {formatDate(business.date)}
                  </span>
                ),
              },
              {
                key: 'status',
                header: t('dashboard.status'),
                render: (business: Business) => (
                  <Badge
                    variant={
                      business.status === 'Active' ? 'success' : 'error'
                    }
                  >
                    {business.status === 'Active' ? t('common.active') : t('business.status.canceled')}
                  </Badge>
                ),
              },
              {
                key: 'actions',
                header: t('dashboard.actions'),
                render: (business: Business) => (
                  <button
                    onClick={() => navigate(`/negocios/${business.id}`)}
                    className="text-gray-600 hover:text-gray-900"
                  >
                    <Eye className="w-4 h-4" />
                  </button>
                ),
              },
            ]}
          />
        )}
      </Card>

      {/* Seção: Pagamentos Pendentes */}
      <Card className="mb-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold text-gray-900 flex items-center">
            <DollarSign className="w-5 h-5 mr-2" />
            {t('dashboard.pendingPayments')}
          </h2>
          <Link to="/pagamentos">
            <Button variant="secondary" size="sm">
              {t('dashboard.viewAll')}
              <ArrowRight className="w-4 h-4 ml-2" />
            </Button>
          </Link>
        </div>

        {isLoadingPendingPayments ? (
          <Loading />
        ) : pendingPayments.length === 0 ? (
          <Alert type="info">{t('dashboard.noPendingPayments')}</Alert>
        ) : (
          <Table
            data={pendingPayments}
            columns={[
              {
                key: 'id',
                header: 'ID',
                render: (payment: Payment) => (
                  <span className="text-sm font-mono text-gray-700">
                    #{payment.id.substring(0, 8)}
                  </span>
                ),
              },
              {
                key: 'recipient',
                header: t('dashboard.recipient'),
                render: (payment: Payment) => (
                  <div>
                    <p className="font-medium text-gray-900">
                      {payment.recipientName}
                    </p>
                    <p className="text-xs text-gray-600">
                      {payment.recipientType}
                    </p>
                  </div>
                ),
              },
              {
                key: 'level',
                header: t('dashboard.level'),
                render: (payment: Payment) => (
                  <Badge
                    variant={
                      payment.level === 1
                        ? 'info'
                        : payment.level === 2
                        ? 'success'
                        : 'default'
                    }
                  >
                    {t('dashboard.level')} {payment.level}
                  </Badge>
                ),
              },
              {
                key: 'value',
                header: t('dashboard.value'),
                render: (payment: Payment) => (
                  <span className="font-semibold text-yellow-600">
                    {formatCurrency(payment.value)}
                  </span>
                ),
              },
              {
                key: 'createdAt',
                header: t('dashboard.createdAt'),
                render: (payment: Payment) => (
                  <span className="text-gray-700">
                    {formatDate(payment.createdAt)}
                  </span>
                ),
              },
            ]}
          />
        )}
      </Card>

      {/* Seção: Árvore de Parceiros (AdminVetor/Operador) */}
      {isAdminVetorOrOperator && (
        <Card>
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-xl font-semibold text-gray-900 flex items-center mb-2">
                <Network className="w-5 h-5 mr-2" />
                {t('dashboard.partnerTree')}
              </h2>
              <p className="text-sm text-gray-600">
                {t('dashboard.viewHierarchy')}
              </p>
              <div className="flex gap-4 mt-4">
                <div className="flex items-center">
                  <div className="w-3 h-3 bg-blue-500 rounded-full mr-2" />
                  <span className="text-sm text-gray-700">{t('dashboard.level')} 1</span>
                </div>
                <div className="flex items-center">
                  <div className="w-3 h-3 bg-green-500 rounded-full mr-2" />
                  <span className="text-sm text-gray-700">{t('dashboard.level')} 2</span>
                </div>
                <div className="flex items-center">
                  <div className="w-3 h-3 bg-purple-500 rounded-full mr-2" />
                  <span className="text-sm text-gray-700">{t('dashboard.level')} 3+</span>
                </div>
              </div>
            </div>
            <Link to="/parceiros/arvore">
              <Button variant="primary">
                {t('dashboard.viewCompleteTree')}
                <ArrowRight className="w-4 h-4 ml-2" />
              </Button>
            </Link>
          </div>
        </Card>
      )}
    </div>
  );
};
