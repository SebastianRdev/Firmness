import React, { useState } from 'react';
import { Form, Input, Button, Card, Typography, Alert, message } from 'antd';
import { UserOutlined, LockOutlined, MailOutlined } from '@ant-design/icons';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const { Title, Text } = Typography;

const Register = () => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const { register } = useAuth();
    const navigate = useNavigate();

    const onFinish = async (values) => {
        setLoading(true);
        setError('');
        try {
            await register(values.firstName, values.lastName, values.email, values.password, values.confirmPassword);
            message.success('Registration successful!');
            navigate('/products');
        } catch (err) {
            // Handle array of errors from backend Identity
            if (Array.isArray(err.response?.data)) {
                setError(err.response.data.map(e => e.description).join(', '));
            } else {
                setError(err.response?.data?.message || 'Failed to register');
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ display: 'flex', minHeight: '100vh', background: '#fff' }}>
            {/* Left Side - Image/Brand */}
            <div style={{
                flex: 1,
                background: 'linear-gradient(rgba(0, 33, 64, 0.8), rgba(0, 33, 64, 0.8)), url("https://images.unsplash.com/photo-1503387762-592deb58ef4e?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80")',
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
                <div style={{ width: '100%', maxWidth: '480px' }}>
                    <div style={{ marginBottom: '32px' }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '24px' }}>
                            {/* Placeholder Logo Icon */}
                            <div style={{ width: 32, height: 32, background: '#1890ff', borderRadius: 4 }}></div>
                            <span style={{ fontSize: '24px', fontWeight: 'bold', color: '#1f1f1f' }}>ConstructGo</span>
                        </div>
                        <h2 style={{ fontSize: '32px', fontWeight: 'bold', marginBottom: '8px' }}>Create Your Account</h2>
                        <Text type="secondary" style={{ fontSize: '16px' }}>Access construction supplies and heavy equipment rentals.</Text>
                    </div>

                    {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 24 }} />}

                    <Form
                        name="register"
                        onFinish={onFinish}
                        layout="vertical"
                        size="large"
                    >
                        <div style={{ display: 'flex', gap: 16 }}>
                            <Form.Item
                                name="firstName"
                                label={<span style={{ fontWeight: 500 }}>Full Name</span>}
                                style={{ flex: 1 }}
                                rules={[{ required: true, message: 'Please input your First Name!' }]}
                            >
                                <Input placeholder="John" style={{ borderRadius: '8px' }} />
                            </Form.Item>
                            <Form.Item
                                name="lastName"
                                label={<span style={{ fontWeight: 500 }}>&nbsp;</span>}
                                style={{ flex: 1 }}
                                rules={[{ required: true, message: 'Please input your Last Name!' }]}
                            >
                                <Input placeholder="Doe" style={{ borderRadius: '8px' }} />
                            </Form.Item>
                        </div>

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
                            rules={[{ required: true, message: 'Please input your Password!' }, { min: 6, message: 'Password must be at least 6 characters' }]}
                        >
                            <Input.Password placeholder="••••••••••" style={{ borderRadius: '8px' }} />
                        </Form.Item>

                        <Form.Item
                            name="confirmPassword"
                            label={<span style={{ fontWeight: 500 }}>Confirm Password</span>}
                            dependencies={['password']}
                            rules={[
                                { required: true, message: 'Please confirm your password!' },
                                ({ getFieldValue }) => ({
                                    validator(_, value) {
                                        if (!value || getFieldValue('password') === value) {
                                            return Promise.resolve();
                                        }
                                        return Promise.reject(new Error('The two passwords that you entered do not match!'));
                                    },
                                }),
                            ]}
                        >
                            <Input.Password placeholder="••••••••••" style={{ borderRadius: '8px' }} />
                        </Form.Item>

                        <Form.Item style={{ marginTop: '24px' }}>
                            <Button type="primary" htmlType="submit" block size="large" loading={loading} style={{ height: '48px', borderRadius: '8px', fontSize: '16px', fontWeight: 600, background: '#1890ff' }}>
                                Create Account
                            </Button>
                        </Form.Item>

                        <div style={{ textAlign: 'center' }}>
                            <Text style={{ color: '#666' }}>Already have an account? <Link to="/login" style={{ fontWeight: 600 }}>Log In</Link></Text>
                        </div>
                    </Form>
                </div>
            </div>
        </div>
    );
};

export default Register;
