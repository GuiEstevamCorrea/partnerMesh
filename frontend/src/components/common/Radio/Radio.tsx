import React, { forwardRef } from 'react';

interface RadioOption {
  value: string;
  label: string;
  disabled?: boolean;
}

interface RadioProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label?: string;
  options: RadioOption[];
  error?: string;
}

export const Radio = forwardRef<HTMLInputElement, RadioProps>(
  ({ label, options, error, name, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="block text-sm font-medium text-gray-900 mb-2">
            {label}
            {props.required && <span className="text-gray-700 ml-1">*</span>}
          </label>
        )}
        <div className="space-y-2">
          {options.map((option) => (
            <label key={option.value} className="flex items-center space-x-2 cursor-pointer">
              <input
                ref={ref}
                type="radio"
                name={name}
                value={option.value}
                disabled={option.disabled}
                className={`
                  w-4 h-4
                  border-2 border-gray-300
                  text-black
                  focus:ring-2 focus:ring-black focus:ring-offset-0
                  disabled:opacity-50 disabled:cursor-not-allowed
                  ${error ? 'border-gray-700' : ''}
                  ${className}
                `}
                {...props}
              />
              <span className={`text-sm font-medium ${option.disabled ? 'text-gray-400' : 'text-gray-900'}`}>
                {option.label}
              </span>
            </label>
          ))}
        </div>
        {error && (
          <p className="mt-1 text-sm text-gray-700">{error}</p>
        )}
      </div>
    );
  }
);

Radio.displayName = 'Radio';
