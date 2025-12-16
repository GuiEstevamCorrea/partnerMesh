import React from 'react';
import { AlertCircle, CheckCircle, Info, X, XCircle } from 'lucide-react';

interface AlertProps {
  type?: 'info' | 'success' | 'warning' | 'error';
  title?: string;
  children: React.ReactNode;
  onClose?: () => void;
}

export const Alert: React.FC<AlertProps> = ({
  type = 'info',
  title,
  children,
  onClose,
}) => {
  const types = {
    info: {
      bg: 'bg-gray-100',
      border: 'border-gray-300',
      icon: <Info className="h-5 w-5 text-gray-900" />,
      text: 'text-gray-900',
    },
    success: {
      bg: 'bg-gray-50',
      border: 'border-gray-800',
      icon: <CheckCircle className="h-5 w-5 text-gray-900" />,
      text: 'text-gray-900',
    },
    warning: {
      bg: 'bg-gray-200',
      border: 'border-gray-500',
      icon: <AlertCircle className="h-5 w-5 text-gray-900" />,
      text: 'text-gray-900',
    },
    error: {
      bg: 'bg-gray-700',
      border: 'border-gray-900',
      icon: <XCircle className="h-5 w-5 text-white" />,
      text: 'text-white',
    },
  };

  const style = types[type];

  return (
    <div className={`${style.bg} ${style.border} border-2 rounded-lg p-4`}>
      <div className="flex items-start">
        <div className="flex-shrink-0">{style.icon}</div>
        <div className="ml-3 flex-1">
          {title && (
            <h3 className={`text-sm font-semibold ${style.text} mb-1`}>{title}</h3>
          )}
          <div className={`text-sm ${style.text}`}>{children}</div>
        </div>
        {onClose && (
          <button
            onClick={onClose}
            className={`ml-3 flex-shrink-0 ${style.text} hover:opacity-70`}
          >
            <X className="h-5 w-5" />
          </button>
        )}
      </div>
    </div>
  );
};
