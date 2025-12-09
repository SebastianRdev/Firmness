import React, { useEffect, useState } from 'react';
import { Typography, Row, Col, Spin, Alert, Input, Button } from 'antd';
import ProductCard from '../components/ProductCard';
import productService from '../services/productService';
import categoryService from '../services/categoryService';

const { Title, Text } = Typography;
const { Search } = Input;

const Products = () => {
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
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
            setError(null);
        } catch (err) {
            setError('Failed to load products. Please try again later.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const fetchCategories = async () => {
        try {
            const data = await categoryService.getAll();
            console.log('Categories response:', data); // Debugging
            if (Array.isArray(data)) {
                setCategories(data);
            } else {
                console.error('Categories data is not an array:', data);
                setCategories([]);
            }
        } catch (err) {
            console.error('Failed to load categories', err);
            setCategories([]);
        }
    };

    useEffect(() => {
        fetchProducts();
        fetchCategories();
    }, []);

    const onSearch = (value) => {
        fetchProducts(value);
    };

    return (
        <div>
            {/* Header Section */}
            <div style={{ marginBottom: '32px' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', marginBottom: '16px' }}>
                    <div>
                        <Text type="secondary" style={{ fontSize: '14px' }}>Home / Catalog / <span style={{ color: '#1f1f1f', fontWeight: 600 }}>All Products</span></Text>
                        <Title level={1} style={{ margin: '8px 0 0', fontSize: '36px' }}>Catalog</Title>
                        <Text type="secondary" style={{ fontSize: '16px' }}>Browse our selection of professional-grade construction supplies.</Text>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                        <span style={{ color: '#666' }}>Sort by:</span>
                        <select style={{ padding: '8px 12px', borderRadius: '6px', border: '1px solid #d9d9d9', background: '#fff', fontSize: '14px', minWidth: '160px' }}>
                            <option>Price (Low to High)</option>
                            <option>Price (High to Low)</option>
                            <option>Newest Arrivals</option>
                        </select>
                    </div>
                </div>

                <Search
                    placeholder="Search by product name or keyword"
                    onSearch={onSearch}
                    style={{ width: '100%', maxWidth: '100%' }}
                    size="large"
                    allowClear
                    enterButton={null}
                    prefix={<span style={{ fontSize: '18px', color: '#999', marginRight: '8px' }}>🔍</span>}
                />
            </div>

            <Row gutter={32}>
                {/* Sidebar Filters */}
                <Col xs={24} lg={6}>
                    <div style={{ background: '#fff', padding: '24px', borderRadius: '12px', boxShadow: '0 2px 8px rgba(0,0,0,0.05)' }}>
                        <Title level={4} style={{ marginTop: 0, marginBottom: '24px' }}>Filter Products</Title>

                        <div style={{ marginBottom: '24px' }}>
                            <Text strong style={{ display: 'block', marginBottom: '12px' }}>Category</Text>
                            <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                {categories.map(category => (
                                    <label key={category.id} style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
                                        <input type="checkbox" style={{ accentColor: '#ffc107' }} /> {category.name}
                                    </label>
                                ))}
                                {categories.length === 0 && <Text type="secondary">No categories found</Text>}
                            </div>
                        </div>

                        <div style={{ marginBottom: '24px' }}>
                            <Text strong style={{ display: 'block', marginBottom: '12px' }}>Price Range</Text>
                            <input type="range" style={{ width: '100%', accentColor: '#ffc107' }} />
                            <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '8px', fontSize: '12px', color: '#666' }}>
                                <span>$0</span>
                                <span>$5000+</span>
                            </div>
                        </div>

                        <div style={{ marginBottom: '24px' }}>
                            <Text strong style={{ display: 'block', marginBottom: '12px' }}>Type</Text>
                            <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                <label style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
                                    <input type="radio" name="type" defaultChecked style={{ accentColor: '#ffc107' }} /> Supplies for Sale
                                </label>
                                <label style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
                                    <input type="radio" name="type" style={{ accentColor: '#ffc107' }} /> Vehicles for Rent
                                </label>
                            </div>
                        </div>

                        <Button type="primary" block style={{ background: '#ffc107', borderColor: '#ffc107', color: '#1f1f1f', fontWeight: 600, marginBottom: '12px' }}>
                            Apply Filters
                        </Button>
                        <Button block>Clear All</Button>
                    </div>
                </Col>

                {/* Product Grid */}
                <Col xs={24} lg={18}>
                    {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 24 }} />}

                    {loading ? (
                        <div style={{ textAlign: 'center', padding: 50 }}>
                            <Spin size="large" />
                        </div>
                    ) : (
                        <Row gutter={[24, 24]}>
                            {products.map((product) => (
                                <Col key={product.id} xs={24} sm={12} md={8} lg={8} xl={8}>
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
                </Col>
            </Row>
        </div>
    );
};

export default Products;
