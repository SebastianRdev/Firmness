import React, { useState } from 'react';
import { Form, Input, Button, Card, Typography, Alert, message } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const { Title, Text } = Typography;

const Login = () => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const { login } = useAuth();
    const navigate = useNavigate();

    const onFinish = async (values) => {
        setLoading(true);
        setError('');
        try {
            await login(values.email, values.password);
            message.success('Login successful!');
            navigate('/products');
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to login');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ display: 'flex', minHeight: '100vh', background: '#fff' }}>
            {/* Left Side - Image/Brand */}
            <div style={{
                flex: 1,
                background: 'linear-gradient(rgba(0, 33, 64, 0.8), rgba(0, 33, 64, 0.8)), url("https://images.unsplash.com/photo-1541888946425-d81bb19240f5?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80")',
                backgroundSize: 'cover',
                backgroundPosition: 'center',
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                padding: '60px',
                color: 'white'
            }}>
                <div style={{ maxWidth: '500px' }}>
                    <h1 style={{ fontSize: '48px', fontWeight: 'bold', marginBottom: '24px', color: 'white', lineHeight: 1.2 }}>
                        Build Your Future with Us
                    </h1>
                    <p style={{ fontSize: '18px', opacity: 0.9, lineHeight: 1.6 }}>
                        Get instant access to top-tier construction supplies and heavy equipment rentals. We are your reliable partner in every project, big or small.
                    </p>
                </div>
            </div>

            {/* Right Side - Form */}
            <div style={{
                flex: 1,
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                padding: '40px',
                background: '#fff'
            }}>
                <div style={{ width: '100%', maxWidth: '420px' }}>
                    <div style={{ marginBottom: '40px' }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '24px' }}>
                            {/* Placeholder Logo Icon */}
                            <div style={{ width: 32, height: 32, background: '#1890ff', borderRadius: 4 }}></div>
                            <span style={{ fontSize: '24px', fontWeight: 'bold', color: '#1f1f1f' }}>ConstructGo</span>
                        </div>
                        <h2 style={{ fontSize: '32px', fontWeight: 'bold', marginBottom: '8px' }}>Welcome Back</h2>
                        <Text type="secondary" style={{ fontSize: '16px' }}>Please login to your account.</Text>
                    </div>

                    {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 24 }} />}

                    <Form
                        name="login"
                        initialValues={{ remember: true }}
                        onFinish={onFinish}
                        layout="vertical"
                        size="large"
                    >
                        <Form.Item
                            name="email"
                            label={<span style={{ fontWeight: 500 }}>Email Address</span>}
                            rules={[{ required: true, message: 'Please input your Email!' }, { type: 'email', message: 'Please enter a valid email!' }]}
                        >
                            <Input placeholder="john.doe@constructgo.com" style={{ borderRadius: '8px' }} />
                        </Form.Item>

                        <Form.Item
                            name="password"
                            label={<span style={{ fontWeight: 500 }}>Password</span>}
                            rules={[{ required: true, message: 'Please input your Password!' }]}
                        >
                            <Input.Password placeholder="••••••••••" style={{ borderRadius: '8px' }} />
                        </Form.Item>

                        <Form.Item style={{ marginTop: '32px' }}>
                            <Button type="primary" htmlType="submit" block size="large" loading={loading} style={{ height: '48px', borderRadius: '8px', fontSize: '16px', fontWeight: 600, background: '#1890ff' }}>
                                Log in
                            </Button>
                        </Form.Item>

                        <div style={{ textAlign: 'center', marginTop: '24px' }}>
                            <Text style={{ color: '#666' }}>Don't have an account? <Link to="/register" style={{ fontWeight: 600 }}>Register now</Link></Text>
                        </div>
                    </Form>
                </div>
            </div>
        </div>
    );
};

export default Login;
