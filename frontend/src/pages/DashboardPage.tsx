import { useAuthStore } from '@/store/auth.store';
import { Card } from '@/components';

export const DashboardPage = () => {
  const user = useAuthStore((state) => state.user);

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">
          Bem-vindo, {user?.name}!
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card>
          <div className="text-center">
            <p className="text-gray-600 text-sm">Usuários Ativos</p>
            <p className="text-3xl font-bold text-black mt-2">--</p>
          </div>
        </Card>

        <Card>
          <div className="text-center">
            <p className="text-gray-600 text-sm">Vetores Ativos</p>
            <p className="text-3xl font-bold text-black mt-2">--</p>
          </div>
        </Card>

        <Card>
          <div className="text-center">
            <p className="text-gray-600 text-sm">Parceiros</p>
            <p className="text-3xl font-bold text-black mt-2">--</p>
          </div>
        </Card>

        <Card>
          <div className="text-center">
            <p className="text-gray-600 text-sm">Negócios (Mês)</p>
            <p className="text-3xl font-bold text-black mt-2">--</p>
          </div>
        </Card>
      </div>

      <div className="mt-8">
        <Card title="Em Desenvolvimento">
          <p className="text-gray-600">
            Dashboard com estatísticas será implementado no Entregável 08.
          </p>
        </Card>
      </div>
    </div>
  );
};
