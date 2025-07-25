import React, { useState, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { TextInput, PasswordInput, Button, Paper, Title, Text, Alert, Stack } from '@mantine/core';
import { AuthContext } from '../contexts/AuthProvider';
import type { CreateUserRequest } from '../types';

export const Register: React.FC = () => {
    const { register } = useContext(AuthContext);
    const navigate = useNavigate();
    const [form, setForm] = useState<CreateUserRequest>({
        firstName: '',
        lastName: '',
        login: '',
        password: '',
    });
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await register(form);
            navigate('/dashboard');
        } catch {
            setError('Ошибка регистрации');
        }
    };

    return (
        <Paper radius="md" p="xl" withBorder>
            <Title>Регистрация</Title>
            <form onSubmit={handleSubmit}>
                <Stack mt="md">
                    <TextInput
                        label="Имя"
                        placeholder="Ваше имя"
                        value={form.firstName}
                        onChange={e => setForm({ ...form, firstName: e.target.value })}
                        required
                    />
                    <TextInput
                        label="Фамилия"
                        placeholder="Ваша фамилия"
                        value={form.lastName}
                        onChange={e => setForm({ ...form, lastName: e.target.value })}
                        required
                    />
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
                        Зарегистрироваться
                    </Button>
                </Stack>
            </form>
            <Text size="sm" mt="md">
                Уже есть аккаунт? <Link to="/login">Войти</Link>
            </Text>
        </Paper>
    );
};
