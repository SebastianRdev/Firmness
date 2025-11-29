import api from './api';

const productService = {
    getAll: async () => {
        const response = await api.get('/products');
        return response.data;
    },

    getById: async (id) => {
        const response = await api.get(`/products/${id}`);
        return response.data;
    },

    search: async (term) => {
        const response = await api.get(`/products/search?term=${term}`);
        return response.data;
    },
};

export default productService;
