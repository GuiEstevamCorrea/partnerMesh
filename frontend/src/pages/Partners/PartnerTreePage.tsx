import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { ArrowLeft, Search, Network } from 'lucide-react';

import { partnersApi } from '@/api/endpoints/partners.api';
import { Partner } from '@/types/partner.types';
import { useAuthStore } from '@/store/auth.store';

import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Card } from '@/components/common/Card';
import { Loading } from '@/components/common/Loading';
import { Alert } from '@/components/common/Alert';
import { PartnerTreeView } from '@/components/common/PartnerTreeView';

export function PartnerTreePage() {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const { user } = useAuthStore();

  // Estado dos filtros
  const [search, setSearch] = useState('');
  const selectedPartnerId = searchParams.get('partnerId');

  // Query: Lista de parceiros para o filtro
  const { data: partnersData, isLoading: isLoadingPartners } = useQuery({
    queryKey: ['partners', 'all'],
    queryFn: () => partnersApi.list({ pageSize: 1000 }),
  });

  // Query: Árvore de parceiros
  const {
    data: treeData,
    isLoading: isLoadingTree,
    error: treeError,
  } = useQuery({
    queryKey: ['partner-tree', selectedPartnerId || user?.vectorId],
    queryFn: () => {
      if (selectedPartnerId) {
        return partnersApi.getTree(selectedPartnerId);
      }
      // Se não houver parceiro selecionado, busca a árvore do vetor
      // O backend deve retornar a árvore completa do vetor
      return partnersApi.getTree(user!.vectorId as string);
    },
    enabled: !!user?.vectorId,
  });

  const partners = partnersData?.items || [];
  
  // Filtrar parceiros para o select
  const filteredPartners = partners.filter((p: Partner) =>
    p.name.toLowerCase().includes(search.toLowerCase())
  );

  const handlePartnerSelect = (partnerId: string | null) => {
    if (partnerId) {
      setSearchParams({ partnerId });
    } else {
      setSearchParams({});
    }
  };

  const handleClearFilter = () => {
    setSearchParams({});
    setSearch('');
  };

  const isLoading = isLoadingPartners || isLoadingTree;

  // Informações do parceiro selecionado
  const selectedPartner = selectedPartnerId
    ? partners.find((p: Partner) => p.id === selectedPartnerId)
    : null;

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button
            variant="outline"
            onClick={() => navigate('/parceiros')}
            className="flex items-center gap-2"
          >
            <ArrowLeft className="w-4 h-4" />
            Voltar
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
              <Network className="w-6 h-6" />
              Árvore de Parceiros
            </h1>
            <p className="text-gray-600 mt-1">
              Visualização hierárquica da rede de parceiros
            </p>
          </div>
        </div>
      </div>

      {/* Filtros */}
      <Card>
        <div className="space-y-4">
          <div className="flex items-center gap-3">
            <Search className="w-5 h-5 text-gray-400" />
            <h2 className="text-lg font-semibold text-gray-900">Filtrar por Parceiro</h2>
          </div>

          <Alert type="info">
            <p>
              {selectedPartnerId
                ? 'Exibindo sub-árvore do parceiro selecionado. Limpe o filtro para ver a árvore completa do vetor.'
                : 'Exibindo árvore completa do vetor. Selecione um parceiro para ver apenas sua sub-árvore.'}
            </p>
          </Alert>

          <div className="flex gap-3">
            <div className="flex-1">
              <Input
                type="text"
                placeholder="Buscar parceiro..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                disabled={isLoadingPartners}
              />
            </div>
          </div>

          {search && filteredPartners.length > 0 && (
            <div className="border rounded-lg max-h-60 overflow-y-auto">
              {filteredPartners.map((partner: Partner) => (
                <button
                  key={partner.id}
                  onClick={() => {
                    handlePartnerSelect(partner.id);
                    setSearch('');
                  }}
                  className="w-full text-left px-4 py-3 hover:bg-gray-50 border-b last:border-b-0 transition-colors"
                >
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="font-medium text-gray-900">{partner.name}</p>
                      <p className="text-sm text-gray-600">{partner.contact}</p>
                    </div>
                    <div className="text-sm text-gray-500">
                      Nível {partner.level}
                    </div>
                  </div>
                </button>
              ))}
            </div>
          )}

          {selectedPartner && (
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-blue-600 font-medium">Parceiro Selecionado:</p>
                  <p className="font-semibold text-gray-900">{selectedPartner.name}</p>
                  <p className="text-sm text-gray-600">
                    Nível {selectedPartner.level} • {selectedPartner.totalRecommended} recomendado{selectedPartner.totalRecommended !== 1 ? 's' : ''}
                  </p>
                </div>
                <Button variant="outline" onClick={handleClearFilter}>
                  Limpar Filtro
                </Button>
              </div>
            </div>
          )}
        </div>
      </Card>

      {/* Árvore */}
      <Card>
        {isLoading ? (
          <Loading />
        ) : treeError ? (
          <Alert type="error">
            <p>Erro ao carregar árvore de parceiros. Tente novamente.</p>
          </Alert>
        ) : !treeData ? (
          <Alert type="info">
            <p>Nenhum parceiro encontrado na rede.</p>
          </Alert>
        ) : (
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold text-gray-900">
                {selectedPartner ? `Sub-árvore de ${selectedPartner.name}` : 'Árvore Completa'}
              </h2>
              <div className="text-sm text-gray-600">
                Total de parceiros: {treeData.totalRecommended || 0}
              </div>
            </div>

            <Alert type="info">
              <p className="text-sm">
                <strong>Dica:</strong> Clique nas setas para expandir ou colapsar os níveis da árvore.
                Os dois primeiros níveis já estão expandidos automaticamente.
              </p>
            </Alert>

            <div className="space-y-2">
              <PartnerTreeView node={treeData} />
            </div>
          </div>
        )}
      </Card>

      {/* Legenda */}
      <Card>
        <div className="space-y-3">
          <h3 className="font-semibold text-gray-900">Legenda</h3>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 rounded bg-blue-100 border border-blue-200"></div>
              <span className="text-sm text-gray-700">Nível 1</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 rounded bg-green-100 border border-green-200"></div>
              <span className="text-sm text-gray-700">Nível 2</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 rounded bg-purple-100 border border-purple-200"></div>
              <span className="text-sm text-gray-700">Nível 3</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 rounded bg-gray-100 border border-gray-200"></div>
              <span className="text-sm text-gray-700">Nível 3+</span>
            </div>
          </div>
        </div>
      </Card>
    </div>
  );
}
