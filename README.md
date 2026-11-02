# Task Management System

A web app for teams to create, assign and track tasks, with separate dashboards for admins and regular users. Built with ASP.NET Core MVC on .NET 8.

## Features

- **Tasks**: title, description, due date, category, priority (Low to Urgent) and status (Pending, In Progress, Completed, Cancelled). Tasks can be assigned to another user and discussed in comments.
- **User area**: personal dashboard, "my tasks" with filters, create and edit tasks, profile page.
- **Admin area**: manage users and roles, categories and all tasks, plus a reports page and app settings.
- **Sign-in**: email and password (hashed with BCrypt) or Google sign-in.
- **Password reset**: one-time code sent by email, then a reset form.

## Tech stack

- ASP.NET Core MVC (.NET 8), Razor views
- Entity Framework Core with SQL Server
- Cookie and JWT authentication, Google OAuth
- Layered structure: Controllers → Services → Repositories, with DTOs between layers

## Project structure

```text
Controllers/     MVC controllers (Admin, Auth, User, Category, ForgotPassword)
Services/        business logic
Repositories/    data access with EF Core
Interfaces/      service and repository contracts
Dtos/            request and response models
Models/          EF Core entities
Views/           Razor views for the admin and user areas
wwwroot/         styles and scripts
```

## Running it locally

Requirements: .NET 8 SDK and SQL Server (LocalDB works).

1. Create `appsettings.json` with a `DefaultConnection` connection string, plus `Google:ClientId` and `Google:ClientSecret` if you want Google sign-in, and your SMTP settings for password reset emails.
2. Apply the migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the app:
   ```bash
   dotnet run
   ```

## Author

Built by [Hussein Najdi](https://github.com/najdihussein2025).
