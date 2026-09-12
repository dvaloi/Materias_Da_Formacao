import { createContext, useContext, useState, useCallback } from 'react';
import { authApi } from '../services/api';

const AuthContext = createContext(null);

// M14 — Context API: estado de autenticação global (evita prop drilling de user/token)
export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem('inss_user');
    return saved ? JSON.parse(saved) : null;
  });

  const login = useCallback(async (email, senha) => {
    const response = await authApi.login(email, senha);
    // JWT fica no localStorage — api.js lê e manda no header Bearer
    localStorage.setItem('inss_token', response.token);
    const userData = { nome: response.nome, email, expiraEm: response.expiraEm };
    localStorage.setItem('inss_user', JSON.stringify(userData));
    setUser(userData);
    return userData;
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('inss_token');
    localStorage.removeItem('inss_user');
    setUser(null);
  }, []);

  const isAuthenticated = !!user && !!localStorage.getItem('inss_token');

  return (
    <AuthContext.Provider value={{ user, login, logout, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth deve ser usado dentro de AuthProvider');
  return ctx;
}
