/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState, useEffect } from 'react';
import {
    Table,
    Button,
    Group,
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
    Title,
    Paper,
    Badge,
    rem,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
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
    const [loading, setLoading] = useState<boolean>(true);
    const [modalOpened, { open: openModal, close: closeModal }] = useDisclosure(false);
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

    useEffect(() => {
        fetchList();
    }, []);

    const resetForm = () => {
        setForm({
            firstName: '',
            lastName: '',
            dateOfBirth: new Date().toISOString().split('T')[0],
            isActive: true,
            photoFile: null,
        });
        setCurrent(null);
    };

    const openCreate = () => {
        setIsEdit(false);
        resetForm();
        openModal();
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
        openModal();
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm('Вы уверены, что хотите удалить эту запись?')) return;
        try {
            await deleteBirthday(id);
            showNotification({ color: 'green', message: 'Запись успешно удалена' });
            fetchList();
        } catch {
            showNotification({ color: 'red', message: 'Ошибка при удалении записи' });
        }
    };

    const handleSubmit = async () => {
        try {
            const data = new FormData();
            if (isEdit && current) {
                data.append('entityId', current.entityId);
            }
            data.append('firstName', form.firstName);
            data.append('lastName', form.lastName);
            data.append('dateOfBirth', new Date(form.dateOfBirth).toISOString());
            data.append('isActive', form.isActive.toString());
            if (form.photoFile) {
                data.append('photo', form.photoFile);
            }

            if (isEdit) {
                await updateBirthday(data);
                showNotification({ color: 'green', message: 'Запись успешно обновлена' });
            } else {
                await createBirthday(data as any);
                showNotification({ color: 'green', message: 'Запись успешно создана' });
            }
            closeModal();
            fetchList();
        } catch {
            showNotification({ color: 'red', message: 'Ошибка при сохранении записи' });
        }
    };

    const withAge = birthdays.map(b => {
        const dob = new Date(b.dateOfBirth);
        let age = new Date().getFullYear() - dob.getFullYear();
        const m = new Date().getMonth() - dob.getMonth();
        if (m < 0 || (m === 0 && new Date().getDate() < dob.getDate())) {
            age--;
        }
        return { ...b, age };
    });

    const sorted = [...withAge].sort((a, b) => {
        let diff = 0;
        if (sortField === 'age') {
            diff = a.age - b.age;
        } else if (sortField === 'dateOfBirth') {
            diff = new Date(a.dateOfBirth).getTime() - new Date(b.dateOfBirth).getTime();
        } else {
            diff = a[sortField].localeCompare(b[sortField]);
        }
        return sortOrder === 'asc' ? diff : -diff;
    });

    const toggleSort = (field: SortField) => {
        if (sortField === field) {
            setSortOrder(prev => (prev === 'asc' ? 'desc' : 'asc'));
        } else {
            setSortField(field);
            setSortOrder('asc');
        }
    };

    const SortIcon = ({ field }: { field: SortField }) => {
        if (sortField !== field) return null;
        return sortOrder === 'asc' ? <IconArrowUp size={14} /> : <IconArrowDown size={14} />;
    };

    const rows = sorted.map(b => {
        const dob = new Date(b.dateOfBirth);
        const isToday = dob.getMonth() === new Date().getMonth() && dob.getDate() === new Date().getDate();

        return (
            <Table.Tr key={b.entityId} bg={isToday ? 'var(--mantine-color-green-light)' : undefined}>
                <Table.Td>
                    {b.photoPath ? (
                        <Image
                            src={`https://localhost:5135/images/${b.photoPath}`}
                            width={128}
                            height={128}
                            fit="cover"
                            radius="md"
                            alt={`${b.firstName} ${b.lastName}`}
                        />
                    ) : (
                        <Text c="dimmed" w={128} ta="center">Нет фото</Text>
                    )}
                </Table.Td>
                <Table.Td>{b.firstName}</Table.Td>
                <Table.Td>{b.lastName}</Table.Td>
                <Table.Td>{dob.toLocaleDateString()}</Table.Td>
                <Table.Td>{b.age}</Table.Td>
                <Table.Td>
                    <Badge color={b.isActive ? 'teal' : 'gray'} variant="light">
                        {b.isActive ? 'Активен' : 'Неактивен'}
                    </Badge>
                </Table.Td>
                <Table.Td>
                    <Group gap={4}>
                        <ActionIcon variant="subtle" onClick={() => openEdit(b)}>
                            <IconEdit style={{ width: rem(16), height: rem(16) }} />
                        </ActionIcon>
                        <ActionIcon variant="subtle" color="red" onClick={() => handleDelete(b.entityId)}>
                            <IconTrash style={{ width: rem(16), height: rem(16) }} />
                        </ActionIcon>
                    </Group>
                </Table.Td>
            </Table.Tr>
        );
    });

    return (
        <Container
            fluid
            p="md"
            style={{
                marginLeft: '-35vh',
                minHeight: '80vh',
                minWidth: '120vh',
                display: 'flex',
                flexDirection: 'column',
            }}
        >
            <Group justify="space-between" mb="xl">
                <Title order={2}>🎂 Список дней рождений</Title>
                <Button onClick={openCreate}>Добавить запись</Button>
            </Group>

            {loading && <Center mt="xl"><Loader /></Center>}

            {!loading && birthdays.length === 0 && (
                <Paper withBorder p="xl" radius="md" style={{ textAlign: 'center' }}>
                    <Text>Список пуст.</Text>
                </Paper>
            )}

            {!loading && birthdays.length > 0 && (
                <Paper withBorder radius="md" style={{ flex: 1, display: 'flex', flexDirection: 'column' }}>
                    <Table.ScrollContainer minWidth={800} style={{ flex: 1 }}>
                        <Table highlightOnHover verticalSpacing="md" striped>
                            <Table.Thead>
                                <Table.Tr>
                                    <Table.Th>Фото</Table.Th>
                                    <Table.Th onClick={() => toggleSort('firstName')} style={{ cursor: 'pointer' }}>
                                        <Group gap="xs">Имя <SortIcon field="firstName" /></Group>
                                    </Table.Th>
                                    <Table.Th onClick={() => toggleSort('lastName')} style={{ cursor: 'pointer' }}>
                                        <Group gap="xs">Фамилия <SortIcon field="lastName" /></Group>
                                    </Table.Th>
                                    <Table.Th onClick={() => toggleSort('dateOfBirth')} style={{ cursor: 'pointer' }}>
                                        <Group gap="xs">Дата рождения <SortIcon field="dateOfBirth" /></Group>
                                    </Table.Th>
                                    <Table.Th onClick={() => toggleSort('age')} style={{ cursor: 'pointer' }}>
                                        <Group gap="xs">Возраст <SortIcon field="age" /></Group>
                                    </Table.Th>
                                    <Table.Th>Статус</Table.Th>
                                    <Table.Th>Действия</Table.Th>
                                </Table.Tr>
                            </Table.Thead>
                            <Table.Tbody>{rows}</Table.Tbody>
                        </Table>
                    </Table.ScrollContainer>
                </Paper>
            )}

            <Modal opened={modalOpened} onClose={closeModal} title={isEdit ? 'Редактировать запись' : 'Создать запись'} centered>
                <TextInput
                    label="Имя"
                    value={form.firstName}
                    onChange={e => setForm({ ...form, firstName: e.currentTarget.value })}
                    mb="sm"
                    required
                />
                <TextInput
                    label="Фамилия"
                    value={form.lastName}
                    onChange={e => setForm({ ...form, lastName: e.currentTarget.value })}
                    mb="sm"
                    required
                />
                <TextInput
                    type="date"
                    label="Дата рождения"
                    value={form.dateOfBirth}
                    onChange={e => setForm({ ...form, dateOfBirth: e.currentTarget.value })}
                    mb="sm"
                    required
                />
                <FileInput
                    placeholder="Выберите файл"
                    label="Фото"
                    accept="image/*"
                    value={form.photoFile}
                    onChange={(file) => setForm({ ...form, photoFile: file })}
                    mb="sm"
                />
                <Switch
                    label="Активен"
                    checked={form.isActive}
                    onChange={e => setForm({ ...form, isActive: e.currentTarget.checked })}
                    mb="md"
                />
                <Group justify="flex-end" mt="md">
                    <Button variant="default" onClick={closeModal}>Отмена</Button>
                    <Button onClick={handleSubmit}>{isEdit ? 'Сохранить изменения' : 'Создать'}</Button>
                </Group>
            </Modal>
        </Container>
    );
};