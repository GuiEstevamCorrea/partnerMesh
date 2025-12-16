import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Search, Plus, GitBranch, Edit2, Power } from 'lucide-react';

import { partnersApi } from '@/api/endpoints/partners.api';
import { Partner } from '@/types/partner.types';
import { useToast } from '@/components/common/Toast';
import { useAuthStore } from '@/store/auth.store';
import { Permission } from '@/types/auth.types';

import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Card } from '@/components/common/Card';
import { Badge } from '@/components/common/Badge';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { Pagination } from '@/components/common/Pagination';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { Table } from '@/components/common/Table';

export function PartnersListPage() {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const queryClient = useQueryClient();
  const { user } = useAuthStore();

  // Estado dos filtros
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');

  // Estado do ConfirmDialog
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    partnerId: string;
    partnerName: string;
    isActive: boolean;
  }>({
    isOpen: false,
    partnerId: '',
    partnerName: '',
    isActive: false,
  });

  const isAdminGlobal = user?.permission === Permission.AdminGlobal;
  const isAdminVetor = user?.permission === Permission.AdminVetor;

  // Query para listar parceiros
  const { data, isLoading, error } = useQuery({
    queryKey: ['partners', page, search, statusFilter],
    queryFn: () => {
      const params: any = {
        page,
        pageSize: 20,
      };

      if (search) {
        params.search = search;
      }

      if (statusFilter === 'active') {
        params.isActive = true;
      } else if (statusFilter === 'inactive') {
        params.isActive = false;
      }

      return partnersApi.list(params);
    },
  });

  // Mutation para ativar/inativar parceiro
  const toggleActiveMutation = useMutation({
    mutationFn: partnersApi.toggleActive,
    onSuccess: (_, partnerId) => {
      const partner = data?.items.find((p) => p.id === partnerId);
      const newStatus = partner?.isActive ? 'inativo' : 'ativo';
      showToast('success', `Parceiro ${newStatus} com sucesso!`);
      queryClient.invalidateQueries({ queryKey: ['partners'] });
      setConfirmDialog({ isOpen: false, partnerId: '', partnerName: '', isActive: false });
    },
    onError: (error: any) => {
      const message = error.response?.data?.message || 'Erro ao alterar status do parceiro';
      showToast('error', message);
    },
  });

  // Handlers
  const handleSearchChange = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  const handleStatusFilterChange = (value: string) => {
    setStatusFilter(value);
    setPage(1);
  };

  const handleToggleActive = (partner: Partner) => {
    setConfirmDialog({
      isOpen: true,
      partnerId: partner.id,
      partnerName: partner.name,
      isActive: partner.isActive,
    });
  };

  const handleConfirmToggle = () => {
    toggleActiveMutation.mutate(confirmDialog.partnerId);
  };

  // Loading state
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Loading />
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <Alert type="error">
        Erro ao carregar parceiros. Tente novamente mais tarde.
      </Alert>
    );
  }

  const partners = data?.items || [];
  const totalPages = Math.ceil((data?.totalItems || 0) / 20);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Parceiros</h1>
          <p className="text-sm text-gray-500 mt-1">
            Gerencie os parceiros da rede de credenciamento
          </p>
        </div>
        <div className="flex gap-3">
          <Button variant="outline" onClick={() => navigate('/parceiros/arvore')}>
            <GitBranch className="w-4 h-4 mr-2" />
            Ver Árvore
          </Button>
          {(isAdminGlobal || isAdminVetor) && (
            <Button onClick={() => navigate('/parceiros/novo')}>
              <Plus className="w-4 h-4 mr-2" />
              Novo Parceiro
            </Button>
          )}
        </div>
      </div>

      {/* Mensagem informativa para Operador */}
      {user?.permission === Permission.Operador && (
        <Alert type="info">
          Você pode visualizar os parceiros do seu vetor e gerenciar negócios.
        </Alert>
      )}

      {/* Filtros */}
      <Card>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {/* Busca por nome/contato */}
          <div className="md:col-span-2">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
              <Input
                placeholder="Buscar por nome ou contato..."
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                className="pl-10"
              />
            </div>
          </div>

          {/* Filtro de status */}
          <div>
            <select
              value={statusFilter}
              onChange={(e) => handleStatusFilterChange(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-black focus:border-transparent"
            >
              <option value="all">Todos os Status</option>
              <option value="active">Apenas Ativos</option>
              <option value="inactive">Apenas Inativos</option>
            </select>
          </div>
        </div>
      </Card>

      {/* Tabela de Parceiros */}
      {partners.length === 0 ? (
        <Card>
          <div className="text-center py-12">
            <GitBranch className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">
              Nenhum parceiro encontrado
            </h3>
            <p className="text-sm text-gray-500 mb-6">
              {search || statusFilter !== 'all'
                ? 'Tente ajustar os filtros de busca.'
                : 'Comece criando o primeiro parceiro da rede.'}
            </p>
            {(isAdminGlobal || isAdminVetor) && !search && statusFilter === 'all' && (
              <Button onClick={() => navigate('/parceiros/novo')}>
                <Plus className="w-4 h-4 mr-2" />
                Criar Primeiro Parceiro
              </Button>
            )}
          </div>
        </Card>
      ) : (
        <>
          <Card>
            <Table<Partner>
              data={partners}
              columns={[
                {
                  key: 'name',
                  header: 'Nome',
                  render: (partner) => (
                    <div>
                      <div className="font-medium text-gray-900">{partner.name}</div>
                      <div className="text-sm text-gray-500">{partner.contact}</div>
                    </div>
                  ),
                },
                {
                  key: 'recommenderName',
                  header: 'Recomendador',
                  render: (partner) => (
                    <div className="text-sm">
                      {partner.recommenderName ? (
                        <>
                          <div className="text-gray-900">{partner.recommenderName}</div>
                          <div className="text-gray-500">
                            {partner.recommenderType === 'Vector' ? 'Vetor' : 'Parceiro'}
                          </div>
                        </>
                      ) : (
                        <span className="text-gray-400">-</span>
                      )}
                    </div>
                  ),
                },
                {
                  key: 'level',
                  header: 'Nível',
                  render: (partner) => (
                    <Badge variant="default">Nível {partner.level}</Badge>
                  ),
                },
                {
                  key: 'totalRecommended',
                  header: 'Recomendados',
                  render: (partner) => (
                    <div className="text-center">
                      <span className="text-lg font-semibold text-gray-900">
                        {partner.totalRecommended}
                      </span>
                    </div>
                  ),
                },
                {
                  key: 'isActive',
                  header: 'Status',
                  render: (partner) => (
                    <Badge variant={partner.isActive ? 'success' : 'error'}>
                      {partner.isActive ? 'Ativo' : 'Inativo'}
                    </Badge>
                  ),
                },
                {
                  key: 'createdAt',
                  header: 'Data Cadastro',
                  render: (partner) => (
                    <span className="text-sm text-gray-500">
                      {new Date(partner.createdAt).toLocaleDateString('pt-BR')}
                    </span>
                  ),
                },
                {
                  key: 'actions',
                  header: 'Ações',
                  render: (partner) => (
                    <div className="flex items-center gap-2">
                      {(isAdminGlobal || isAdminVetor) && (
                        <>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => navigate(`/parceiros/${partner.id}/editar`)}
                            title="Editar"
                          >
                            <Edit2 className="w-4 h-4" />
                          </Button>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleToggleActive(partner)}
                            title={partner.isActive ? 'Inativar' : 'Ativar'}
                          >
                            <Power className="w-4 h-4" />
                          </Button>
                        </>
                      )}
                    </div>
                  ),
                },
              ]}
            />
          </Card>

          {/* Paginação */}
          {totalPages > 1 && (
            <div className="flex justify-center">
              <Pagination
                currentPage={page}
                totalPages={totalPages}
                onPageChange={setPage}
              />
            </div>
          )}
        </>
      )}

      {/* ConfirmDialog para ativar/inativar */}
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={() =>
          setConfirmDialog({ isOpen: false, partnerId: '', partnerName: '', isActive: false })
        }
        onConfirm={handleConfirmToggle}
        title={`${confirmDialog.isActive ? 'Inativar' : 'Ativar'} Parceiro`}
        message={
          confirmDialog.isActive
            ? `Tem certeza que deseja inativar o parceiro "${confirmDialog.partnerName}"? Parceiros inativos não poderão ter novos negócios cadastrados.`
            : `Tem certeza que deseja ativar o parceiro "${confirmDialog.partnerName}"?`
        }
        confirmText={confirmDialog.isActive ? 'Inativar' : 'Ativar'}
        variant={confirmDialog.isActive ? 'warning' : 'info'}
        isLoading={toggleActiveMutation.isPending}
      />
    </div>
  );
}
