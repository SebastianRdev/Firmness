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
            <Header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0 40px', background: '#fff', boxShadow: '0 2px 8px rgba(0,0,0,0.05)', height: '80px' }}>
                {/* Logo */}
                <div className="logo" style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                    <div style={{ width: 32, height: 32, background: '#ffc107', borderRadius: 4, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <div style={{ width: 16, height: 16, border: '2px solid #fff', borderRadius: 2 }}></div>
                    </div>
                    <Link to="/products" style={{ fontSize: '24px', fontWeight: 'bold', color: '#1f1f1f', textDecoration: 'none' }}>ConstructGo</Link>
                </div>

                {/* Navigation */}
                <div style={{ flex: 1, display: 'flex', justifyContent: 'center' }}>
                    <Menu
                        mode="horizontal"
                        style={{ borderBottom: 'none', background: 'transparent', width: '100%', justifyContent: 'center', display: 'flex' }}
                        defaultSelectedKeys={['products']}
                        items={[
                            {
                                key: 'products',
                                label: <Link to="/products" style={{ fontWeight: 600, fontSize: '16px' }}>Catalog</Link>,
                            },
                            {
                                key: 'cart',
                                label: <Link to="/cart" style={{ fontWeight: 600, fontSize: '16px' }}>Cart</Link>,
                            },
                        ]}
                    />
                </div>

                {/* User Profile */}
                <div style={{ display: 'flex', alignItems: 'center', gap: '24px' }}>
                    {user ? (
                        <Dropdown
                            menu={{
                                items: [
                                    {
                                        key: 'logout',
                                        label: 'Logout',
                                        onClick: handleLogout,
                                        danger: true
                                    }
                                ]
                            }}
                            placement="bottomRight"
                        >
                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px', cursor: 'pointer' }}>
                                <span style={{ color: '#666' }}>Welcome, <span style={{ color: '#1f1f1f', fontWeight: 600 }}>{user.fullName || user.email}</span></span>
                                <div style={{ width: 40, height: 40, background: '#ffc107', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff', fontWeight: 'bold', fontSize: '18px' }}>
                                    {(user.fullName || user.email)[0].toUpperCase()}
                                </div>
                            </div>
                        </Dropdown>
                    ) : (
                        <Space>
                            <Link to="/login">
                                <Button type="text" style={{ fontWeight: 600 }}>Log In</Button>
                            </Link>
                            <Link to="/register">
                                <Button type="primary" style={{ background: '#1890ff', fontWeight: 600 }}>Sign Up</Button>
                            </Link>
                        </Space>
                    )}
                </div>
            </Header>

            <Content style={{ padding: '40px', background: '#f5f7fa', flex: 1 }}>
                <div style={{ maxWidth: '1400px', margin: '0 auto', minHeight: 'calc(100vh - 80px - 70px - 80px)' }}>
                    <Outlet />
                </div>
            </Content>

            <Footer style={{ textAlign: 'center', background: '#f5f7fa', color: '#999' }}>ConstructGo ©2025 Created by Antigravity</Footer>
        </Layout>
    );
};

export default MainLayout;
