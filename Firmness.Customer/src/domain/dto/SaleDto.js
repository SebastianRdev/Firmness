/**
 * DTO for creating a sale from the frontend
 */
export class CreateSaleRequest {
    constructor(customerId, cartItems, totals) {
        this.customerId = customerId;
        this.date = new Date().toISOString();
        this.totalAmount = totals.subtotal || 0;
        this.taxAmount = totals.taxes || 0;
        this.grandTotal = totals.grandTotal || 0;
        this.saleDetails = cartItems.map(item => ({
            productId: item.id,
            quantity: item.quantity,
            unitPrice: item.price
        }));
    }
}

/**
 * Response DTO for a created sale
 */
export class SaleResponse {
    constructor(data) {
        this.id = data.id;
        this.customerId = data.customerId;
        this.customerName = data.customerName;
        this.customerEmail = data.customerEmail;
        this.date = data.date;
        this.totalAmount = data.totalAmount;
        this.taxAmount = data.taxAmount;
        this.grandTotal = data.grandTotal;
        this.receiptFileName = data.receiptFileName;
        this.saleDetails = data.saleDetails || [];
    }
}
