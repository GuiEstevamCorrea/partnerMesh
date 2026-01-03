import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Input, Button, Alert, Card } from '@/components';
import { authApi } from '@/api/endpoints/auth.api';
import { useAuthStore } from '@/store/auth.store';
import { LanguageSelector } from '@/components/common/LanguageSelector';
import { useI18n } from '@/hooks/useI18n';

const createLoginSchema = (t: any) => z.object({
  email: z
    .string()
    .min(1, t('auth.validation.emailRequired'))
    .email(t('auth.validation.emailInvalid')),
  password: z
    .string()
    .min(6, t('auth.validation.passwordMin')),
});

type LoginFormData = z.infer<ReturnType<typeof createLoginSchema>>;

export const LoginPage = () => {
  const navigate = useNavigate();
  const setAuth = useAuthStore((state) => state.setAuth);
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState(false);
  const { t } = useI18n();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(createLoginSchema(t)),
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
        t('auth.loginError');
      
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="w-full max-w-md">
      {/* Seletor de Idioma */}
      <div className="flex justify-end mb-4">
        <LanguageSelector />
      </div>

      {/* Card com Formulário */}
      <Card>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div>
            <h2 className="text-2xl font-semibold text-gray-900 mb-2">
              {t('auth.loginTitle')}
            </h2>
            <p className="text-sm text-gray-600 mb-6">
              {t('auth.loginSubtitle')}
            </p>

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
                label={t('auth.email')}
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
                label={t('auth.password')}
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
              {t('auth.login')}
            </Button>
          </div>
        </form>
      </Card>

      {/* Link de Ajuda (Opcional) */}
      <div className="text-center mt-4">
        <p className="text-sm text-gray-600">
          {t('auth.forgotPassword')}
        </p>
      </div>
    </div>
  );
};
