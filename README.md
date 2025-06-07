# 📁 Document Manager - Backend

<p align="center">
  <img src="https://github.com/user-attachments/assets/a9327df4-2617-49de-8707-3259cdee98b8" alt="Document Manager Logo" />
</p>

## 📝 Overview

**Document Manager** is a powerful backend system built with .NET 8. It handles document uploads, user management, role-based access control, and authentication using JWT. It provides secure, RESTful endpoints for managing employee documents and profiles.

## 🚀 Features

- 🔐 **Authentication & Authorization**
  - JWT-based authentication
  - Permission management

- 👥 **User Management**
  - User CRUD operations
  - Profile image handling
  - Password management
  - Role assignment

- 📄 **Document Management**
  - Document upload/download
  - Update Document Information
  - Delete Document

## 🗂️ ER Diagram

<div align="center">
  <img src="https://res.cloudinary.com/dgfwfpxvg/image/upload/v1749278509/k876pspwni4t4ufzr0dh.png">
</div>

## 🔄 Data Flow

<div align="center">
  <img src="https://res.cloudinary.com/dgfwfpxvg/image/upload/v1748796571/dataflow_bi4jqx.png">
</div>

1. **🔑 Authentication Flow**
   ```
   Client → Login Request → Validate Credentials → Generate JWT → Return Token
   ```

2. **📁 Document Management Flow**
   ```
   Upload: Client → Validate Token → Save File → Store Cloudinary → Return Success
   Download: Client → Validate Token → Serve File
   ```

3. **👤 User Management Flow**
   ```
   Create/Update: Client → Validate Input → Hash Password → Store Data → Return User
   ```

## ⚙️ Installation

### Prerequisites

- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### 🔧 Setup Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/erisk405/Server-DocumentManager.git
   cd Server-DocumentManager
   ```

2. **Configure Database**
   - Update connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
        "PostgresConnection": "Host=localhost;Port=5432;Database=YouDB;Username=postgres;Password=yourPassword",
        "SqlServerConnection": "Server=localhost\\SQLEXPRESS;Database=DB-HRM;Trusted_Connection=True;TrustServerCertificate=True;"
     },
   }
   ```

3. **Apply Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Configure JWT Settings & Cloudinary**
   - Update JWT settings & Cloudinary in `appsettings.json`:
   ```json
   {
       "JwtSettings": {
         "Key": "your-secret-key",
         "Issuer": "your-issuer",
         "Audience": "your-audience",
         "ExpireMinutes": 60
       },
        "Cloudinary": {
         "CloudName": "YourCloudName",
         "ApiKey": "Apikey",
         "ApiSecret": "Your secret"
       }
   }
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

### 🐳 Docker Deployment

1. **Build the Docker Image**
   ```bash
   docker build -t hrm-api .
   ```

2. **Run the Container**
   ```bash
   docker run -p 8080:80 hrm-api
   ```

## 📚 API Documentation

Once running, access the Swagger documentation at:
```
http://localhost:5048/swagger
```

## 🏗️ Project Structure

```
HRM-API/
├── Controllers/     # API endpoints
├── Models/          # Data models
├── DTOs/           # Data transfer objects
├── Repository/     # Data access layer
├── Migrations/     # Database migrations
└── wwwroot/        # Static files
    ├── uploads/    # Document storage
    └── profiles/   # Profile images
```
