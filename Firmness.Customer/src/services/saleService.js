import api from './api';

const saleService = {
    createSale: async (saleData) => {
        const response = await api.post('/sales', saleData);
        return response.data; // Usually returns the created sale object or a redirect URL
    },

    downloadReceipt: async (fileName) => {
        const response = await api.get(`/sales/downloadreceipt?fileName=${fileName}`, {
            responseType: 'blob',
        });
        return response.data;
    },
};

export default saleService;
