# Book Management System

## 📖 Introduction
The **Book Management System** is a web-based API built using **ASP.NET Core Web API**. It allows users to manage books by performing CRUD operations such as adding, updating, deleting, and viewing books.

## 🚀 Features
- User authentication and authorization using **JWT**.
- Secure API endpoints with **ASP.NET Identity**.
- Manage books with **Create, Read, Update, and Delete (CRUD)** functionality.
- Database integration using **SQL Server**.
- Unit of Work and Repository Pattern for efficient data management.

## 🛠️ Technologies Used
- **Backend:** ASP.NET Core Web API (.NET 8)
- **Database:** SQL Server
- **Authentication:** ASP.NET Identity & JWT

## 🔧 Installation & Setup
### 1️⃣ Clone the Repository
```sh
https://github.com/miantalha45/BookManagmentSystem.git
```

### 2️⃣ Backend Setup (ASP.NET Core Web API)
1. Open the project in **Visual Studio 2022**.
2. Configure the database connection in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=your_server;Database=your_db;user=username;Password=password;Trusted_Connection=False;TrustServerCertificate=True;MultipleActiveResultSets=true;"
   }
   ```
3. Run the following command in package manager console to apply migrations:
   ```sh
   update-database
   ```
  
## 📧 Contact & Support
For any queries, feel free to reach out:
- **Email:** myown4500@gmail.com
- **GitHub Issues:** https://github.com/miantalha45/BookManagmentSystem/issues
- **LinkedIn:** https://www.linkedin.com/in/talha-amjad%F0%9F%87%B5%F0%9F%87%B8-0087ab2bb/
