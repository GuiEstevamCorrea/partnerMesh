import { useState, useMemo, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { DollarSign, CheckSquare, Square, XCircle } from 'lucide-react';

import { paymentsApi } from '@/api/endpoints/payments.api';
import { vectorsApi } from '@/api/endpoints/vectors.api';
import { Payment } from '@/types/payment.types';
import { useToast } from '@/components/common/Toast';
import { useAuthStore } from '@/store/auth.store';
import { Permission } from '@/types/auth.types';
import { formatCurrency, formatDate } from '@/utils/formatters';

import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Card } from '@/components/common/Card';
import { Badge } from '@/components/common/Badge';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { Table } from '@/components/common/Table';
import { Pagination } from '@/components/common/Pagination';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';

const PaymentsListPage = () => {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const { user } = useAuthStore();

  // Permissões
  const isAdminGlobal = user?.permission === Permission.AdminGlobal;

  // Estados de filtros
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');
  const [levelFilter, setLevelFilter] = useState('');
  const [vectorFilter, setVectorFilter] = useState('');
  const [dateStart, setDateStart] = useState('');
  const [dateEnd, setDateEnd] = useState('');

  // Seleção múltipla
  const [selectedPayments, setSelectedPayments] = useState<Set<string>>(new Set());

  // ConfirmDialog
  const [confirmDialog, setConfirmDialog] = useState({
    isOpen: false,
    payments: [] as Payment[],
  });

  // Query: Lista de pagamentos
  const {
    data: paymentsData,
    isLoading,
    error,
  } = useQuery({
    queryKey: [
      'payments',
      page,
      statusFilter,
      levelFilter,
      vectorFilter,
      dateStart,
      dateEnd,
    ],
    queryFn: () =>
      paymentsApi.list({
        page,
        pageSize: 20,
        status: (statusFilter as 'Pending' | 'Paid' | undefined) || undefined,
        level: levelFilter ? Number(levelFilter) : undefined,
        vectorId: vectorFilter || undefined,
        startDate: dateStart || undefined,
        endDate: dateEnd || undefined,
      }),
  });

  // Query: Lista de vetores (para filtro - apenas AdminGlobal)
  const { data: vectorsData } = useQuery({
    queryKey: ['vectors-all'],
    queryFn: () => vectorsApi.list({ page: 1, pageSize: 100 }),
    enabled: isAdminGlobal,
  });

  // Query: Resumo de negócios cancelados
  const { data: cancelledSummary } = useQuery({
    queryKey: ['cancelled-business-summary'],
    queryFn: () => paymentsApi.getCancelledBusinessSummary(),
  });

  // Mutation: Processar pagamentos
  const processPaymentsMutation = useMutation({
    mutationFn: (paymentIds: string[]) =>
      paymentsApi.process({ paymentIds }),
    onSuccess: () => {
      showToast('success', 'Pagamentos processados com sucesso');
      // Invalidar queries relacionadas a pagamentos
      queryClient.invalidateQueries({ queryKey: ['payments'] });
      // Invalidar queries relacionadas a negócios para atualizar status
      queryClient.invalidateQueries({ queryKey: ['businesses'] });
      queryClient.invalidateQueries({ queryKey: ['business'] });
      // Invalidar relatórios que dependem de status de negócios
      queryClient.invalidateQueries({ queryKey: ['business-report'] });
      queryClient.invalidateQueries({ queryKey: ['financial-report'] });
      queryClient.invalidateQueries({ queryKey: ['financial-payments'] });
      setSelectedPayments(new Set());
      setConfirmDialog({ isOpen: false, payments: [] });
    },
    onError: (error: any) => {
      showToast(
        'error',
        error.response?.data?.message || 'Erro ao processar pagamentos'
      );
    },
  });

  // Handlers
  const handleFilterChange = () => {
    setPage(1);
  };

  const handleClearFilters = () => {
    setStatusFilter('');
    setLevelFilter('');
    setVectorFilter('');
    setDateStart('');
    setDateEnd('');
    setPage(1);
  };

  const handleSelectPayment = (paymentId: string) => {
    const newSelected = new Set(selectedPayments);
    if (newSelected.has(paymentId)) {
      newSelected.delete(paymentId);
    } else {
      newSelected.add(paymentId);
    }
    setSelectedPayments(newSelected);
  };

  const handleProcessPayments = () => {
    if (selectedPayments.size === 0) {
      showToast('warning', 'Selecione ao menos um pagamento');
      return;
    }

    const payments =
      paymentsData?.items.filter((p) => selectedPayments.has(p.id)) || [];
    setConfirmDialog({ isOpen: true, payments });
  };

  const handleConfirmProcess = () => {
    processPaymentsMutation.mutate(Array.from(selectedPayments));
  };

  // Mensagem do ConfirmDialog
  const confirmMessage = useMemo(() => {
    if (confirmDialog.payments.length === 0) return '';
    
    const total = confirmDialog.payments.reduce((sum, p) => sum + p.value, 0);
    const recipients = confirmDialog.payments
      .map((p) => `• ${p.recipientName} - ${formatCurrency(p.value)}`)
      .join('\n');

    return `Tem certeza que deseja processar ${selectedPayments.size} pagamento(s)?\n\nValor Total: ${formatCurrency(total)}\n\nDestinatários:\n${recipients}`;
  }, [confirmDialog.payments, selectedPayments.size]);

  // Calcular resumo - excluindo valores cancelados dos totais
  const summary = useMemo(() => {
    if (!paymentsData?.items) {
      return {
        totalPaid: 0,
        totalPending: 0,
        countPaid: 0,
        countPending: 0,
      };
    }

    const paid = paymentsData.items.filter((p) => p.status === 'Paid');
    const pending = paymentsData.items.filter((p) => p.status === 'Pending');

    return {
      totalPaid: paid.reduce((sum, p) => sum + p.value, 0),
      totalPending: pending.reduce((sum, p) => sum + p.value, 0),
      countPaid: paid.length,
      countPending: pending.length,
    };
  }, [paymentsData?.items]);

  const payments = paymentsData?.items || [];
  const totalPages = paymentsData?.totalPages || 0;

  // Reset página se ela for maior que o total de páginas
  useEffect(() => {
    if (totalPages > 0 && page > totalPages) {
      setPage(1);
    }
  }, [totalPages, page]);

  // Loading state
  if (isLoading && !paymentsData) {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <Loading />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">Pagamentos</h1>
        <p className="text-sm text-gray-600 mt-1">
          Gerencie e processe pagamentos de comissões
        </p>
      </div>

      {/* Cards de Resumo */}
      <div className="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
        <Card className="bg-green-50 border-green-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-green-800">Total Pago</p>
              <p className="text-2xl font-bold text-green-600 mt-1">
                {formatCurrency(summary.totalPaid)}
              </p>
              <p className="text-xs text-green-700 mt-1">
                {summary.countPaid} pagamento(s)
              </p>
            </div>
            <DollarSign className="w-10 h-10 text-green-600 opacity-50" />
          </div>
        </Card>

        <Card className="bg-yellow-50 border-yellow-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-yellow-800">
                Total Pendente
              </p>
              <p className="text-2xl font-bold text-yellow-600 mt-1">
                {formatCurrency(summary.totalPending)}
              </p>
              <p className="text-xs text-yellow-700 mt-1">
                {summary.countPending} pagamento(s)
              </p>
            </div>
            <DollarSign className="w-10 h-10 text-yellow-600 opacity-50" />
          </div>
        </Card>

        <Card className="bg-red-50 border-red-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-red-800">
                Negócios Cancelados
              </p>
              <p className="text-2xl font-bold text-red-600 mt-1">
                {formatCurrency(cancelledSummary?.totalCancelledValue || 0)}
              </p>
              <p className="text-xs text-red-700 mt-1">
                {cancelledSummary?.totalCancelledBusinesses || 0} negócio(s)
              </p>
            </div>
            <XCircle className="w-10 h-10 text-red-600 opacity-50" />
          </div>
        </Card>

        <Card className="bg-blue-50 border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-blue-800">
                Total Geral
              </p>
              <p className="text-2xl font-bold text-blue-600 mt-1">
                {formatCurrency(summary.totalPaid + summary.totalPending)}
              </p>
              <p className="text-xs text-blue-700 mt-1">
                {summary.countPaid + summary.countPending} pagamento(s)
              </p>
            </div>
            <DollarSign className="w-10 h-10 text-blue-600 opacity-50" />
          </div>
        </Card>

        <Card className="bg-gray-50 border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-800">
                Selecionados
              </p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                {selectedPayments.size}
              </p>
              <p className="text-xs text-gray-700 mt-1">
                {formatCurrency(
                  payments
                    .filter((p) => selectedPayments.has(p.id))
                    .reduce((sum, p) => sum + p.value, 0)
                )}
              </p>
            </div>
            <CheckSquare className="w-10 h-10 text-gray-600 opacity-50" />
          </div>
        </Card>
      </div>

      {/* Filtros */}
      <Card className="mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
          {/* Status */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Status
            </label>
            <select
              value={statusFilter}
              onChange={(e) => {
                setStatusFilter(e.target.value);
                handleFilterChange();
              }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black"
            >
              <option value="">Todos</option>
              <option value="Pending">Pendente</option>
              <option value="Paid">Pago</option>
            </select>
          </div>

          {/* Nível */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Nível
            </label>
            <select
              value={levelFilter}
              onChange={(e) => {
                setLevelFilter(e.target.value);
                handleFilterChange();
              }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black"
            >
              <option value="">Todos</option>
              <option value="1">Nível 1</option>
              <option value="2">Nível 2</option>
              <option value="3">Nível 3</option>
            </select>
          </div>

          {/* Vetor (apenas AdminGlobal) */}
          {isAdminGlobal && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Vetor
              </label>
              <select
                value={vectorFilter}
                onChange={(e) => {
                  setVectorFilter(e.target.value);
                  handleFilterChange();
                }}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black"
              >
                <option value="">Todos</option>
                {vectorsData?.items?.map((vector) => (
                  <option key={vector.id} value={vector.id}>
                    {vector.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Data Início */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Data Início
            </label>
            <Input
              type="date"
              value={dateStart}
              onChange={(e) => {
                setDateStart(e.target.value);
                handleFilterChange();
              }}
            />
          </div>

          {/* Data Fim */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Data Fim
            </label>
            <Input
              type="date"
              value={dateEnd}
              onChange={(e) => {
                setDateEnd(e.target.value);
                handleFilterChange();
              }}
            />
          </div>
        </div>

        {/* Botão Limpar Filtros */}
        <div className="flex justify-end">
          <Button variant="secondary" onClick={handleClearFilters}>
            Limpar Filtros
          </Button>
        </div>
      </Card>

      {/* Botão Pagar Selecionados */}
      {selectedPayments.size > 0 && (
        <div className="mb-4">
          <Button
            variant="primary"
            onClick={handleProcessPayments}
            disabled={processPaymentsMutation.isPending}
          >
            <DollarSign className="w-4 h-4 mr-2" />
            Pagar Selecionados ({selectedPayments.size})
          </Button>
        </div>
      )}

      {/* Tabela de Pagamentos */}
      <Card>
        {error ? (
          <Alert type="error">
            {(error as any)?.response?.data?.message ||
              'Erro ao carregar pagamentos'}
          </Alert>
        ) : payments.length === 0 && totalPages === 0 ? (
          <Alert type="info">
            Nenhum pagamento encontrado com os filtros aplicados.
          </Alert>
        ) : payments.length === 0 && totalPages > 0 ? (
          <Alert type="info">
            Página vazia. Use a paginação abaixo para navegar.
          </Alert>
        ) : (
          <>
            <Table
              data={payments}
              columns={[
                {
                  key: 'select',
                  header: '',
                  render: (payment: Payment) =>
                    payment.status === 'Pending' ? (
                      <button
                        onClick={() => handleSelectPayment(payment.id)}
                        className="flex items-center justify-center"
                      >
                        {selectedPayments.has(payment.id) ? (
                          <CheckSquare className="w-5 h-5 text-gray-900" />
                        ) : (
                          <Square className="w-5 h-5 text-gray-400" />
                        )}
                      </button>
                    ) : (
                      <Square className="w-5 h-5 text-gray-200" />
                    ),
                },
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
                  header: 'Destinatário',
                  render: (payment: Payment) => (
                    <div>
                      <p className="font-medium text-gray-900">
                        {payment.recipientName}
                      </p>
                      <p className="text-sm text-gray-600">
                        {payment.recipientType}
                      </p>
                    </div>
                  ),
                },
                {
                  key: 'businessId',
                  header: 'Negócio',
                  render: (payment: Payment) => (
                    <span className="text-sm font-mono text-gray-700">
                      #{payment.businessId ? payment.businessId.substring(0, 8) : 'N/A'}
                    </span>
                  ),
                },
                {
                  key: 'level',
                  header: 'Nível',
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
                      Nível {payment.level}
                    </Badge>
                  ),
                },
                {
                  key: 'value',
                  header: 'Valor',
                  render: (payment: Payment) => (
                    <span
                      className={`font-semibold ${
                        payment.status === 'Paid'
                          ? 'text-green-600'
                          : payment.status === 'Pending'
                          ? 'text-yellow-600'
                          : 'text-gray-500'
                      }`}
                    >
                      {formatCurrency(payment.value)}
                    </span>
                  ),
                },
                {
                  key: 'status',
                  header: 'Status',
                  render: (payment: Payment) => (
                    <Badge
                      variant={
                        payment.status === 'Paid'
                          ? 'success'
                          : payment.status === 'Pending'
                          ? 'warning'
                          : 'error'
                      }
                    >
                      {payment.status === 'Paid'
                        ? 'Pago'
                        : payment.status === 'Pending'
                        ? 'Pendente'
                        : 'Cancelado'}
                    </Badge>
                  ),
                },
                {
                  key: 'paidAt',
                  header: 'Data Pagamento',
                  render: (payment: Payment) => (
                    <span className="text-gray-900">
                      {payment.paidAt ? formatDate(payment.paidAt) : '-'}
                    </span>
                  ),
                },
              ]}
            />
          </>
        )}

        {/* Paginação - sempre exibir se há mais de uma página */}
        {totalPages > 1 && (
          <div className="mt-6">
            <Pagination
              currentPage={page}
              totalPages={totalPages}
              onPageChange={(newPage) => {
                // Validar se a nova página está dentro do range válido
                if (newPage >= 1 && newPage <= totalPages) {
                  setPage(newPage);
                }
              }}
            />
          </div>
        )}
      </Card>

      {/* ConfirmDialog */}
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={() => setConfirmDialog({ isOpen: false, payments: [] })}
        onConfirm={handleConfirmProcess}
        title="Confirmar Pagamento"
        message={confirmMessage}
        confirmText="Sim, processar"
        cancelText="Cancelar"
        variant="info"
        isLoading={processPaymentsMutation.isPending}
      />
    </div>
  );
};

export default PaymentsListPage;
