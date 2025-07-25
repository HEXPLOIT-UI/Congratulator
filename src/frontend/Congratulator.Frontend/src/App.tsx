import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { MantineProvider, Container } from '@mantine/core';
import { AuthProvider } from './contexts/AuthProvider';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Dashboard } from './pages/Dashboard';
import { ProtectedRoute } from './components/ProtectedRoute';

export const App: React.FC = () => (
    <MantineProvider
        theme={{
            // Опциональные настройки темы
            /**
             * colorScheme: 'light' | 'dark',
             * components: { ... }
             */
        }}
    >
        <AuthProvider>
            <Container size={420} my={40}>
                <BrowserRouter>
                    <Routes>
                        <Route path="/" element={<Navigate to="/login" />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route
                            path="/dashboard"
                            element={
                                <ProtectedRoute>
                                    <Dashboard />
                                </ProtectedRoute>
                            }
                        />
                    </Routes>
                </BrowserRouter>
            </Container>
        </AuthProvider>
    </MantineProvider>
);