import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Input, Button, Alert, Card } from '@/components';
import { authApi } from '@/api/endpoints/auth.api';
import { useAuthStore } from '@/store/auth.store';

const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'Email é obrigatório')
    .email('Email inválido'),
  password: z
    .string()
    .min(6, 'Senha deve ter no mínimo 6 caracteres'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export const LoginPage = () => {
  const navigate = useNavigate();
  const setAuth = useAuthStore((state) => state.setAuth);
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    setError('');
    setIsLoading(true);

    try {
      const response = await authApi.login({ email: data.email, password: data.password });
      
      setAuth(
        response.token,
        response.refreshToken,
        response.user
      );

      navigate('/dashboard');
    } catch (err: any) {
      const errorMessage = 
        err.response?.data?.message || 
        err.response?.data?.error ||
        'Erro ao fazer login. Verifique suas credenciais.';
      
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="w-full max-w-md">
      {/* Card com Formulário */}
      <Card>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div>
            <h2 className="text-2xl font-semibold text-gray-900 mb-6">
              Entrar
            </h2>

            {/* Alerta de Erro */}
            {error && (
              <div className="mb-4">
                <Alert type="error" onClose={() => setError('')}>
                  {error}
                </Alert>
              </div>
            )}

            {/* Campo Email */}
            <div className="mb-4">
              <Input
                label="Email"
                type="email"
                placeholder="seu@email.com"
                error={errors.email?.message}
                {...register('email')}
                disabled={isLoading}
              />
            </div>

            {/* Campo Senha */}
            <div className="mb-6">
              <Input
                label="Senha"
                type="password"
                placeholder="••••••"
                error={errors.password?.message}
                {...register('password')}
                disabled={isLoading}
              />
            </div>

            {/* Botão Submit */}
            <Button
              type="submit"
              fullWidth
              isLoading={isLoading}
            >
              Entrar
            </Button>
          </div>
        </form>
      </Card>

      {/* Link de Ajuda (Opcional) */}
      <div className="text-center mt-4">
        <p className="text-sm text-gray-600">
          Problemas para acessar? Entre em contato com o administrador.
        </p>
      </div>
    </div>
  );
};
