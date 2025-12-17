import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/store/auth.store';
import { Card } from '@/components/common/Card';
import { Input } from '@/components/common/Input';
import { Select } from '@/components/common/Select';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { Badge } from '@/components/common/Badge';
import { reportsApi } from '@/api/endpoints/reports.api';
import { vectorsApi } from '@/api/endpoints/vectors.api';
import { partnersApi } from '@/api/endpoints/partners.api';
import { paymentsApi } from '@/api/endpoints/payments.api';
import { formatCurrency, formatDate } from '@/utils/formatters';
import { Permission } from '@/types/auth.types';
import { Payment } from '@/types/payment.types';
import {
  DollarSign,
  Clock,
  TrendingUp,
  Layers,
  CheckCircle,
} from 'lucide-react';

export const FinancialReportPage = () => {
  const { user } = useAuthStore();
  const isAdminGlobal = user?.permission === Permission.AdminGlobal;

  // Estados de filtros
  const [vectorId, setVectorId] = useState<string>('');
  const [startDate, setStartDate] = useState<string>('');
  const [endDate, setEndDate] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [levelFilter, setLevelFilter] = useState<string>('all');
  const [partnerId, setPartnerId] = useState<string>('');
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState<string>('createdAt');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

  const pageSize = 20;

  // Query: Vetores (apenas AdminGlobal)
  const { data: vectorsData } = useQuery({
    queryKey: ['vectors'],
    queryFn: () => vectorsApi.list({ page: 1, pageSize: 1000 }),
    enabled: isAdminGlobal,
  });

  // Query: Parceiros (para select)
  const { data: partnersData } = useQuery({
    queryKey: ['partners-select'],
    queryFn: () => partnersApi.list({ page: 1, pageSize: 10000 }),
  });

  // Query: Relatório Financeiro (resumo)
  const { data: financialReport } = useQuery({
    queryKey: ['financial-report', vectorId, startDate, endDate],
    queryFn: () =>
      reportsApi.financial({
        vectorId: vectorId || undefined,
        startDate: startDate || undefined,
        endDate: endDate || undefined,
      }),
  });

  // Query: Pagamentos (tabela detalhada)
  const { data: paymentsData, isLoading } = useQuery({
    queryKey: [
      'financial-payments',
      vectorId,
      startDate,
      endDate,
      statusFilter,
      levelFilter,
      partnerId,
      page,
    ],
    queryFn: () =>
      paymentsApi.list({
        page,
        pageSize,
        status: statusFilter === 'all' ? undefined : (statusFilter as 'Paid' | 'Pending'),
        startDate: startDate || undefined,
        endDate: endDate || undefined,
      }),
  });

  const payments = paymentsData?.items || [];
  const totalItems = paymentsData?.totalItems || 0;
  const totalPages = Math.ceil(totalItems / pageSize);

  // Filtrar por nível e parceiro no frontend
  const filteredPayments = payments.filter((p) => {
    if (levelFilter !== 'all' && p.level !== parseInt(levelFilter)) {
      return false;
    }
    if (partnerId && p.recipientId !== partnerId) {
      return false;
    }
    return true;
  });

  // Ordenar no frontend
  const sortedPayments = [...filteredPayments].sort((a, b) => {
    let aValue: any = a[sortBy as keyof Payment];
    let bValue: any = b[sortBy as keyof Payment];

    if (typeof aValue === 'string') {
      aValue = aValue.toLowerCase();
      bValue = bValue.toLowerCase();
    }

    if (sortOrder === 'asc') {
      return aValue > bValue ? 1 : -1;
    } else {
      return aValue < bValue ? 1 : -1;
    }
  });

  // Calcular resumos
  const totalPaid = financialReport?.totalPaid || 0;
  const totalPending = financialReport?.totalPending || 0;
  const paymentsByLevel = financialReport?.paymentsByLevel || [];

  // Total por nível
  const level1Total =
    paymentsByLevel.find((p) => p.level === 1)?.total || 0;
  const level2Total =
    paymentsByLevel.find((p) => p.level === 2)?.total || 0;
  const level3Total =
    paymentsByLevel.find((p) => p.level === 3)?.total || 0;

  // Total por vetor (se AdminGlobal) - calcular do filteredPayments
  const totalByVector: { [key: string]: number } = {};
  if (isAdminGlobal && vectorsData) {
    vectorsData.items.forEach((vector) => {
      const vectorPayments = sortedPayments.filter((p) => {
        const partner = partnersData?.items.find(
          (partner) => partner.id === p.recipientId
        );
        return partner?.vectorId === vector.id;
      });
      totalByVector[vector.name] = vectorPayments.reduce(
        (sum, p) => sum + p.value,
        0
      );
    });
  }

  // Handler de ordenação
  const handleSort = (column: string) => {
    if (sortBy === column) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(column);
      setSortOrder('asc');
    }
  };

  // Resetar filtros
  const handleReset = () => {
    setVectorId('');
    setStartDate('');
    setEndDate('');
    setStatusFilter('all');
    setLevelFilter('all');
    setPartnerId('');
    setPage(1);
    setSortBy('createdAt');
    setSortOrder('desc');
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">
          Relatório Financeiro
        </h1>
        <p className="text-gray-600 mt-1">
          Análise completa de pagamentos e comissões
        </p>
      </div>

      {/* Cards de Resumo */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        {/* Total Pago */}
        <Card className="bg-green-50 border-green-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-green-800">
                Total Pago no Período
              </p>
              <p className="text-3xl font-bold text-green-600 mt-2">
                {formatCurrency(totalPaid)}
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <CheckCircle className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </Card>

        {/* Total Pendente */}
        <Card className="bg-yellow-50 border-yellow-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-yellow-800">
                Total Pendente
              </p>
              <p className="text-3xl font-bold text-yellow-600 mt-2">
                {formatCurrency(totalPending)}
              </p>
            </div>
            <div className="w-12 h-12 bg-yellow-100 rounded-full flex items-center justify-center">
              <Clock className="w-6 h-6 text-yellow-600" />
            </div>
          </div>
        </Card>

        {/* Total Geral */}
        <Card className="bg-blue-50 border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-blue-800">Total Geral</p>
              <p className="text-3xl font-bold text-blue-600 mt-2">
                {formatCurrency(totalPaid + totalPending)}
              </p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <DollarSign className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </Card>
      </div>

      {/* Cards: Total por Nível */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        {/* Nível 1 */}
        <Card className="bg-blue-50 border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-blue-800">Nível 1</p>
              <p className="text-2xl font-bold text-blue-600 mt-2">
                {formatCurrency(level1Total)}
              </p>
              <p className="text-xs text-blue-700 mt-1">
                {totalPaid + totalPending > 0
                  ? (
                      (level1Total / (totalPaid + totalPending)) *
                      100
                    ).toFixed(1)
                  : 0}
                % do total
              </p>
            </div>
            <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
              <Layers className="w-5 h-5 text-blue-600" />
            </div>
          </div>
        </Card>

        {/* Nível 2 */}
        <Card className="bg-green-50 border-green-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-green-800">Nível 2</p>
              <p className="text-2xl font-bold text-green-600 mt-2">
                {formatCurrency(level2Total)}
              </p>
              <p className="text-xs text-green-700 mt-1">
                {totalPaid + totalPending > 0
                  ? (
                      (level2Total / (totalPaid + totalPending)) *
                      100
                    ).toFixed(1)
                  : 0}
                % do total
              </p>
            </div>
            <div className="w-10 h-10 bg-green-100 rounded-full flex items-center justify-center">
              <Layers className="w-5 h-5 text-green-600" />
            </div>
          </div>
        </Card>

        {/* Nível 3+ */}
        <Card className="bg-purple-50 border-purple-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-purple-800">Nível 3+</p>
              <p className="text-2xl font-bold text-purple-600 mt-2">
                {formatCurrency(level3Total)}
              </p>
              <p className="text-xs text-purple-700 mt-1">
                {totalPaid + totalPending > 0
                  ? (
                      (level3Total / (totalPaid + totalPending)) *
                      100
                    ).toFixed(1)
                  : 0}
                % do total
              </p>
            </div>
            <div className="w-10 h-10 bg-purple-100 rounded-full flex items-center justify-center">
              <Layers className="w-5 h-5 text-purple-600" />
            </div>
          </div>
        </Card>
      </div>

      {/* Card: Total por Vetor (apenas AdminGlobal) */}
      {isAdminGlobal && vectorsData && Object.keys(totalByVector).length > 0 && (
        <Card className="mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            Total por Vetor
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {Object.entries(totalByVector).map(([vectorName, total]) => (
              <div
                key={vectorName}
                className="flex items-center justify-between p-4 bg-gray-50 rounded-lg"
              >
                <div>
                  <p className="text-sm font-medium text-gray-700">
                    {vectorName}
                  </p>
                  <p className="text-xl font-bold text-gray-900 mt-1">
                    {formatCurrency(total)}
                  </p>
                </div>
                <TrendingUp className="w-5 h-5 text-gray-500" />
              </div>
            ))}
          </div>
        </Card>
      )}

      {/* Filtros */}
      <Card className="mb-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Filtros</h2>
        <div className="grid grid-cols-1 md:grid-cols-6 gap-4">
          {/* Vetor (apenas AdminGlobal) */}
          {isAdminGlobal && (
            <Select
              label="Vetor"
              value={vectorId}
              onChange={(e) => {
                setVectorId(e.target.value);
                setPage(1);
              }}
              options={[
                { value: '', label: 'Todos os Vetores' },
                ...(vectorsData?.items || []).map((vector) => ({
                  value: vector.id,
                  label: vector.name,
                })),
              ]}
            />
          )}

          {/* Data Início */}
          <Input
            label="Data Início"
            type="date"
            value={startDate}
            onChange={(e) => {
              setStartDate(e.target.value);
              setPage(1);
            }}
          />

          {/* Data Fim */}
          <Input
            label="Data Fim"
            type="date"
            value={endDate}
            onChange={(e) => {
              setEndDate(e.target.value);
              setPage(1);
            }}
          />

          {/* Status */}
          <Select
            label="Status"
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPage(1);
            }}
            options={[
              { value: 'all', label: 'Todos' },
              { value: 'Paid', label: 'Pago' },
              { value: 'Pending', label: 'Pendente' },
            ]}
          />

          {/* Nível */}
          <Select
            label="Nível"
            value={levelFilter}
            onChange={(e) => {
              setLevelFilter(e.target.value);
              setPage(1);
            }}
            options={[
              { value: 'all', label: 'Todos os Níveis' },
              { value: '1', label: 'Nível 1' },
              { value: '2', label: 'Nível 2' },
              { value: '3', label: 'Nível 3+' },
            ]}
          />

          {/* Parceiro */}
          <Select
            label="Parceiro"
            value={partnerId}
            onChange={(e) => {
              setPartnerId(e.target.value);
              setPage(1);
            }}
            options={[
              { value: '', label: 'Todos os Parceiros' },
              ...(partnersData?.items || [])
                .sort((a, b) => a.name.localeCompare(b.name))
                .map((partner) => ({
                  value: partner.id,
                  label: partner.name,
                })),
            ]}
          />
        </div>
        <div className="mt-4">
          <button
            onClick={handleReset}
            className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
          >
            Resetar Filtros
          </button>
        </div>
      </Card>

      {/* Tabela Detalhada */}
      <Card>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-semibold text-gray-900">
            Pagamentos Detalhados ({sortedPayments.length})
          </h2>
        </div>

        {isLoading ? (
          <Loading />
        ) : sortedPayments.length === 0 ? (
          <Alert type="info">
            Nenhum pagamento encontrado com os filtros aplicados.
          </Alert>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('createdAt')}
                    >
                      Data{' '}
                      {sortBy === 'createdAt' &&
                        (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('recipientName')}
                    >
                      Destinatário{' '}
                      {sortBy === 'recipientName' &&
                        (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Negócio ID
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('value')}
                    >
                      Valor{' '}
                      {sortBy === 'value' && (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('level')}
                    >
                      Nível{' '}
                      {sortBy === 'level' && (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('status')}
                    >
                      Status{' '}
                      {sortBy === 'status' && (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {sortedPayments.map((payment) => (
                    <tr key={payment.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {formatDate(payment.createdAt)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-gray-900">
                          {payment.recipientName}
                        </div>
                        <div className="text-xs text-gray-500">
                          {payment.recipientType}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                        <span className="font-mono">
                          #{payment.businessId.substring(0, 8)}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold">
                        <span
                          className={
                            payment.status === 'Paid'
                              ? 'text-green-600'
                              : 'text-yellow-600'
                          }
                        >
                          {formatCurrency(payment.value)}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <Badge
                          variant={
                            payment.level === 1
                              ? 'info'
                              : payment.level === 2
                              ? 'success'
                              : 'default'
                          }
                        >
                          Nível {payment.level}
                        </Badge>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <Badge
                          variant={
                            payment.status === 'Paid' ? 'success' : 'warning'
                          }
                        >
                          {payment.status === 'Paid' ? 'Pago' : 'Pendente'}
                        </Badge>
                        {payment.paidAt && (
                          <div className="text-xs text-gray-500 mt-1">
                            {formatDate(payment.paidAt)}
                          </div>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Paginação */}
            {totalPages > 1 && (
              <div className="flex items-center justify-between mt-6 pt-6 border-t">
                <div className="text-sm text-gray-700">
                  Mostrando {(page - 1) * pageSize + 1} até{' '}
                  {Math.min(page * pageSize, totalItems)} de {totalItems}{' '}
                  resultados
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                    disabled={page === 1}
                    className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Anterior
                  </button>
                  <span className="px-4 py-2 text-sm font-medium text-gray-700">
                    Página {page} de {totalPages}
                  </span>
                  <button
                    onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                    disabled={page === totalPages}
                    className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Próxima
                  </button>
                </div>
              </div>
            )}
          </>
        )}
      </Card>
    </div>
  );
};
