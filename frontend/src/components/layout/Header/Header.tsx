import React from 'react';
import { LogOut, User } from 'lucide-react';
import { useAuthStore } from '@/store/auth.store';
import { Button } from '@/components/common/Button';
import { authApi } from '@/api/endpoints/auth.api';
import { LanguageSelector } from '@/components/common/LanguageSelector';
import { useI18n } from '@/hooks/useI18n';

export const Header: React.FC = () => {
  const user = useAuthStore((state) => state.user);
  const logout = useAuthStore((state) => state.logout);
  const { t } = useI18n();

  const handleLogout = async () => {
    try {
      await authApi.logout();
    } finally {
      logout();
      window.location.href = '/login';
    }
  };

  return (
    <header className="bg-black text-white border-b-2 border-gray-800">
      <div className="container mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <h1 className="text-2xl font-bold">Sistema de Rede</h1>
          </div>
          
          <div className="flex items-center space-x-4">
            <LanguageSelector className="text-white" />
            
            <div className="flex items-center space-x-2 text-sm">
              <User className="h-4 w-4" />
              <span>{user?.name}</span>
              <span className="text-gray-400">|</span>
              <span className="text-gray-300">{user?.permission}</span>
            </div>
            <Button
              variant="ghost"
              size="sm"
              icon={<LogOut className="h-4 w-4" />}
              onClick={handleLogout}
              className="text-white hover:bg-gray-800"
            >
              {t('navigation.logout')}
            </Button>
          </div>
        </div>
      </div>
    </header>
  );
};
