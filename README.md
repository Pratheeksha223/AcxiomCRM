# AcxiomCRM – Customer Relationship Management System

## Overview

AcxiomCRM is a web-based Customer Relationship Management (CRM) application developed using ASP.NET Core MVC. It helps businesses manage customer information, track leads, monitor sales opportunities, schedule follow-ups and maintain customer interaction records through a centralized platform.

## Features

- **Dashboard:** Overview of CRM activities and business information.
- **Customer Management:** Add, view, edit and delete customer records.
- **Lead Management:** Track and manage potential customers.
- **Opportunity Management:** Manage sales opportunities, deal values and stages.
- **Follow-up Management:** Schedule and track customer follow-ups.
- **Activity Management:** Record and manage customer interactions.
- **Reports:** View customer, lead, opportunity and follow-up statistics.
- **User Management:** Create users and assign Admin or Employee roles.
- **Audit Logs:** Maintain a history of important system actions.

## Technology Stack

| Component | Technology |
|---|---|
| Frontend | HTML, CSS, Bootstrap |
| Backend | C#, ASP.NET Core MVC |
| Database | SQLite |
| ORM | Entity Framework Core |
| Authentication | ASP.NET Core Identity |
| Icons | Bootstrap Icons |
| Development Environment | Visual Studio Code |

## Project Structure

- Controllers – Handles application requests and business logic.
- Models – Contains application data models.
- Views – Contains Razor views and user interface pages.
- Data – Database context and configuration.
- Migrations – Entity Framework Core database migrations.
- Services – Application services, including audit logging.
- wwwroot – Static files such as CSS and JavaScript.

## Installation and Setup

1. Clone the repository:

   `git clone YOUR_GITHUB_REPOSITORY_URL`

2. Open the project folder.

3. Restore dependencies:

   `dotnet restore`

4. Apply database migrations:

   `dotnet ef database update`

5. Run the application:

   `dotnet run`

6. Open the localhost URL displayed in the terminal.

## Project Objective

The main objective of AcxiomCRM is to simplify customer relationship management by providing an organized platform for managing customer data, sales activities, follow-ups, user roles and business reports.

## Future Enhancements

- Advanced analytics and visual dashboards.
- Email notifications and reminders.
- Enhanced role-based access control.
- Cloud deployment and integration.

---

Developed as a software development project using ASP.NET Core MVC.
