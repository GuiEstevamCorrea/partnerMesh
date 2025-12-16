import React, { forwardRef } from 'react';

interface CheckboxProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label?: string;
  error?: string;
}

export const Checkbox = forwardRef<HTMLInputElement, CheckboxProps>(
  ({ label, error, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        <label className="flex items-center space-x-2 cursor-pointer">
          <input
            ref={ref}
            type="checkbox"
            className={`
              w-4 h-4
              border-2 border-gray-300 rounded
              text-black
              focus:ring-2 focus:ring-black focus:ring-offset-0
              disabled:opacity-50 disabled:cursor-not-allowed
              ${error ? 'border-gray-700' : ''}
              ${className}
            `}
            {...props}
          />
          {label && (
            <span className="text-sm font-medium text-gray-900">{label}</span>
          )}
        </label>
        {error && (
          <p className="mt-1 text-sm text-gray-700">{error}</p>
        )}
      </div>
    );
  }
);

Checkbox.displayName = 'Checkbox';
