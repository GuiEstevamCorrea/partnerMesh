import React from 'react';

export const Footer: React.FC = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-white border-t-2 border-gray-200 mt-auto">
      <div className="container mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-600">
            © {currentYear} Sistema de Rede de Credenciamento. Todos os direitos reservados.
          </p>
          <p className="text-sm text-gray-500">
            Versão 1.0.0
          </p>
        </div>
      </div>
    </footer>
  );
};
