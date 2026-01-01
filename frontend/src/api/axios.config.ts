import axios, { AxiosError } from 'axios';
import { useAuthStore } from '@/store/auth.store';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});

// Interceptor para adicionar token
api.interceptors.request.use(
  (config) => {
    console.log('🔵 Interceptor - URL:', config.url, 'Method:', config.method);
    const token = useAuthStore.getState().token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
      console.log('🔵 Token adicionado ao header');
    } else {
      console.log('⚠️ Nenhum token encontrado!');
    }
    console.log('🔵 Request config:', config);
    return config;
  },
  (error) => {
    console.error('❌ Erro no interceptor request:', error);
    return Promise.reject(error);
  }
);

// Interceptor para tratar erros e refresh token
api.interceptors.response.use(
  (response) => {
    console.log('✅ Response recebida:', response.status, response.config.url);
    return response;
  },
  async (error: AxiosError) => {
    console.error('❌ Erro na resposta:', error.response?.status, error.message);
    console.error('❌ Response data:', error.response?.data);
    const originalRequest = error.config as any;

    // Token expirado
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = useAuthStore.getState().refreshToken;
        if (refreshToken) {
          const response = await axios.post(
            `${import.meta.env.VITE_API_BASE_URL}/auth/refresh`,
            { refreshToken }
          );

          const { token, refreshToken: newRefreshToken, user } = response.data;
          useAuthStore.getState().setAuth(token, newRefreshToken, user);

          originalRequest.headers.Authorization = `Bearer ${token}`;
          return api(originalRequest);
        }
      } catch (refreshError) {
        useAuthStore.getState().logout();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api;
