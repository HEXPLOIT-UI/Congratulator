/* eslint-disable @typescript-eslint/no-explicit-any */
// src/pages/Dashboard.tsx
import React, { useState, useEffect } from 'react';
import {
    Table,
    Button,
    Group,
    Box,
    Modal,
    TextInput,
    Switch,
    ActionIcon,
    Text,
    Loader,
    Center,
    Container,
    FileInput,
    Image,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { IconEdit, IconTrash, IconArrowUp, IconArrowDown } from '@tabler/icons-react';
import {
    getBirthdays,
    createBirthday,
    updateBirthday,
    deleteBirthday,
} from '../services/BirthdayService';
import type { BirthdayDTO } from '../types';

interface FormBirthday {
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    isActive: boolean;
    photoFile: File | null;
}

type SortField = 'firstName' | 'lastName' | 'dateOfBirth' | 'age';
type SortOrder = 'asc' | 'desc';

export const Dashboard: React.FC = () => {
    const [birthdays, setBirthdays] = useState<BirthdayDTO[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const [isEdit, setIsEdit] = useState<boolean>(false);
    const [current, setCurrent] = useState<BirthdayDTO | null>(null);
    const [form, setForm] = useState<FormBirthday>({
        firstName: '',
        lastName: '',
        dateOfBirth: new Date().toISOString().split('T')[0],
        isActive: true,
        photoFile: null,
    });
    const [sortField, setSortField] = useState<SortField>('dateOfBirth');
    const [sortOrder, setSortOrder] = useState<SortOrder>('asc');

    const fetchList = async () => {
        setLoading(true);
        try {
            const result = await getBirthdays(false);
            setBirthdays(result.result);
        } catch {
            showNotification({ color: 'red', message: 'Ошибка при загрузке списка' });
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchList(); }, []);

    const openCreate = () => {
        setIsEdit(false);
        setCurrent(null);
        setForm({
            firstName: '', lastName: '',
            dateOfBirth: new Date().toISOString().split('T')[0],
            isActive: true,
            photoFile: null,
        });
        setModalOpen(true);
    };

    const openEdit = (b: BirthdayDTO) => {
        setIsEdit(true);
        setCurrent(b);
        setForm({
            firstName: b.firstName,
            lastName: b.lastName,
            dateOfBirth: b.dateOfBirth.split('T')[0],
            isActive: b.isActive,
            photoFile: null,
        });
        setModalOpen(true);
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm('Удалить запись?')) return;
        try {
            await deleteBirthday(id);
            showNotification({ color: 'green', message: 'Удалено' });
            fetchList();
        } catch {
            showNotification({ color: 'red', message: 'Ошибка при удалении' });
        }
    };

    const handleSubmit = async () => {
        try {
            // Формируем FormData для отправки multipart/form-data
            const data = new FormData();
            if (current) {
                data.append('entityId', current.entityId);
            }
            data.append('firstName', form.firstName);
            data.append('lastName', form.lastName);
            data.append('dateOfBirth', new Date(form.dateOfBirth).toISOString());
            data.append('isActive', form.isActive.toString());
            if (form.photoFile) {
                data.append('photo', form.photoFile);
            }

            if (isEdit && current) {
                await updateBirthday(data);
                showNotification({ color: 'green', message: 'Обновлено' });
            } else {
                await createBirthday(data as any);
                showNotification({ color: 'green', message: 'Создано' });
            }

            setModalOpen(false);
            fetchList();
        } catch {
            showNotification({ color: 'red', message: 'Ошибка при сохранении' });
        }
    };

    // compute derived list with age
    const withAge = birthdays.map(b => {
        const dob = new Date(b.dateOfBirth);
        const today = new Date();
        const age = today.getFullYear() - dob.getFullYear();
        return { ...b, age };
    });

    // sort
    const sorted = [...withAge].sort((a, b) => {
        let diff = 0;
        if (sortField === 'age') diff = a.age - b.age;
        else if (sortField === 'dateOfBirth') diff = new Date(a.dateOfBirth).getTime() - new Date(b.dateOfBirth).getTime();
        else diff = a[sortField].localeCompare(b[sortField]);
        return sortOrder === 'asc' ? diff : -diff;
    });

    // toggle sort
    const toggleSort = (field: SortField) => {
        if (sortField === field) setSortOrder(prev => prev === 'asc' ? 'desc' : 'asc');
        else { setSortField(field); setSortOrder('asc'); }
    };

    // render sort icon
    const SortIcon = ({ field }: { field: SortField }) => (
        sortField === field ? (sortOrder === 'asc' ? <IconArrowUp size={14} /> : <IconArrowDown size={14} />) : null
    );

    const rows = sorted.map(b => {
        const dob = new Date(b.dateOfBirth);
        const bg = dob.getMonth() === new Date().getMonth() && dob.getDate() === new Date().getDate()
            ? 'lightgreen' : dob < new Date() ? 'lightcoral' : undefined;
        return (
            <tr key={b.entityId} style={{ backgroundColor: bg }}>
                <td>
                    {b.photoPath ? (
                        <Image
                            src={`https://localhost:5135/images/${b.photoPath}`}
                            width={256}
                            height={256}
                            fit="cover"
                            alt="Фото"
                        />
                    ) : <Text color="dimmed">Нет фото</Text>}
                </td>
                <td>{b.firstName}</td>
                <td>{b.lastName}</td>
                <td>{dob.toLocaleDateString()}</td>
                <td>{b.age}</td>
                <td>{b.isActive ? '✔' : '✖'}</td>
                <td>
                    <Group gap={4}>
                        <ActionIcon onClick={() => openEdit(b)}><IconEdit size={16} /></ActionIcon>
                        <ActionIcon color="red" onClick={() => handleDelete(b.entityId)}><IconTrash size={16} /></ActionIcon>
                    </Group>
                </td>
            </tr>
        );
    });

    return (
        <Container size="100%">
            <Box mb="md">
                <Group justify="apart">
                    <Text size="xl">Список дней рождений</Text>
                    <Button onClick={openCreate}>Добавить</Button>
                </Group>
            </Box>
            {loading ? <Center><Loader /></Center> : (
                <Table highlightOnHover horizontalSpacing="xl" verticalSpacing="sm" style={{ width: '100%' }}>
                    <thead>
                        <tr>
                            <th>Фото</th>
                            <th onClick={() => toggleSort('firstName')}>Имя <SortIcon field="firstName" /></th>
                            <th onClick={() => toggleSort('lastName')}><span>Фамилия <SortIcon field="lastName" /></span></th>
                            <th onClick={() => toggleSort('dateOfBirth')}><span>Дата <SortIcon field="dateOfBirth" /></span></th>
                            <th onClick={() => toggleSort('age')}><span>Возраст <SortIcon field="age" /></span></th>
                            <th>Активен</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>{rows}</tbody>
                </Table>
            )}

            <Modal opened={modalOpen} onClose={() => setModalOpen(false)} title={isEdit ? 'Редактировать' : 'Создать'}>
                <TextInput label="Имя" value={form.firstName} onChange={e => setForm({ ...form, firstName: e.currentTarget.value })} mb="sm" />
                <TextInput label="Фамилия" value={form.lastName} onChange={e => setForm({ ...form, lastName: e.currentTarget.value })} mb="sm" />
                <TextInput type="date" label="Дата рождения" value={form.dateOfBirth} onChange={e => setForm({ ...form, dateOfBirth: e.currentTarget.value })} mb="sm" />
                <Switch label="Активен" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.currentTarget.checked })} mb="sm" />
                <FileInput
                    placeholder="Выберите картинку"
                    label="Фото"
                    accept="image/*"
                    value={form.photoFile}
                    onChange={(file) => setForm({ ...form, photoFile: file })}
                    mb="md"
                />
                <Group justify="right"><Button onClick={handleSubmit}>{isEdit ? 'Сохранить' : 'Создать'}</Button></Group>
            </Modal>
        </Container>
    );
};
