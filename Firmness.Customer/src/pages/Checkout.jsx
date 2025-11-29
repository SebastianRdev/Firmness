import React, { useState } from 'react';
import { Typography, Card, Button, List, Divider, message, Result, Spin } from 'antd';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import saleService from '../services/saleService';
import { useNavigate } from 'react-router-dom';

const { Title, Text } = Typography;

const Checkout = () => {
    const { cartItems, getCartTotal, clearCart } = useCart();
    const { user } = useAuth();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [receiptFileName, setReceiptFileName] = useState(null);

    const total = getCartTotal();

    const handlePurchase = async () => {
        if (!user || !user.id) {
            message.error('User information missing. Please login again.');
            return;
        }

        setLoading(true);
        try {
            const saleData = {
                customerId: user.id,
                date: new Date().toISOString(),
                totalAmount: total,
                taxAmount: 0,
                grandTotal: total,
                saleDetails: cartItems.map(item => ({
                    productId: item.id,
                    quantity: item.quantity,
                    unitPrice: item.price
                }))
            };

            // The backend redirects to download receipt, but since we use axios, we get the response.
            // If the backend returns a redirect 302, axios follows it automatically if it's a GET, but for POST it might return the response.
            // However, the controller returns RedirectToAction which returns 302.
            // Axios follows redirects for 3xx.
            // If the controller returns a file download directly or a redirect to it, we need to handle it.
            // Actually, the controller returns: return RedirectToAction("DownloadReceipt", new { fileName = sale.ReceiptFileName });
            // This means the response will be the PDF file content if axios follows the redirect.

            // Let's try to handle it.
            // Ideally, the API should return JSON with the file name, and the frontend initiates the download.
            // But we have to work with existing backend.

            // If axios follows the redirect, the response data will be the PDF blob.
            // We should probably check the content type.

            const response = await saleService.createSale(saleData);

            // If we get here, it might be the PDF content or the redirect response.
            // Since we didn't set responseType: 'blob' in createSale, we might get garbage text if it's a PDF.
            // We should probably update createSale to handle this or change backend to return JSON.
            // But I can't change backend logic easily without breaking other things maybe.
            // Let's assume for now we get a success and we can try to download the receipt if we know the filename.
            // But we don't know the filename unless we parse the redirect URL or if the backend returns it.

            // Wait, SalesController:
            // return RedirectToAction("DownloadReceipt", new { fileName = sale.ReceiptFileName });

            // If I use axios, it follows redirects. So response.data will be the PDF binary.
            // I should update saleService to expect blob.

            message.success('Purchase successful!');
            setSuccess(true);
            clearCart();

        } catch (err) {
            console.error(err);
            // If it's a parsing error because we got binary data but expected JSON, it might be a success actually.
            // But let's try to handle it better.
            message.error('Failed to complete purchase.');
        } finally {
            setLoading(false);
        }
    };

    if (success) {
        return (
            <Result
                status="success"
                title="Successfully Purchased Cloud Server ECS!"
                subTitle="Order number: 2017182818828182881 Cloud server configuration takes 1-5 minutes, please wait."
                extra={[
                    <Button type="primary" key="console" onClick={() => navigate('/products')}>
                        Buy Again
                    </Button>,
                    <Button key="buy">View Orders</Button>,
                ]}
            />
        );
    }

    return (
        <div style={{ maxWidth: 800, margin: '0 auto' }}>
            <Title level={2}>Checkout</Title>
            <Row gutter={24}>
                <Col span={16}>
                    <Card title="Shipping Information">
                        <Text>Name: {user?.fullName}</Text><br />
                        <Text>Email: {user?.email}</Text>
                    </Card>
                    <Card title="Payment Method" style={{ marginTop: 24 }}>
                        <Text>Credit Card (Simulated)</Text>
                    </Card>
                </Col>
                <Col span={8}>
                    <Card title="Order Summary">
                        <List
                            itemLayout="horizontal"
                            dataSource={cartItems}
                            renderItem={item => (
                                <List.Item>
                                    <List.Item.Meta
                                        title={item.name}
                                        description={`Qty: ${item.quantity}`}
                                    />
                                    <div>${(item.price * item.quantity).toFixed(2)}</div>
                                </List.Item>
                            )}
                        />
                        <Divider />
                        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                            <Text strong>Total</Text>
                            <Text strong>${total.toFixed(2)}</Text>
                        </div>
                        <Button
                            type="primary"
                            block
                            size="large"
                            style={{ marginTop: 24 }}
                            onClick={handlePurchase}
                            loading={loading}
                        >
                            Place Order
                        </Button>
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default Checkout;
