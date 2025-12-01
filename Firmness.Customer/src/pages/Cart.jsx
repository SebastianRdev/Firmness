import React from 'react';
import { Typography, Table, Button, InputNumber, Card, Row, Col, Space, Empty } from 'antd';
import { DeleteOutlined, ShoppingOutlined } from '@ant-design/icons';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';

const { Title, Text } = Typography;

const Cart = () => {
    const { cartItems, removeFromCart, updateQuantity, getCartTotal, clearCart } = useCart();
    const navigate = useNavigate();

    const columns = [
        {
            title: 'Product',
            dataIndex: 'name',
            key: 'name',
            render: (text, record) => (
                <Space orientation="vertical" size={0}>
                    <Text strong>{text}</Text>
                    <Text type="secondary" style={{ fontSize: 12 }}>Code: {record.code}</Text>
                </Space>
            ),
        },
        {
            title: 'Price',
            dataIndex: 'price',
            key: 'price',
            render: (price) => `$${price.toFixed(2)}`,
        },
        {
            title: 'Quantity',
            key: 'quantity',
            render: (_, record) => (
                <InputNumber
                    min={1}
                    max={record.stock}
                    value={record.quantity}
                    onChange={(value) => updateQuantity(record.id, value)}
                />
            ),
        },
        {
            title: 'Total',
            key: 'total',
            render: (_, record) => `$${(record.price * record.quantity).toFixed(2)}`,
        },
        {
            title: 'Action',
            key: 'action',
            render: (_, record) => (
                <Button
                    type="text"
                    danger
                    icon={<DeleteOutlined />}
                    onClick={() => removeFromCart(record.id)}
                />
            ),
        },
    ];

    if (cartItems.length === 0) {
        return (
            <div style={{ textAlign: 'center', padding: '50px 0' }}>
                <Empty
                    image={Empty.PRESENTED_IMAGE_SIMPLE}
                    description={
                        <Space orientation="vertical">
                            <Text type="secondary">Your cart is empty</Text>
                            <Link to="/products">
                                <Button type="primary" icon={<ShoppingOutlined />}>
                                    Go Shopping
                                </Button>
                            </Link>
                        </Space>
                    }
                />
            </div>
        );
    }

    return (
        <div>
            <Title level={1} style={{ marginBottom: '32px', fontSize: '36px' }}>Your Shopping Cart</Title>

            <Row gutter={32}>
                <Col xs={24} lg={16}>
                    <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 2px 8px rgba(0,0,0,0.05)', overflow: 'hidden' }}>
                        <Table
                            columns={columns}
                            dataSource={cartItems}
                            rowKey="id"
                            pagination={false}
                            style={{ width: '100%' }}
                        />
                    </div>
                    <div style={{ marginTop: '24px', textAlign: 'right' }}>
                        <Button onClick={clearCart} danger type="text">Clear Cart</Button>
                    </div>
                </Col>

                <Col xs={24} lg={8}>
                    <Card
                        title={<span style={{ fontSize: '20px', fontWeight: 'bold' }}>Order Summary</span>}
                        style={{ borderRadius: '12px', boxShadow: '0 2px 8px rgba(0,0,0,0.05)' }}
                        styles={{
                            header: { borderBottom: '1px solid #f0f0f0', padding: '24px' },
                            body: { padding: '24px' }
                        }}
                    >
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
                            <Text type="secondary">Subtotal</Text>
                            <Text strong>${getCartTotal().toFixed(2)}</Text>
                        </div>
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
                            <Text type="secondary">Estimated Taxes (5%)</Text>
                            <Text strong>${(getCartTotal() * 0.05).toFixed(2)}</Text>
                        </div>
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
                            <Text type="secondary">Delivery Fees</Text>
                            <Text strong>$75.00</Text>
                        </div>
                        <div style={{ borderTop: '1px solid #f0f0f0', margin: '24px 0' }} />
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 32, alignItems: 'center' }}>
                            <Title level={4} style={{ margin: 0 }}>Grand Total</Title>
                            <Title level={3} style={{ margin: 0 }}>${(getCartTotal() * 1.05 + 75).toFixed(2)}</Title>
                        </div>

                        <Button
                            type="primary"
                            block
                            size="large"
                            onClick={() => navigate('/checkout')}
                            style={{
                                height: '48px',
                                background: '#1e3a8a', /* Darker blue for checkout button */
                                borderColor: '#1e3a8a',
                                borderRadius: '8px',
                                fontSize: '16px',
                                fontWeight: 600
                            }}
                        >
                            Proceed to Checkout
                        </Button>
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default Cart;
