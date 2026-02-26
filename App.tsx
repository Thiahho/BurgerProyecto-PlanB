import React, { Suspense, lazy } from "react";
import { HashRouter, Routes, Route, Navigate } from "react-router-dom";
import { CatalogProvider } from "./hooks/useCatalog";
import { AuthProvider, useAuth } from "./hooks/useAuth";
import { ToastProvider } from "./contexts/ToastContext";
import { CartProvider } from "./contexts/CartContext";

// Lazy loading de componentes públicos
const CatalogPage = lazy(() => import("./components/public/CatalogPage"));
const InfoPage = lazy(() => import("./components/public/InfoPage"));
const OrderTrackingPage = lazy(() => import("./components/public/OrderTrackingPage"));

// Lazy loading de componentes de admin
const LoginPage = lazy(() => import("./components/admin/LoginPage"));
const AdminLayout = lazy(() => import("./components/admin/AdminLayout"));
const Dashboard = lazy(() => import("./components/admin/Dashboard"));
const ProductManager = lazy(() => import("./components/admin/ProductManager"));
const CategoryManager = lazy(() => import("./components/admin/CategoryManager"));
const SiteSettings = lazy(() => import("./components/admin/SiteSettings"));
const OrderManager = lazy(() => import("./components/admin/OrderManager"));
const ModifierManager = lazy(() => import("./components/admin/ModifierManager"));
const CouponManager = lazy(() => import("./components/admin/CouponManager"));
const ComboManager = lazy(() => import("./components/admin/ComboManager"));
const ReportsManager = lazy(() => import("./components/admin/ReportsManager"));
const GrowthManager = lazy(() => import("./components/admin/GrowthManager"));

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
};

// Componente de loading mejorado
const LoadingFallback = () => (
  <div className="flex justify-center items-center min-h-screen bg-[#1a1310]">
    <div className="text-center">
      <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4"></div>
      <p className="text-white/60">Cargando...</p>
    </div>
  </div>
);

function App() {
  return (
    <ToastProvider>
      <CatalogProvider>
        <CartProvider>
          <AuthProvider>
            <HashRouter>
              <Suspense fallback={<LoadingFallback />}>
                <Routes>
                  {/* Public Routes */}
                  <Route path="/" element={<CatalogPage />} />
                  <Route path="/info" element={<InfoPage />} />
                  <Route path="/pedido/:code" element={<OrderTrackingPage />} />

                  {/* Admin Routes */}
                  <Route path="/login" element={<LoginPage />} />
                  <Route
                    path="/admin"
                    element={
                      <ProtectedRoute>
                        <AdminLayout />
                      </ProtectedRoute>
                    }
                  >
                    <Route index element={<Dashboard />} />
                    <Route path="products" element={<ProductManager />} />
                    <Route path="categories" element={<CategoryManager />} />
                    <Route path="modifiers" element={<ModifierManager />} />
                    <Route path="coupons" element={<CouponManager />} />
                    <Route path="combos" element={<ComboManager />} />
                    <Route path="growth" element={<GrowthManager />} />
                    <Route path="orders" element={<OrderManager />} />
                    <Route path="reports" element={<ReportsManager />} />
                    <Route path="settings" element={<SiteSettings />} />
                  </Route>
                </Routes>
              </Suspense>
            </HashRouter>
          </AuthProvider>
        </CartProvider>
      </CatalogProvider>
    </ToastProvider>
  );
}

export default App;
