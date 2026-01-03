import { useState, useMemo } from 'react';
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
import { businessTypesApi } from '@/api/endpoints/businessTypes.api';
import { formatCurrency, formatDate } from '@/utils/formatters';
import { Permission } from '@/types/auth.types';
import { BusinessReport } from '@/types/report.types';
import {
  Briefcase,
  DollarSign,
  TrendingUp,
  Award,
  Package,
} from 'lucide-react';

export const BusinessReportPage = () => {
  const { user } = useAuthStore();
  const isAdminGlobal = user?.permission === Permission.AdminGlobal;

  // Estados de filtros
  const [vectorId, setVectorId] = useState<string>('');
  const [startDate, setStartDate] = useState<string>('');
  const [endDate, setEndDate] = useState<string>('');
  const [businessTypeId, setBusinessTypeId] = useState<string>('');
  const [partnerId, setPartnerId] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [minValue, setMinValue] = useState<string>('');
  const [maxValue, setMaxValue] = useState<string>('');
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState<string>('date');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

  const pageSize = 20;

  // Query: Vetores (apenas AdminGlobal)
  const { data: vectorsData } = useQuery({
    queryKey: ['vectors'],
    queryFn: () => vectorsApi.list({ page: 1, pageSize: 1000 }),
    enabled: isAdminGlobal,
  });

  // Query: Parceiros
  const { data: partnersData } = useQuery({
    queryKey: ['partners-select'],
    queryFn: () => partnersApi.list({ page: 1, pageSize: 10000 }),
  });

  // Query: Tipos de Negócio
  const { data: businessTypesData } = useQuery({
    queryKey: ['business-types-select'],
    queryFn: () => businessTypesApi.list({ page: 1, pageSize: 100 }),
  });

  // Query: Relatório de Negócios
  const { data, isLoading } = useQuery({
    queryKey: [
      'business-report',
      vectorId,
      startDate,
      endDate,
      businessTypeId,
      partnerId,
      statusFilter,
      minValue,
      maxValue,
      page,
    ],
    queryFn: () =>
      reportsApi.business({
        vectorId: vectorId || undefined,
        startDate: startDate || undefined,
        endDate: endDate || undefined,
        page,
        pageSize,
      }),
  });

  const businesses = data?.items || [];
  const totalItems = data?.totalItems || 0;
  const totalPages = Math.ceil(totalItems / pageSize);

  // Filtrar no frontend
  const filteredBusinesses = useMemo(() => {
    return businesses.filter((b) => {
      // Filtro por tipo de negócio
      if (businessTypeId && b.businessTypeName !== businessTypesData?.items.find(bt => bt.id === businessTypeId)?.name) {
        return false;
      }
      
      // Filtro por parceiro
      if (partnerId && b.partnerName !== partnersData?.items.find(p => p.id === partnerId)?.name) {
        return false;
      }
      
      // Filtro por status
      if (statusFilter === 'active' && b.status !== 'Active') {
        return false;
      }
      if (statusFilter === 'cancelled' && b.status !== 'Cancelled') {
        return false;
      }
      
      // Filtro por valor mínimo
      if (minValue && b.value < parseFloat(minValue)) {
        return false;
      }
      
      // Filtro por valor máximo
      if (maxValue && b.value > parseFloat(maxValue)) {
        return false;
      }
      
      return true;
    });
  }, [businesses, businessTypeId, partnerId, statusFilter, minValue, maxValue, businessTypesData, partnersData]);

  // Ordenar no frontend
  const sortedBusinesses = useMemo(() => {
    return [...filteredBusinesses].sort((a, b) => {
      let aValue: any = a[sortBy as keyof BusinessReport];
      let bValue: any = b[sortBy as keyof BusinessReport];

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
  }, [filteredBusinesses, sortBy, sortOrder]);

  // Calcular resumos
  const totalBusiness = sortedBusinesses.length;
  const totalValue = sortedBusinesses.reduce((sum, b) => sum + b.value, 0);
  const totalCommission = sortedBusinesses.reduce(
    (sum, b) => sum + b.totalCommission,
    0
  );
  const averageValue = totalBusiness > 0 ? totalValue / totalBusiness : 0;

  // Tipo de negócio mais comum
  const businessTypeCount: { [key: string]: number } = {};
  sortedBusinesses.forEach((b) => {
    businessTypeCount[b.businessTypeName] =
      (businessTypeCount[b.businessTypeName] || 0) + 1;
  });
  const mostCommonType = Object.entries(businessTypeCount).sort(
    (a, b) => b[1] - a[1]
  )[0];

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
    setBusinessTypeId('');
    setPartnerId('');
    setStatusFilter('all');
    setMinValue('');
    setMaxValue('');
    setPage(1);
    setSortBy('date');
    setSortOrder('desc');
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">
          Relatório de Negócios
        </h1>
        <p className="text-gray-600 mt-1">
          Análise completa de negócios e comissões geradas
        </p>
      </div>

      {/* Cards de Resumo */}
      <div className="grid grid-cols-1 md:grid-cols-5 gap-6 mb-6">
        {/* Total de Negócios */}
        <Card className="bg-blue-50 border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-blue-800">
                Total de Negócios
              </p>
              <p className="text-3xl font-bold text-blue-600 mt-2">
                {totalBusiness}
              </p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <Briefcase className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </Card>

        {/* Valor Total */}
        <Card className="bg-green-50 border-green-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-green-800">
                Valor Total
              </p>
              <p className="text-2xl font-bold text-green-600 mt-2">
                {formatCurrency(totalValue)}
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <DollarSign className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </Card>

        {/* Comissão Total */}
        <Card className="bg-purple-50 border-purple-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-purple-800">
                Comissão Total
              </p>
              <p className="text-2xl font-bold text-purple-600 mt-2">
                {formatCurrency(totalCommission)}
              </p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
              <Award className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </Card>

        {/* Valor Médio */}
        <Card className="bg-yellow-50 border-yellow-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-yellow-800">
                Valor Médio
              </p>
              <p className="text-2xl font-bold text-yellow-600 mt-2">
                {formatCurrency(averageValue)}
              </p>
            </div>
            <div className="w-12 h-12 bg-yellow-100 rounded-full flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-yellow-600" />
            </div>
          </div>
        </Card>

        {/* Tipo Mais Comum */}
        <Card className="bg-indigo-50 border-indigo-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-indigo-800">
                Tipo Mais Comum
              </p>
              <p className="text-sm font-bold text-indigo-600 mt-2">
                {mostCommonType ? mostCommonType[0] : 'N/A'}
              </p>
              {mostCommonType && (
                <p className="text-xs text-indigo-700 mt-1">
                  {mostCommonType[1]} negócio(s)
                </p>
              )}
            </div>
            <div className="w-12 h-12 bg-indigo-100 rounded-full flex items-center justify-center">
              <Package className="w-6 h-6 text-indigo-600" />
            </div>
          </div>
        </Card>
      </div>

      {/* Filtros */}
      <Card className="mb-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Filtros</h2>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
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

          {/* Tipo de Negócio */}
          <Select
            label="Tipo de Negócio"
            value={businessTypeId}
            onChange={(e) => {
              setBusinessTypeId(e.target.value);
              setPage(1);
            }}
            options={[
              { value: '', label: 'Todos os Tipos' },
              ...(businessTypesData?.items || [])
                .sort((a, b) => a.name.localeCompare(b.name))
                .map((type) => ({
                  value: type.id,
                  label: type.name,
                })),
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
              { value: 'active', label: 'Ativo' },
              { value: 'cancelled', label: 'Cancelado' },
            ]}
          />

          {/* Valor Mínimo */}
          <Input
            label="Valor Mínimo"
            type="number"
            value={minValue}
            onChange={(e) => {
              setMinValue(e.target.value);
              setPage(1);
            }}
            placeholder="R$ 0,00"
          />

          {/* Valor Máximo */}
          <Input
            label="Valor Máximo"
            type="number"
            value={maxValue}
            onChange={(e) => {
              setMaxValue(e.target.value);
              setPage(1);
            }}
            placeholder="R$ 999.999,99"
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

      {/* Tabela de Negócios */}
      <Card>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-semibold text-gray-900">
            Negócios Detalhados ({sortedBusinesses.length})
          </h2>
        </div>

        {isLoading ? (
          <Loading />
        ) : sortedBusinesses.length === 0 ? (
          <Alert type="info">
            Nenhum negócio encontrado com os filtros aplicados.
          </Alert>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('date')}
                    >
                      Data{' '}
                      {sortBy === 'date' && (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('partnerName')}
                    >
                      Parceiro{' '}
                      {sortBy === 'partnerName' &&
                        (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('businessTypeName')}
                    >
                      Tipo{' '}
                      {sortBy === 'businessTypeName' &&
                        (sortOrder === 'asc' ? '↑' : '↓')}
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
                      onClick={() => handleSort('totalCommission')}
                    >
                      Comissão Total{' '}
                      {sortBy === 'totalCommission' &&
                        (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status Pagamentos
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
                  {sortedBusinesses.map((business) => {
                    const totalPayments =
                      business.paymentsPaid + business.paymentsPending;
                    const paymentPercentage =
                      totalPayments > 0
                        ? (business.paymentsPaid / totalPayments) * 100
                        : 0;

                    return (
                      <tr key={business.businessId} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {formatDate(business.date)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm font-medium text-gray-900">
                            {business.partnerName}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                          {business.businessTypeName}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-green-600">
                          {formatCurrency(business.value)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-purple-600">
                          {formatCurrency(business.totalCommission)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="flex items-center">
                            <div className="flex-1 mr-3">
                              <div className="w-full bg-gray-200 rounded-full h-2">
                                <div
                                  className={`h-2 rounded-full ${
                                    paymentPercentage === 100
                                      ? 'bg-green-500'
                                      : paymentPercentage > 0
                                      ? 'bg-yellow-500'
                                      : 'bg-gray-300'
                                  }`}
                                  style={{ width: `${paymentPercentage}%` }}
                                />
                              </div>
                            </div>
                            <div className="text-xs text-gray-600 min-w-[60px]">
                              {business.paymentsPaid}/{totalPayments} pago
                              {business.paymentsPaid !== 1 ? 's' : ''}
                            </div>
                          </div>
                          <div className="text-xs text-gray-500 mt-1">
                            {paymentPercentage.toFixed(0)}% completo
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <Badge
                            variant={
                              business.status === 'Active'
                                ? 'success'
                                : 'error'
                            }
                          >
                            {business.status === 'Active'
                              ? 'Ativo'
                              : 'Cancelado'}
                          </Badge>
                        </td>
                      </tr>
                    );
                  })}
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
