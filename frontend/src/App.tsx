import React from 'react'

function App() {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-black mb-4">
          Sistema de Rede de Credenciamento
        </h1>
        <p className="text-gray-600 text-lg">
          Frontend React - Setup Completo ✓
        </p>
        <div className="mt-8 p-6 bg-white rounded-lg shadow-lg border-2 border-black max-w-md mx-auto">
          <h2 className="text-2xl font-semibold mb-4">Tecnologias</h2>
          <ul className="text-left space-y-2 text-gray-700">
            <li>✓ React 18 + TypeScript</li>
            <li>✓ Vite</li>
            <li>✓ Tailwind CSS (Preto e Branco)</li>
            <li>✓ React Router</li>
            <li>✓ React Query</li>
            <li>✓ Zustand</li>
            <li>✓ Axios</li>
            <li>✓ React Hook Form + Zod</li>
          </ul>
        </div>
      </div>
    </div>
  )
}

export default App
