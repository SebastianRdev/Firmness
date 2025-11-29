import React from 'react';
import { Card, Button, Typography, Tag, Space } from 'antd';
import { ShoppingCartOutlined } from '@ant-design/icons';
import { useCart } from '../context/CartContext';

const { Meta } = Card;
const { Text } = Typography;

const ProductCard = ({ product }) => {
    const { addToCart } = useCart();

    return (
        <Card
            hoverable
            style={{ width: 300, display: 'flex', flexDirection: 'column', height: '100%' }}
            cover={
                <div style={{ height: 200, background: '#f5f5f5', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#ccc' }}>
                    {/* Placeholder for image if not available in DTO */}
                    <span>No Image</span>
                </div>
            }
            actions={[
                <Button
                    type="primary"
                    icon={<ShoppingCartOutlined />}
                    onClick={() => addToCart(product)}
                    disabled={product.stock <= 0}
                >
                    {product.stock > 0 ? 'Add to Cart' : 'Out of Stock'}
                </Button>
            ]}
        >
            <Meta
                title={product.name}
                description={
                    <div style={{ height: 60, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                        {product.description}
                    </div>
                }
            />
            <div style={{ marginTop: 16 }}>
                <Space direction="vertical" style={{ width: '100%' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <Text strong style={{ fontSize: 18 }}>${product.price.toFixed(2)}</Text>
                        <Tag color="blue">{product.category}</Tag>
                    </div>
                    <Text type="secondary" style={{ fontSize: 12 }}>Code: {product.code}</Text>
                    <Text type={product.stock > 0 ? 'success' : 'danger'}>
                        {product.stock > 0 ? `${product.stock} in stock` : 'Out of stock'}
                    </Text>
                </Space>
            </div>
        </Card>
    );
};

export default ProductCard;
