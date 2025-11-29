# Firmness Customer Module

This is the frontend module for the Firmness application, built with React, Vite, and Ant Design.

## Features

- **Authentication**: JWT-based Login and Registration.
- **Product Catalog**: Browse and search products.
- **Shopping Cart**: Manage items and quantities.
- **Checkout**: Complete purchases and download receipts.
- **Responsive Design**: Built with Ant Design for a modern look.

## Prerequisites

- Node.js (v18+)
- .NET 8 SDK (for Backend)
- PostgreSQL (for Backend)

## Setup & Installation

1. **Backend Setup**
   - Navigate to `Firmness.Api` directory.
   - Configure `.env` with your database credentials.
   - Run `dotnet run`.

2. **Frontend Setup**
   - Navigate to `Firmness.Customer` directory.
   - Run `npm install`.
   - Run `npm run dev` to start the development server.

## Project Structure

- `src/components`: Reusable UI components.
- `src/pages`: Application views (Login, Register, Products, Cart, Checkout).
- `src/services`: API integration (Auth, Products, Sales).
- `src/context`: Global state management (Auth, Cart).
- `src/layouts`: Main application layout.

## Technologies

- React 18
- Vite
- Ant Design
- Axios
- React Router DOM
