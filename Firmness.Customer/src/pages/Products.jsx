import React, { useEffect, useState } from 'react';
import { Typography, Row, Col, Spin, Alert, Input } from 'antd';
import ProductCard from '../components/ProductCard';
import productService from '../services/productService';

const { Title } = Typography;
const { Search } = Input;

const Products = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const fetchProducts = async (searchTerm = '') => {
        setLoading(true);
        try {
            let data;
            if (searchTerm) {
                data = await productService.search(searchTerm);
            } else {
                data = await productService.getAll();
            }
            setProducts(data);
        } catch (err) {
            setError('Failed to load products. Please try again later.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProducts();
    }, []);

    const onSearch = (value) => {
        fetchProducts(value);
    };

    return (
        <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
                <Title level={2} style={{ margin: 0 }}>Products Catalog</Title>
                <Search placeholder="Search products..." onSearch={onSearch} style={{ width: 300 }} allowClear />
            </div>

            {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 24 }} />}

            {loading ? (
                <div style={{ textAlign: 'center', padding: 50 }}>
                    <Spin size="large" />
                </div>
            ) : (
                <Row gutter={[24, 24]}>
                    {products.map((product) => (
                        <Col key={product.id} xs={24} sm={12} md={8} lg={6} xl={6}>
                            <ProductCard product={product} />
                        </Col>
                    ))}
                    {products.length === 0 && !error && (
                        <Col span={24}>
                            <div style={{ textAlign: 'center', padding: 50, color: '#999' }}>
                                No products found.
                            </div>
                        </Col>
                    )}
                </Row>
            )}
        </div>
    );
};

export default Products;
