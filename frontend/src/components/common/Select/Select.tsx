import React, { forwardRef } from 'react';
import { SelectOption } from '@/types';

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  helperText?: string;
  options: SelectOption[];
  placeholder?: string;
}

export const Select = forwardRef<HTMLSelectElement, SelectProps>(
  ({ label, error, helperText, options, placeholder, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="block text-sm font-medium text-gray-900 mb-1">
            {label}
            {props.required && <span className="text-gray-700 ml-1">*</span>}
          </label>
        )}
        <select
          ref={ref}
          className={`
            w-full px-3 py-2 
            border-2 border-gray-300 rounded-md
            text-gray-900
            focus:outline-none focus:ring-2 focus:ring-black focus:border-black
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${error ? 'border-gray-700' : ''}
            ${className}
          `}
          {...props}
        >
          {placeholder && (
            <option value="">{placeholder}</option>
          )}
          {options.map((option) => (
            <option 
              key={option.value} 
              value={option.value}
              disabled={option.disabled}
            >
              {option.label}
            </option>
          ))}
        </select>
        {error && (
          <p className="mt-1 text-sm text-gray-700">{error}</p>
        )}
        {helperText && !error && (
          <p className="mt-1 text-sm text-gray-500">{helperText}</p>
        )}
      </div>
    );
  }
);

Select.displayName = 'Select';
