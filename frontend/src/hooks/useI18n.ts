import { useTranslation } from 'react-i18next';
import { format } from 'date-fns';
import { ptBR, enUS, es } from 'date-fns/locale';

export const useI18n = () => {
  const { t, i18n } = useTranslation();

  // Formatação de moeda baseada no idioma
  const formatCurrency = (value: number): string => {
    const locale = i18n.language;
    
    switch (locale) {
      case 'pt-BR':
        return new Intl.NumberFormat('pt-BR', {
          style: 'currency',
          currency: 'BRL'
        }).format(value);
      case 'en':
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(value);
      case 'es':
        return new Intl.NumberFormat('es-ES', {
          style: 'currency',
          currency: 'EUR'
        }).format(value);
      default:
        return new Intl.NumberFormat('pt-BR', {
          style: 'currency',
          currency: 'BRL'
        }).format(value);
    }
  };

  // Formatação de data baseada no idioma
  const formatDate = (date: Date | string, pattern = 'dd/MM/yyyy'): string => {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const locale = i18n.language;

    const localeMap = {
      'pt-BR': ptBR,
      'en': enUS,
      'es': es
    };

    const dateLocale = localeMap[locale as keyof typeof localeMap] || ptBR;
    
    return format(dateObj, pattern, { locale: dateLocale });
  };

  // Formatação de data e hora
  const formatDateTime = (date: Date | string): string => {
    const locale = i18n.language;
    const pattern = locale === 'en' ? 'MM/dd/yyyy HH:mm' : 'dd/MM/yyyy HH:mm';
    return formatDate(date, pattern);
  };

  // Tradução de status
  const translateStatus = (status: string, context = 'common'): string => {
    const statusKey = status.toLowerCase();
    return t(`${context}.status.${statusKey}`, status);
  };

  // Pluralização
  const pluralize = (count: number, singular: string, plural?: string): string => {
    if (count === 1) {
      return t(singular);
    }
    return t(plural || `${singular}_plural`, { count });
  };

  // Mudança de idioma
  const changeLanguage = (language: string) => {
    i18n.changeLanguage(language);
  };

  return {
    t,
    i18n,
    formatCurrency,
    formatDate,
    formatDateTime,
    translateStatus,
    pluralize,
    changeLanguage,
    currentLanguage: i18n.language
  };
};

export default useI18n;