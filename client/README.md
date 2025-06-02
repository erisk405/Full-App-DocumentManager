# 📁 Frontend - Document Manager

<p align="center">
  <img src="https://github.com/user-attachments/assets/a9327df4-2617-49de-8707-3259cdee98b8" alt="letgodoc" />
</p>

## Overview

This is the **frontend** project of the Document Manager system. It is built with **Angular** and connects to a backend API (developed with .NET 8) for handling document uploads, user management, authentication, and more.

## 🚀 Features

- 📄 Document upload/download
- 👤 User profile with role-based views
- 🔐 JWT authentication
- 🧩 Modular component-based architecture
- 🎨 Responsive UI

## 🛠️ Tech Stack

| Layer         | Technology            |
|---------------|------------------------|
| Frontend      | Angular 17+            |
| Styling       | Tailwind CSS, CSS, PrimeNG |
| UI Components | [PrimeNG](https://primeng.org/) |
| Communication | Angular `HttpClient`   |
| Backend API   | .NET 8                 |

## ⚙️ Prerequisites

Before running the frontend, make sure you have the following installed:

- [Node.js](https://nodejs.org/) (v18 or higher recommended)
- [Angular CLI](https://angular.io/cli) (Install via `npm install -g @angular/cli`)

## 📦 Installation

Clone the repository and install dependencies:

```bash
git clone https://github.com/erisk405/DocumentManager.git
npm install
```

## 🧪 Development Server
To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

🏗️ Build for Production
To build the project run:

```bash
ng build
```

📁 Code Structure
```
src/
├── app/
│   ├── components/
│   ├── pages/
│   ├── services/
│   ├── models/
│   └── app.module.ts
├── assets/
└── environments/
```

🔗 API Configuration
```
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api' // <-- your .NET backend
};
```


