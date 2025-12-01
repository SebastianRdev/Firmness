import api from '../api';

/**
 * API client for customer sales operations
 */
const customerSaleApi = {
    /**
     * Creates a new sale with PDF receipt and email notification
     * @param {Object} saleData - Sale data including customer and products
     * @returns {Promise<Object>} Created sale response
     */
    createSale: async (saleData) => {
        try {
            const response = await api.post('/customer/sales', saleData);
            return response.data;
        } catch (error) {
            // Enhanced error handling
            if (error.response) {
                // Server responded with error
                const errorMessage = error.response.data?.error || 'Error al crear la venta';
                throw new Error(errorMessage);
            } else if (error.request) {
                // Request made but no response
                throw new Error('No se pudo conectar con el servidor. Por favor, verifica tu conexión.');
            } else {
                // Something else happened
                throw new Error('Error inesperado al procesar la venta.');
            }
        }
    },

    /**
     * Gets a sale by its ID
     * @param {number} id - Sale ID
     * @returns {Promise<Object>} Sale details
     */
    getSaleById: async (id) => {
        try {
            const response = await api.get(`/customer/sales/${id}`);
            return response.data;
        } catch (error) {
            if (error.response?.status === 404) {
                throw new Error('Venta no encontrada');
            }
            throw new Error('Error al obtener los detalles de la venta');
        }
    }
};

export default customerSaleApi;
