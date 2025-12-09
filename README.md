# Firmness

Firmness is a comprehensive web application designed for managing sales, inventory, and customer relationships. It features a robust backend built with .NET Core and a user-friendly frontend for both administration and customer interaction.

## Features

*   **Product Management:** Create, update, delete, and search for products. Manage categories and stock levels.
*   **Customer Management:** Register and manage customer accounts. View customer details and purchase history.
*   **Sales & Orders:** Process sales, generate receipts (PDF), and track order status.
*   **Excel Import:** Bulk upload products and customers using Excel files with AI-powered column mapping correction.
*   **Authentication & Authorization:** Secure login and registration with role-based access control (Admin, Customer).
*   **Dashboard:** Overview of key metrics such as total products, customers, and sales.

## Technology Stack

*   **Backend:** .NET Core 8, Entity Framework Core, SQL Server (or compatible).
*   **Frontend (Web Admin):** ASP.NET Core MVC, Razor Views, Bootstrap/Tailwind CSS.
*   **Frontend (Customer App):** React (planned/integrated).
*   **Libraries:**
    *   `EPPlus`: For Excel file handling.
    *   `QuestPDF`: For generating PDF receipts.
    *   `AutoMapper`: For object-to-object mapping.
    *   `Microsoft.AspNetCore.Identity`: For user authentication and management.

## Getting Started

### Prerequisites

* .NET 8 SDK
* PostgreSQL (LocalDB o instancia completa)
* Node.js (para el frontend en React)
* Docker y Docker Compose (para levantar todo el entorno con contenedores)

### Configuration

Antes de correr la aplicación, debes crear un archivo `.env` en la raíz del proyecto y en `Firmness.Api`. Puedes ver `.env.example` como referencia para ver los valores requeridos.

Luego edita `.env` con las credenciales de tu base de datos y cualquier otra configuración necesaria.

### Running the Project

#### Local (sin Docker)

1. Aplica las migraciones a la base de datos:

```bash
dotnet ef database update --project Firmness.Infrastructure --startup-project Firmness.Api
```

2. Ejecuta la aplicación:

```bash
dotnet run --project Firmness.WebAdmin
```

#### Con Docker

Puedes levantar todo el entorno usando el script `start.sh` incluido en el proyecto:

```bash
./start.sh
```

Este script hace lo siguiente:

* Construye y levanta los contenedores definidos en `docker-compose.yml` (`API`, `WebAdmin`, `Customer App`).
* Muestra en consola las URLs para acceder a cada servicio:

```
📱 Customer App:   http://localhost:8083
💻 WebAdmin:       http://localhost:8082
🔌 API Swagger:    http://localhost:8081/index.html
```

## Screenshots

### Dashboard
![Dashboard Screenshot](docs/screenshots/dashboard.png)
*Overview of the admin dashboard showing key statistics.*

### Product Management
![Product List Screenshot](docs/screenshots/product_list.png)
*List of products with search and filter options.*

### Excel Import with AI Mapping
![Excel Import Screenshot](docs/screenshots/excel_import.png)
*Bulk upload interface showing AI-suggested column mapping.*

### PDF Receipt
![Receipt Screenshot](docs/screenshots/receipt_sample.png)
*Sample PDF receipt generated after a sale.*

## Architecture

The solution follows a Clean Architecture approach:

*   **Firmness.Domain:** Core entities and business logic.
*   **Firmness.Application:** Application services, DTOs, and interfaces.
*   **Firmness.Infrastructure:** Data access, external service implementations (Excel, PDF), and repositories.
*   **Firmness.Api:** RESTful API endpoints for the frontend.
*   **Firmness.WebAdmin:** MVC-based administration interface.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License.
