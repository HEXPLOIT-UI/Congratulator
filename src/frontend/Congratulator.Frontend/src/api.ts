import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:5135/api',
    withCredentials: true,
});

export default api;