import api from './api';

const categoryService = {
    getAll: async () => {
        const response = await api.get('/categories');
        return response.data;
    }
};

export default categoryService;
