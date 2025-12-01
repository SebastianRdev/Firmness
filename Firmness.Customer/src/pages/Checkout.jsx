import React, { useState } from 'react';
import { Typography, Card, Button, List, Divider, message, Result, Spin, Row, Col } from 'antd';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import customerSaleApi from '../services/api/customerSaleApi';
import { CreateSaleRequest } from '../domain/dto/SaleDto';
import { useNavigate } from 'react-router-dom';

const { Title, Text } = Typography;

const Checkout = () => {
    const { cartItems, getCartTotal, clearCart } = useCart();
    const { user } = useAuth();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [saleInfo, setSaleInfo] = useState(null);

    const total = getCartTotal();
    const taxAmount = total * 0.19; // 19% IVA
    const grandTotal = total + taxAmount;

    const handlePurchase = async () => {
        if (!user || !user.id) {
            message.error('Información de usuario faltante. Por favor, inicia sesión nuevamente.');
            return;
        }

        if (cartItems.length === 0) {
            message.warning('Tu carrito está vacío');
            return;
        }

        setLoading(true);
        try {
            // Create sale request using DTO
            const saleRequest = new CreateSaleRequest(
                user.id,
                cartItems,
                {
                    subtotal: total,
                    taxes: taxAmount,
                    grandTotal: grandTotal
                }
            );

            // Call API
            const response = await customerSaleApi.createSale(saleRequest);

            // Success!
            message.success('¡Compra realizada exitosamente! Tu comprobante ha sido enviado a tu correo.');
            setSaleInfo(response);
            setSuccess(true);
            clearCart();

        } catch (err) {
            console.error('Purchase error:', err);

            // Display user-friendly error message
            const errorMessage = err.message || 'Error al completar la compra. Por favor, intenta nuevamente.';
            message.error(errorMessage, 5); // Show for 5 seconds

        } finally {
            setLoading(false);
        }
    };

    if (success) {
        return (
            <Result
                status="success"
                title="¡Compra Realizada Exitosamente!"
                subTitle={`Tu comprobante ha sido enviado a ${user?.email}. Recibirás un correo con el PDF adjunto en unos momentos.`}
                extra={[
                    <Button type="primary" key="products" onClick={() => navigate('/products')}>
                        Seguir Comprando
                    </Button>,
                    <Button key="home" onClick={() => navigate('/')}>
                        Ir al Inicio
                    </Button>,
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
