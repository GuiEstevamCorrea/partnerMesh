import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { ArrowLeft, Edit2, XCircle, DollarSign, Calendar, User, FileText, TrendingUp } from 'lucide-react';
import { businessApi } from '@/api/endpoints/business.api';
import { Business } from '@/types/business.types';
import { Payment } from '@/types/payment.types';
import { Card } from '@/components/common/Card';
import { Button } from '@/components/common/Button';
import { Badge } from '@/components/common/Badge';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { Table } from '@/components/common/Table';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useToast } from '@/components/common/Toast';
import { useAuthStore } from '@/store/auth.store';
import { Permission } from '@/types/auth.types';
import { formatCurrency, formatDate } from '@/utils/formatters';

const BusinessDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const { user } = useAuthStore();

  const [confirmDialog, setConfirmDialog] = useState({
    isOpen: false,
    business: null as Business | null,
  });

  // Permissões
  const canManage =
    user?.permission === Permission.AdminGlobal ||
    user?.permission === Permission.AdminVetor;

  // Query: Carregar negócio
  const {
    data: business,
    isLoading: isLoadingBusiness,
    error: errorBusiness,
  } = useQuery({
    queryKey: ['business', id],
    queryFn: () => businessApi.getById(id!),
    enabled: !!id,
  });

  // Query: Carregar pagamentos do negócio
  const {
    data: payments,
    isLoading: isLoadingPayments,
    error: errorPayments,
  } = useQuery({
    queryKey: ['business-payments', id],
    queryFn: () => businessApi.getPayments(id!),
    enabled: !!id,
  });

  // Mutation: Cancelar negócio
  const cancelMutation = useMutation({
    mutationFn: (businessId: string) => businessApi.cancel(businessId),
    onSuccess: () => {
      showToast('success', 'Negócio cancelado com sucesso');
      queryClient.invalidateQueries({ queryKey: ['business', id] });
      queryClient.invalidateQueries({ queryKey: ['business-payments', id] });
      queryClient.invalidateQueries({ queryKey: ['businesses'] });
      setConfirmDialog({ isOpen: false, business: null });
    },
    onError: (error: any) => {
      showToast(
        'error',
        error.response?.data?.message || 'Erro ao cancelar negócio'
      );
    },
  });

  // Handlers
  const handleCancelClick = (business: Business) => {
    setConfirmDialog({ isOpen: true, business });
  };

  const handleConfirmCancel = () => {
    if (confirmDialog.business) {
      cancelMutation.mutate(confirmDialog.business.id);
    }
  };

  const handleEdit = () => {
    navigate(`/negocios/${id}/editar`);
  };

  const handleBack = () => {
    navigate('/negocios');
  };

  // Calcular totais
  const totalPaid = payments
    ?.filter((p) => p.status === 'Paid')
    .reduce((sum, p) => sum + p.value, 0) || 0;

  const totalPending = payments
    ?.filter((p) => p.status === 'Pending')
    .reduce((sum, p) => sum + p.value, 0) || 0;

  // Loading state
  if (isLoadingBusiness || isLoadingPayments) {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <Loading />
      </div>
    );
  }

  // Error state
  if (errorBusiness || errorPayments) {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <Alert type="error">
          {(errorBusiness as any)?.response?.data?.message ||
            (errorPayments as any)?.response?.data?.message ||
            'Erro ao carregar detalhes do negócio'}
        </Alert>
        <div className="mt-4">
          <Button variant="secondary" onClick={handleBack}>
            <ArrowLeft className="w-4 h-4 mr-2" />
            Voltar
          </Button>
        </div>
      </div>
    );
  }

  // Not found
  if (!business) {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <Alert type="error">Negócio não encontrado</Alert>
        <div className="mt-4">
          <Button variant="secondary" onClick={handleBack}>
            <ArrowLeft className="w-4 h-4 mr-2" />
            Voltar
          </Button>
        </div>
      </div>
    );
  }

  const isActive = business.status === 'Active';
  const isCancelled = business.status === 'Cancelled';

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      {/* Header */}
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">
            Detalhes do Negócio
          </h1>
          <p className="text-sm text-gray-600 mt-1">
            #{business.id.substring(0, 8)}
          </p>
        </div>
        <div className="flex gap-3">
          <Button variant="secondary" onClick={handleBack}>
            <ArrowLeft className="w-4 h-4 mr-2" />
            Voltar
          </Button>
          {canManage && isActive && (
            <>
              <Button variant="primary" onClick={handleEdit}>
                <Edit2 className="w-4 h-4 mr-2" />
                Editar
              </Button>
              <Button
                variant="danger"
                onClick={() => handleCancelClick(business)}
              >
                <XCircle className="w-4 h-4 mr-2" />
                Cancelar
              </Button>
            </>
          )}
        </div>
      </div>

      {/* Alert se cancelado */}
      {isCancelled && (
        <div className="mb-6">
          <Alert type="warning">
            Este negócio foi cancelado. Pagamentos pendentes foram cancelados automaticamente.
          </Alert>
        </div>
      )}

      {/* Seção 1: Dados do Negócio */}
      <Card className="mb-6">
        <div className="border-b border-gray-200 pb-4 mb-4">
          <h2 className="text-xl font-semibold text-gray-900 flex items-center">
            <FileText className="w-5 h-5 mr-2 text-gray-700" />
            Dados do Negócio
          </h2>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* ID */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              ID do Negócio
            </label>
            <p className="text-base text-gray-900 font-mono">
              {business.id}
            </p>
          </div>

          {/* Status */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Status
            </label>
            <div>
              <Badge variant={isActive ? 'success' : 'error'}>
                {isActive ? 'Ativo' : 'Cancelado'}
              </Badge>
            </div>
          </div>

          {/* Parceiro */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1 flex items-center">
              <User className="w-4 h-4 mr-1" />
              Parceiro
            </label>
            <p className="text-base text-gray-900">
              {business.partnerName}
            </p>
          </div>

          {/* Tipo de Negócio */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Tipo de Negócio
            </label>
            <p className="text-base text-gray-900">
              {business.businessTypeName}
            </p>
          </div>

          {/* Valor */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1 flex items-center">
              <DollarSign className="w-4 h-4 mr-1" />
              Valor
            </label>
            <p
              className={`text-base font-semibold ${
                isActive ? 'text-green-600' : 'text-gray-500'
              }`}
            >
              {formatCurrency(business.value)}
            </p>
          </div>

          {/* Comissão Total */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1 flex items-center">
              <TrendingUp className="w-4 h-4 mr-1" />
              Comissão Total (10%)
            </label>
            <p
              className={`text-base font-semibold ${
                isActive ? 'text-blue-600' : 'text-gray-500'
              }`}
            >
              {formatCurrency(business.commission?.totalValue || 0)}
            </p>
          </div>

          {/* Data do Negócio */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1 flex items-center">
              <Calendar className="w-4 h-4 mr-1" />
              Data do Negócio
            </label>
            <p className="text-base text-gray-900">
              {formatDate(business.date)}
            </p>
          </div>

          {/* Data de Criação */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Criado em
            </label>
            <p className="text-base text-gray-900">
              {formatDate(business.createdAt)}
            </p>
          </div>

          {/* Observações */}
          {business.observations && (
            <div className="col-span-full">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Observações
              </label>
              <p className="text-base text-gray-900 whitespace-pre-wrap">
                {business.observations}
              </p>
            </div>
          )}
        </div>
      </Card>

      {/* Seção 2: Comissões Geradas */}
      <Card>
        <div className="border-b border-gray-200 pb-4 mb-4">
          <h2 className="text-xl font-semibold text-gray-900 flex items-center justify-between">
            <span className="flex items-center">
              <DollarSign className="w-5 h-5 mr-2 text-gray-700" />
              Comissões Geradas
            </span>
            <span className="text-sm font-normal text-gray-600">
              {payments?.length || 0} pagamento(s)
            </span>
          </h2>
        </div>

        {/* Resumo Financeiro */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
          <div className="bg-green-50 border border-green-200 rounded-lg p-4">
            <p className="text-sm font-medium text-green-800 mb-1">
              Total Pago
            </p>
            <p className="text-2xl font-bold text-green-600">
              {formatCurrency(totalPaid)}
            </p>
          </div>
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
            <p className="text-sm font-medium text-yellow-800 mb-1">
              Total Pendente
            </p>
            <p className="text-2xl font-bold text-yellow-600">
              {formatCurrency(totalPending)}
            </p>
          </div>
        </div>

        {/* Tabela de Pagamentos */}
        {payments && payments.length > 0 ? (
          <Table
            data={payments}
            columns={[
              {
                key: 'recipient',
                header: 'Destinatário',
                render: (payment: Payment) => (
                  <div>
                    <p className="font-medium text-gray-900">
                      {payment.partnerName}
                    </p>
                    <p className="text-sm text-gray-600">
                      {payment.tipoPagamento}
                    </p>
                  </div>
                ),
              },
              {
                key: 'type',
                header: 'Tipo',
                render: (payment: Payment) => (
                  <span className="text-sm text-gray-700">
                    {payment.tipoPagamento}
                  </span>
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
                key: 'paidOn',
                header: 'Data Pagamento',
                render: (payment: Payment) => (
                  <span className="text-gray-900">
                    {payment.paidOn ? formatDate(payment.paidOn) : '-'}
                  </span>
                ),
              },
            ]}
          />
        ) : (
          <Alert type="info">
            Nenhum pagamento gerado para este negócio.
          </Alert>
        )}
      </Card>

      {/* ConfirmDialog para cancelar */}
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={() => setConfirmDialog({ isOpen: false, business: null })}
        onConfirm={handleConfirmCancel}
        title="Cancelar Negócio"
        message={`Tem certeza que deseja cancelar o negócio #${confirmDialog.business?.id.substring(
          0,
          8
        )}? Esta ação cancelará todos os pagamentos pendentes. Pagamentos já realizados não serão afetados.`}
        confirmText="Sim, cancelar"
        cancelText="Não, manter"
        variant="danger"
        isLoading={cancelMutation.isPending}
      />
    </div>
  );
};

export default BusinessDetailPage;
