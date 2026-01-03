import { createBrowserRouter, Navigate } from 'react-router-dom';
import { PublicLayout } from '@/components/layout/PublicLayout';
import { Layout } from '@/components/layout/Layout';
import { PrivateRoute } from './PrivateRoute';
import { LoginPage } from '@/pages/auth/LoginPage';
import { DashboardPage } from '@/pages/DashboardPage';
import { UsersListPage, UserFormPage } from '@/pages/Users';
import { VectorsListPage, VectorFormPage } from '@/pages/Vectors';
import { PartnersListPage, PartnerFormPage, PartnerTreePage } from '@/pages/Partners';
import { BusinessTypesListPage, BusinessTypeFormPage } from '@/pages/BusinessTypes';
import { BusinessListPage, BusinessFormPage, BusinessDetailPage } from '@/pages/Business';
import { PaymentsListPage } from '@/pages/Payments';
import { AuditLogsPage } from '@/pages/Audit/AuditLogsPage';
import { AuditTimelinePage } from '@/pages/Audit/AuditTimelinePage';

export const router = createBrowserRouter([
  // ==================== ROTAS PÚBLICAS ====================
  {
    path: '/',
    element: <PublicLayout />,
    children: [
      {
        index: true,
        element: <Navigate to="/login" replace />,
      },
      {
        path: 'login',
        element: <LoginPage />,
      },
    ],
  },

  // ==================== ROTAS PROTEGIDAS ====================
  {
    path: '/',
    element: (
      <PrivateRoute>
        <Layout />
      </PrivateRoute>
    ),
    children: [
      // Dashboard
      {
        path: 'dashboard',
        element: <DashboardPage />,
      },

      // ========== ENTREGÁVEL 05 - Gestão de Usuários e Vetores ==========
      {
        path: 'usuarios',
        children: [
          { index: true, element: <UsersListPage /> },
          { path: 'novo', element: <UserFormPage /> },
          { path: ':id/editar', element: <UserFormPage /> },
        ],
      },
      {
        path: 'vetores',
        children: [
          { index: true, element: <VectorsListPage /> },
          { path: 'novo', element: <VectorFormPage /> },
          { path: ':id/editar', element: <VectorFormPage /> },
        ],
      },

      // ========== ENTREGÁVEL 06 - Gestão de Parceiros e Tipos de Negócio ==========
      {
        path: 'parceiros',
        children: [
          { index: true, element: <PartnersListPage /> },
          { path: 'novo', element: <PartnerFormPage /> },
          { path: ':id/editar', element: <PartnerFormPage /> },
          { path: 'arvore', element: <PartnerTreePage /> },
        ],
      },
      {
        path: 'tipos-negocio',
        children: [
          { index: true, element: <BusinessTypesListPage /> },
          { path: 'novo', element: <BusinessTypeFormPage /> },
          { path: ':id/editar', element: <BusinessTypeFormPage /> },
        ],
      },

      // ========== ENTREGÁVEL 07 - Gestão de Negócios e Comissões ==========
      {
        path: 'negocios',
        children: [
          { index: true, element: <BusinessListPage /> },
          { path: 'novo', element: <BusinessFormPage /> },
          { path: ':id', element: <BusinessDetailPage /> },
          { path: ':id/editar', element: <BusinessFormPage /> },
          // { path: ':id/pagamentos', element: <BusinessPaymentsPage /> },
        ],
      },
      {
        path: 'pagamentos',
        element: <PaymentsListPage />,
      },

      // ========== ENTREGÁVEL 09 - Auditoria e Logs ==========
      {
        path: 'auditoria',
        children: [
          { index: true, element: <AuditLogsPage /> },
          { path: 'timeline/:entityType/:entityId', element: <AuditTimelinePage /> },
        ],
      },
    ],
  },

  // ==================== WILDCARD - Redireciona para login ====================
  {
    path: '*',
    element: <Navigate to="/login" replace />,
  },
]);
