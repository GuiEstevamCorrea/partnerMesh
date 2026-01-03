import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

// Recursos de tradução
import ptBR from './locales/pt-BR.json';
import en from './locales/en.json';
import es from './locales/es.json';

const resources = {
  'pt-BR': {
    translation: ptBR
  },
  en: {
    translation: en
  },
  es: {
    translation: es
  }
};

i18n
  .use(LanguageDetector) // Detecta idioma do navegador
  .use(initReactI18next) // Integra com React
  .init({
    resources,
    fallbackLng: 'pt-BR', // Idioma padrão
    debug: false, // Ative para debug

    // Opções de detecção
    detection: {
      order: ['localStorage', 'navigator', 'htmlTag'],
      lookupLocalStorage: 'i18nextLng',
      caches: ['localStorage']
    },

    interpolation: {
      escapeValue: false // React já faz escape
    }
  });

export default i18n;