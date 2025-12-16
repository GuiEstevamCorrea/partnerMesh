import React, { forwardRef } from 'react';

interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  helperText?: string;
}

export const Textarea = forwardRef<HTMLTextAreaElement, TextareaProps>(
  ({ label, error, helperText, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="block text-sm font-medium text-gray-900 mb-1">
            {label}
            {props.required && <span className="text-gray-700 ml-1">*</span>}
          </label>
        )}
        <textarea
          ref={ref}
          className={`
            w-full px-3 py-2 
            border-2 border-gray-300 rounded-md
            text-gray-900 placeholder-gray-400
            focus:outline-none focus:ring-2 focus:ring-black focus:border-black
            disabled:bg-gray-100 disabled:cursor-not-allowed
            resize-y
            ${error ? 'border-gray-700 focus:border-gray-700 focus:ring-gray-700' : ''}
            ${className}
          `}
          {...props}
        />
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

Textarea.displayName = 'Textarea';
