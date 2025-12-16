import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { User, Permission } from '@/types';

interface AuthState {
  token: string | null;
  refreshToken: string | null;
  user: User | null;
  isAuthenticated: boolean;
  
  setAuth: (token: string, refreshToken: string, user: User) => void;
  setTokens: (token: string, refreshToken: string) => void;
  setUser: (user: User) => void;
  logout: () => void;
  hasPermission: (requiredPermissions: Permission[]) => boolean;
  hasVectorAccess: (vectorId: string) => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      refreshToken: null,
      user: null,
      isAuthenticated: false,

      setAuth: (token, refreshToken, user) => {
        set({ token, refreshToken, user, isAuthenticated: true });
      },

      setTokens: (token, refreshToken) => {
        set({ token, refreshToken, isAuthenticated: true });
      },

      setUser: (user) => {
        set({ user });
      },

      logout: () => {
        set({
          token: null,
          refreshToken: null,
          user: null,
          isAuthenticated: false,
        });
      },

      hasPermission: (requiredPermissions) => {
        const { user } = get();
        if (!user) return false;
        
        // Admin Global tem todas as permissões
        if (user.permission === Permission.AdminGlobal) return true;
        
        return requiredPermissions.includes(user.permission);
      },

      hasVectorAccess: (vectorId) => {
        const { user } = get();
        if (!user) return false;
        
        // Admin Global tem acesso a todos os vetores
        if (user.permission === Permission.AdminGlobal) return true;
        
        // Outros usuários só têm acesso ao seu próprio vetor
        return user.vectorId === vectorId;
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        token: state.token,
        refreshToken: state.refreshToken,
        user: state.user,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
);
