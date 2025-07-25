import api from '../api';
import type { ResultWithPaginationOfBirthdayDTO } from '../types';

const RESOURCE = '/Birthdays';

interface GetBirthdaysParams {
    ActiveOnly: boolean;
    Page: number;
    BatchSize: number;
    Search?: string;
}

export const getBirthdays = async (
    activeOnly = false,
    page = 1,
    batchSize = 20,
    query?: string
): Promise<ResultWithPaginationOfBirthdayDTO> => {
    const params: GetBirthdaysParams = { ActiveOnly: activeOnly, Page: page, BatchSize: batchSize };
    const endpoint = `${RESOURCE}/query`;
    if (query) params.Search = query;
    const { data } = await api.get<ResultWithPaginationOfBirthdayDTO>(endpoint, { params });
    return data;
};

export const createBirthday = async (
    formData: FormData
): Promise<string> => {
    const { data } = await api.post<string>(
        `${RESOURCE}/create`,
        formData,
        { headers: { 'Content-Type': 'multipart/form-data' } }
    );
    return data;
};

export const updateBirthday = async (
    formData: FormData
): Promise<void> => {
    await api.put(
        `${RESOURCE}`,
        formData,
        { headers: { 'Content-Type': 'multipart/form-data' } }
    );
};

export const deleteBirthday = async (
    id: string
): Promise<void> => {
    await api.delete(`${RESOURCE}/${id}`);
};
