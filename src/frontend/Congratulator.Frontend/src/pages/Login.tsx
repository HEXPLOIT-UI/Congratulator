import React, { useState, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { TextInput, PasswordInput, Button, Paper, Title, Text, Alert, Stack } from '@mantine/core';
import { AuthContext } from '../contexts/AuthProvider';

export const Login: React.FC = () => {
    const { login } = useContext(AuthContext);
    const navigate = useNavigate();
    const [form, setForm] = useState({ login: '', password: '' });
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await login({ login: form.login, password: form.password });
            navigate('/dashboard');
        } catch {
            setError('Неверные учетные данные');
        }
    };

    return (
        <Paper radius="md" p="xl" withBorder>
            <Title>Вход</Title>
            <form onSubmit={handleSubmit}>
                <Stack mt="md">
                    <TextInput
                        label="Логин"
                        placeholder="Введите логин"
                        value={form.login}
                        onChange={e => setForm({ ...form, login: e.target.value })}
                        required
                    />
                    <PasswordInput
                        label="Пароль"
                        placeholder="Введите пароль"
                        value={form.password}
                        onChange={e => setForm({ ...form, password: e.target.value })}
                        required
                    />
                    {error && <Alert color="red">{error}</Alert>}
                    <Button fullWidth mt="xl" type="submit">
                        Войти
                    </Button>
                </Stack>
            </form>
            <Text size="sm" mt="md">
                Нет аккаунта? <Link to="/register">Зарегистрироваться</Link>
            </Text>
        </Paper>
    );
};
