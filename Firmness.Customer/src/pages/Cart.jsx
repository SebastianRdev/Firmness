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
                <Space direction="vertical" size={0}>
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
                        <Space direction="vertical">
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
            <Title level={2} style={{ marginBottom: 24 }}>Shopping Cart</Title>

            <Row gutter={24}>
                <Col xs={24} lg={16}>
                    <Table
                        columns={columns}
                        dataSource={cartItems}
                        rowKey="id"
                        pagination={false}
                        style={{ marginBottom: 24 }}
                    />
                    <Button onClick={clearCart}>Clear Cart</Button>
                </Col>

                <Col xs={24} lg={8}>
                    <Card title="Order Summary">
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
                            <Text>Subtotal</Text>
                            <Text strong>${getCartTotal().toFixed(2)}</Text>
                        </div>
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
                            <Text>Tax (0%)</Text>
                            <Text strong>$0.00</Text>
                        </div>
                        <div style={{ borderTop: '1px solid #f0f0f0', margin: '16px 0' }} />
                        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 24 }}>
                            <Title level={4}>Total</Title>
                            <Title level={4}>${getCartTotal().toFixed(2)}</Title>
                        </div>

                        <Button type="primary" block size="large" onClick={() => navigate('/checkout')}>
                            Proceed to Checkout
                        </Button>
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default Cart;
