import { Navigate } from 'react-router-dom';

interface PrivateRouteProps {
  children: React.ReactNode;
}

export const PrivateRoute = ({ children }: PrivateRouteProps) => {
  const token = localStorage.getItem('token');
  
  if (!token) {
    // Redirect them to the login page if not authenticated
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};
