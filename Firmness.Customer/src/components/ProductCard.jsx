import React from 'react';
import { Card, Button, Typography } from 'antd';
import { ShoppingCartOutlined } from '@ant-design/icons';
import { useCart } from '../context/CartContext';

const { Text } = Typography;

const ProductCard = ({ product }) => {
    const { addToCart } = useCart();

    return (
        <Card
            hoverable
            style={{ width: '100%', display: 'flex', flexDirection: 'column', height: '100%', borderRadius: '12px', overflow: 'hidden', border: 'none', boxShadow: '0 4px 12px rgba(0,0,0,0.05)' }}
            styles={{ body: { padding: '20px', flex: 1, display: 'flex', flexDirection: 'column' } }}
            cover={
                <div style={{ height: 220, background: '#f5f7fa', display: 'flex', alignItems: 'center', justifyContent: 'center', position: 'relative' }}>
                    {/* Placeholder Image */}
                    <img
                        src="https://images.unsplash.com/photo-1504148455328-c376907d081c?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80"
                        alt={product.name}
                        style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                    />
                    {product.stock <= 0 && (
                        <div style={{ position: 'absolute', top: 10, right: 10, background: '#ff4d4f', color: 'white', padding: '4px 12px', borderRadius: '4px', fontSize: '12px', fontWeight: 'bold' }}>
                            Out of Stock
                        </div>
                    )}
                </div>
            }
        >
            <div style={{ marginBottom: '12px' }}>
                <Text strong style={{ fontSize: '18px', display: 'block', marginBottom: '4px', lineHeight: 1.3 }}>{product.name}</Text>
                <Text type="secondary" style={{ fontSize: '14px', height: '42px', overflow: 'hidden', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical' }}>
                    {product.description}
                </Text>
            </div>

            <div style={{ marginTop: 'auto' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
                    <Text strong style={{ fontSize: '24px', color: '#1f1f1f' }}>${product.price.toFixed(2)}</Text>
                </div>

                <Button
                    type="primary"
                    block
                    icon={<ShoppingCartOutlined />}
                    onClick={() => {
                        addToCart(product);
                        const btn = document.getElementById(`btn-${product.id}`);
                        if (btn) {
                            btn.style.transform = 'scale(0.95)';
                            setTimeout(() => btn.style.transform = 'scale(1)', 100);
                        }
                    }}
                    id={`btn-${product.id}`}
                    disabled={product.stock <= 0}
                    style={{
                        background: '#ffc107',
                        borderColor: '#ffc107',
                        color: '#1f1f1f',
                        fontWeight: 600,
                        height: '40px',
                        borderRadius: '8px',
                        transition: 'all 0.2s ease'
                    }}
                >
                    Add to Cart
                </Button>
            </div>
        </Card>
    );
};

export default ProductCard;
