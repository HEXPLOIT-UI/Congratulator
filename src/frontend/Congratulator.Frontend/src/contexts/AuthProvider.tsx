import React, { createContext, useState, useEffect, type ReactNode } from 'react';
import api from '../api';
import type { CreateUserRequest, LoginUserRequest, UserDTO } from '../types';

interface AuthContextProps {
    user: UserDTO | null;
    loading: boolean;
    login: (payload: LoginUserRequest) => Promise<void>;
    register: (data: CreateUserRequest) => Promise<void>;
    logout: () => Promise<void>;
}

// eslint-disable-next-line react-refresh/only-export-components
export const AuthContext = createContext<AuthContextProps>({} as AuthContextProps);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<UserDTO | null>(null);
    const [loading, setLoading] = useState(true);

    const checkAuth = async () => {
        try {
            const { data } = await api.get<UserDTO>('/Auth/check');
            setUser(data);
        } catch {
            setUser(null);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        checkAuth();
    }, []);

    const login = async (payload: LoginUserRequest) => {
        const form = new URLSearchParams();
        form.append('login', payload.login);
        form.append('password', payload.password);
        await api.post('/Auth/login', form);
        await checkAuth();
    };

    const register = async (data: CreateUserRequest) => {
        const form = new URLSearchParams();
        form.append('firstName', data.firstName);
        form.append('lastName', data.lastName);
        form.append('login', data.login);
        form.append('password', data.password);
        await api.post('/Auth/register', form);
        await checkAuth();
    };

    const logout = async () => {
        await api.post('/Auth/logout');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, loading, login, register, logout }}>
            {children}
        </AuthContext.Provider>
    );
};
