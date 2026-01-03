import { Link } from 'react-router-dom';
import { BarChart3, Users, DollarSign, TrendingUp } from 'lucide-react';
import { Card } from '@/components/common/Card';

export const ReportsIndexPage = () => {
  const reports = [
    {
      title: 'Relatório de Parceiros',
      description: 'Visualize estatísticas e dados dos parceiros cadastrados no sistema.',
      icon: Users,
      path: '/relatorios/parceiros',
      color: 'bg-blue-500',
    },
    {
      title: 'Relatório Financeiro', 
      description: 'Acompanhe comissões, pagamentos e indicadores financeiros.',
      icon: DollarSign,
      path: '/relatorios/financeiro',
      color: 'bg-green-500',
    },
    {
      title: 'Relatório de Negócios',
      description: 'Análise detalhada dos negócios realizados e seus resultados.',
      icon: TrendingUp,
      path: '/relatorios/negocios',
      color: 'bg-purple-500',
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-3">
        <BarChart3 className="w-8 h-8 text-blue-600" />
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Relatórios</h1>
          <p className="text-gray-600 mt-1">
            Acesse relatórios detalhados e indicadores do sistema
          </p>
        </div>
      </div>

      {/* Cards de Relatórios */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {reports.map((report) => {
          const Icon = report.icon;
          
          return (
            <Link key={report.path} to={report.path}>
              <Card className="hover:shadow-lg transition-shadow duration-200 cursor-pointer h-full">
                <div className="flex items-center gap-4 mb-4">
                  <div className={`p-3 rounded-lg ${report.color}`}>
                    <Icon className="w-6 h-6 text-white" />
                  </div>
                  <h2 className="text-xl font-semibold text-gray-900">
                    {report.title}
                  </h2>
                </div>
                
                <p className="text-gray-600 leading-relaxed">
                  {report.description}
                </p>

                <div className="mt-6 flex items-center text-blue-600 font-medium">
                  Acessar relatório
                  <svg 
                    className="w-4 h-4 ml-2" 
                    fill="none" 
                    stroke="currentColor" 
                    viewBox="0 0 24 24"
                  >
                    <path 
                      strokeLinecap="round" 
                      strokeLinejoin="round" 
                      strokeWidth={2} 
                      d="M9 5l7 7-7 7" 
                    />
                  </svg>
                </div>
              </Card>
            </Link>
          );
        })}
      </div>

      {/* Informações Adicionais */}
      <Card className="bg-blue-50 border-blue-200">
        <div className="flex items-start gap-4">
          <div className="p-2 bg-blue-100 rounded-lg">
            <BarChart3 className="w-5 h-5 text-blue-600" />
          </div>
          <div>
            <h3 className="font-semibold text-blue-900 mb-2">
              Dicas para usar os relatórios
            </h3>
            <ul className="text-blue-800 space-y-1 text-sm">
              <li>• Use os filtros para personalizar os dados apresentados</li>
              <li>• Todos os relatórios podem ser exportados para análise externa</li>
              <li>• Os dados são atualizados em tempo real</li>
              <li>• Acesse o histórico através dos filtros de data</li>
            </ul>
          </div>
        </div>
      </Card>
    </div>
  );
};