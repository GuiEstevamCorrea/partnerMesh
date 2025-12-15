import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

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
    <QueryClientProvider client={queryClient}>
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-screen">
          <h1 className="text-3xl font-bold text-black">
            Configuração Base Completa ✓
          </h1>
        </div>
      </div>
    </QueryClientProvider>
  );
}

export default App;
