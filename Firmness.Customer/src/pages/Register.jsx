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
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', background: '#f0f2f5' }}>
            <Card style={{ width: 450, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}>
                <div style={{ textAlign: 'center', marginBottom: 24 }}>
                    <Title level={2}>Firmness</Title>
                    <Text type="secondary">Create a new account</Text>
                </div>

                {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

                <Form
                    name="register"
                    onFinish={onFinish}
                    layout="vertical"
                >
                    <div style={{ display: 'flex', gap: 16 }}>
                        <Form.Item
                            name="firstName"
                            style={{ flex: 1 }}
                            rules={[{ required: true, message: 'Please input your First Name!' }]}
                        >
                            <Input placeholder="First Name" size="large" />
                        </Form.Item>
                        <Form.Item
                            name="lastName"
                            style={{ flex: 1 }}
                            rules={[{ required: true, message: 'Please input your Last Name!' }]}
                        >
                            <Input placeholder="Last Name" size="large" />
                        </Form.Item>
                    </div>

                    <Form.Item
                        name="email"
                        rules={[{ required: true, message: 'Please input your Email!' }, { type: 'email', message: 'Please enter a valid email!' }]}
                    >
                        <Input prefix={<MailOutlined />} placeholder="Email" size="large" />
                    </Form.Item>

                    <Form.Item
                        name="password"
                        rules={[{ required: true, message: 'Please input your Password!' }, { min: 6, message: 'Password must be at least 6 characters' }]}
                    >
                        <Input.Password prefix={<LockOutlined />} placeholder="Password" size="large" />
                    </Form.Item>

                    <Form.Item
                        name="confirmPassword"
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
                        <Input.Password prefix={<LockOutlined />} placeholder="Confirm Password" size="large" />
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" block size="large" loading={loading}>
                            Register
                        </Button>
                    </Form.Item>

                    <div style={{ textAlign: 'center' }}>
                        <Text>Already have an account? <Link to="/login">Login now</Link></Text>
                    </div>
                </Form>
            </Card>
        </div>
    );
};

export default Register;
