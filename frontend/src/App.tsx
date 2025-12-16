import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ErrorBoundary, ToastProvider } from './components';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutos
    },
  },
});

function App() {
  return (
    <ErrorBoundary>
      <QueryClientProvider client={queryClient}>
        <ToastProvider>
          <div className="min-h-screen bg-gray-50">
            <div className="flex items-center justify-center h-screen">
              <h1 className="text-3xl font-bold text-black">
                Componentes Comuns Completos ✓
              </h1>
            </div>
          </div>
        </ToastProvider>
      </QueryClientProvider>
    </ErrorBoundary>
  );
}

export default App;
