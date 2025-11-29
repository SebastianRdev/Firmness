import React from 'react';
import { Layout, Menu, Button, Avatar, Dropdown, Space } from 'antd';
import { UserOutlined, ShoppingCartOutlined, LogoutOutlined } from '@ant-design/icons';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const { Header, Content, Footer } = Layout;

const MainLayout = () => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const userMenu = (
        <Menu
            items={[
                {
                    key: 'logout',
                    label: 'Logout',
                    icon: <LogoutOutlined />,
                    onClick: handleLogout,
                },
            ]}
        />
    );

    return (
        <Layout style={{ minHeight: '100vh' }}>
            <Header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0 24px', background: '#fff', boxShadow: '0 2px 8px #f0f1f2' }}>
                <div className="logo" style={{ fontSize: '20px', fontWeight: 'bold', color: '#1890ff' }}>
                    <Link to="/products" style={{ color: 'inherit' }}>Firmness</Link>
                </div>

                <div style={{ display: 'flex', alignItems: 'center', gap: '24px' }}>
                    <Menu mode="horizontal" style={{ borderBottom: 'none', minWidth: '300px' }} defaultSelectedKeys={['products']}>
                        <Menu.Item key="products">
                            <Link to="/products">Products</Link>
                        </Menu.Item>
                        <Menu.Item key="cart">
                            <Link to="/cart">
                                <Space>
                                    <ShoppingCartOutlined />
                                    Cart
                                </Space>
                            </Link>
                        </Menu.Item>
                    </Menu>

                    {user ? (
                        <Dropdown overlay={userMenu} placement="bottomRight">
                            <Space style={{ cursor: 'pointer' }}>
                                <Avatar icon={<UserOutlined />} style={{ backgroundColor: '#1890ff' }} />
                                <span style={{ fontWeight: 500 }}>{user.fullName || user.email}</span>
                            </Space>
                        </Dropdown>
                    ) : (
                        <Space>
                            <Link to="/login">
                                <Button type="text">Login</Button>
                            </Link>
                            <Link to="/register">
                                <Button type="primary">Register</Button>
                            </Link>
                        </Space>
                    )}
                </div>
            </Header>

            <Content style={{ padding: '24px 50px', background: '#f0f2f5' }}>
                <div style={{ background: '#fff', padding: 24, minHeight: 380, borderRadius: 8 }}>
                    <Outlet />
                </div>
            </Content>

            <Footer style={{ textAlign: 'center' }}>Firmness ©2025 Created by Antigravity</Footer>
        </Layout>
    );
};

export default MainLayout;
