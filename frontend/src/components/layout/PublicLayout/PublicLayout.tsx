import { Outlet } from 'react-router-dom';

export const PublicLayout = () => {
  const currentYear = new Date().getFullYear();

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      {/* Header Público */}
      <header className="bg-black text-white py-6 shadow-lg">
        <div className="container mx-auto px-6 text-center">
          <h1 className="text-3xl font-bold">Sistema de Rede</h1>
          <p className="text-gray-300 text-sm mt-1">Gestão de Credenciamento e Vetores</p>
        </div>
      </header>

      {/* Conteúdo Principal */}
      <main className="flex-1 flex items-center justify-center px-4 py-8">
        <Outlet />
      </main>

      {/* Footer Público */}
      <footer className="bg-white border-t border-gray-200 py-4">
        <div className="container mx-auto px-6 text-center">
          <p className="text-sm text-gray-600">
            © {currentYear} Sistema de Rede de Credenciamento. Todos os direitos reservados.
          </p>
          <p className="text-xs text-gray-500 mt-1">Versão 1.0.0</p>
        </div>
      </footer>
    </div>
  );
};
