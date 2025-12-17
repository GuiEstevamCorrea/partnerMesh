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
import { formatCurrency } from '@/utils/formatters';
import { Permission } from '@/types/auth.types';
import { PartnerReport } from '@/types/report.types';
import { Partner } from '@/types/partner.types';
import { Users, TrendingUp, UserCheck, UserX } from 'lucide-react';

export const PartnersReportPage = () => {
  const { user } = useAuthStore();
  const isAdminGlobal = user?.permission === Permission.AdminGlobal;

  // Estados de filtros
  const [vectorId, setVectorId] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [startDate, setStartDate] = useState<string>('');
  const [endDate, setEndDate] = useState<string>('');
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState<string>('partnerName');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('asc');

  const pageSize = 20;

  // Query: Vetores (apenas AdminGlobal)
  const { data: vectorsData } = useQuery({
    queryKey: ['vectors'],
    queryFn: () => vectorsApi.list({ page: 1, pageSize: 1000 }),
    enabled: isAdminGlobal,
  });

  // Query: Relatório de Parceiros
  const { data, isLoading } = useQuery({
    queryKey: [
      'partners-report',
      vectorId,
      statusFilter,
      startDate,
      endDate,
      page,
      sortBy,
      sortOrder,
    ],
    queryFn: () =>
      reportsApi.partners({
        vectorId: vectorId || undefined,
        startDate: startDate || undefined,
        endDate: endDate || undefined,
        page,
        pageSize,
      }),
  });

  // Query: Todos os parceiros para obter stats completos
  const { data: allPartnersData } = useQuery({
    queryKey: ['all-partners'],
    queryFn: () => partnersApi.list({ page: 1, pageSize: 10000 }),
  });

  const partners = data?.items || [];
  const totalItems = data?.totalItems || 0;
  const totalPages = Math.ceil(totalItems / pageSize);

  // Filtrar por status no frontend (se API não suporta)
  const filteredPartners = partners.filter((p) => {
    if (statusFilter === 'active') {
      return (allPartnersData?.items.find((ap: Partner) => ap.id === p.partnerId)?.isActive ?? true);
    }
    if (statusFilter === 'inactive') {
      return !(allPartnersData?.items.find((ap: Partner) => ap.id === p.partnerId)?.isActive ?? true);
    }
    return true; // 'all'
  });

  // Ordenar no frontend
  const sortedPartners = [...filteredPartners].sort((a, b) => {
    let aValue: any = a[sortBy as keyof PartnerReport];
    let bValue: any = b[sortBy as keyof PartnerReport];

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

  // Calcular resumo
  const allPartners = allPartnersData?.items || [];
  const totalPartners = allPartners.length;
  const totalActive = allPartners.filter((p: Partner) => p.isActive).length;
  const totalInactive = totalPartners - totalActive;
  const totalRecommendations = partners.reduce(
    (sum, p) => sum + p.totalRecommended,
    0
  );

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
    setStatusFilter('all');
    setStartDate('');
    setEndDate('');
    setPage(1);
    setSortBy('partnerName');
    setSortOrder('asc');
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">
          Relatório de Parceiros
        </h1>
        <p className="text-gray-600 mt-1">
          Análise completa da rede de parceiros
        </p>
      </div>

      {/* Cards de Resumo */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
        {/* Total de Parceiros */}
        <Card className="bg-blue-50 border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-blue-800">
                Total de Parceiros
              </p>
              <p className="text-3xl font-bold text-blue-600 mt-2">
                {totalPartners}
              </p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <Users className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </Card>

        {/* Total Ativos */}
        <Card className="bg-green-50 border-green-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-green-800">Ativos</p>
              <p className="text-3xl font-bold text-green-600 mt-2">
                {totalActive}
              </p>
              <p className="text-xs text-green-700 mt-1">
                {totalPartners > 0
                  ? ((totalActive / totalPartners) * 100).toFixed(1)
                  : 0}
                % do total
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <UserCheck className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </Card>

        {/* Total Inativos */}
        <Card className="bg-red-50 border-red-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-red-800">Inativos</p>
              <p className="text-3xl font-bold text-red-600 mt-2">
                {totalInactive}
              </p>
              <p className="text-xs text-red-700 mt-1">
                {totalPartners > 0
                  ? ((totalInactive / totalPartners) * 100).toFixed(1)
                  : 0}
                % do total
              </p>
            </div>
            <div className="w-12 h-12 bg-red-100 rounded-full flex items-center justify-center">
              <UserX className="w-6 h-6 text-red-600" />
            </div>
          </div>
        </Card>

        {/* Total de Recomendações */}
        <Card className="bg-purple-50 border-purple-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-purple-800">
                Recomendações
              </p>
              <p className="text-3xl font-bold text-purple-600 mt-2">
                {totalRecommendations}
              </p>
              <p className="text-xs text-purple-700 mt-1">
                Média:{' '}
                {totalPartners > 0
                  ? (totalRecommendations / totalPartners).toFixed(1)
                  : 0}{' '}
                por parceiro
              </p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </Card>
      </div>

      {/* Filtros */}
      <Card className="mb-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Filtros</h2>
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
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
              { value: 'active', label: 'Ativos' },
              { value: 'inactive', label: 'Inativos' },
            ]}
          />

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

          {/* Botão Resetar */}
          <div className="flex items-end">
            <button
              onClick={handleReset}
              className="w-full px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
            >
              Resetar Filtros
            </button>
          </div>
        </div>
      </Card>

      {/* Tabela de Parceiros */}
      <Card>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-semibold text-gray-900">
            Parceiros ({sortedPartners.length})
          </h2>
        </div>

        {isLoading ? (
          <Loading />
        ) : sortedPartners.length === 0 ? (
          <Alert type="info">
            Nenhum parceiro encontrado com os filtros aplicados.
          </Alert>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('partnerName')}
                    >
                      Nome{' '}
                      {sortBy === 'partnerName' && (sortOrder === 'asc' ? '↑' : '↓')}
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
                      onClick={() => handleSort('totalRecommended')}
                    >
                      Recomendados{' '}
                      {sortBy === 'totalRecommended' && (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('totalEarned')}
                    >
                      Total Recebido{' '}
                      {sortBy === 'totalEarned' && (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                      onClick={() => handleSort('totalPending')}
                    >
                      Total a Receber{' '}
                      {sortBy === 'totalPending' && (sortOrder === 'asc' ? '↑' : '↓')}
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {sortedPartners.map((partner) => {
                    const partnerData = allPartnersData?.items.find(
                      (p: Partner) => p.id === partner.partnerId
                    );
                    const isActive = partnerData?.isActive ?? true;

                    return (
                      <tr key={partner.partnerId} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm font-medium text-gray-900">
                            {partner.partnerName}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <Badge
                            variant={
                              partner.level === 1
                                ? 'info'
                                : partner.level === 2
                                ? 'success'
                                : 'default'
                            }
                          >
                            Nível {partner.level}
                          </Badge>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {partner.totalRecommended}
                          {partner.activeRecommendations > 0 && (
                            <span className="text-xs text-green-600 ml-1">
                              ({partner.activeRecommendations} ativos)
                            </span>
                          )}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-green-600">
                          {formatCurrency(partner.totalEarned)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-yellow-600">
                          {formatCurrency(partner.totalPending)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <Badge variant={isActive ? 'success' : 'error'}>
                            {isActive ? 'Ativo' : 'Inativo'}
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
