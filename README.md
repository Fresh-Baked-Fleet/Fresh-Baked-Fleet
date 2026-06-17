# 🍞 Fresh Baked Fleet

A full-stack, dynamic e-commerce and inventory management web application built for a bakery. Fresh Baked Fleet provides a seamless storefront for customers to browse goods, alongside a secure, robust administrative backend for bakery managers to control inventory and process orders.

## 🚀 Tech Stack

- **Frontend:** Blazor Web App (Interactive Server), HTML5, CSS3, Bootstrap 5
- **Backend:** C#, ASP.NET Core 8
- **Database:** PostgreSQL, Entity Framework Core (EF Core)
- **Authentication:** ASP.NET Core Identity (Role-based access control)

## ✨ Key Features

### Customer Storefront

- **Product Catalog:** Browse a dynamic list of fresh bakery items, complete with images, pricing, and availability.
- **User Accounts:** Secure registration and login for customers to save preferences.
- **Ordering System (_Merging Soon!_):** A complete cart and checkout flow allowing customers to place and track their bakery orders.

### Admin Dashboard (Inventory Management)

- **Role-Based Security:** Protected administrative routes that require an "Admin" or "Manager" role to access.
- **CRUD Operations:** Create, Read, Update, and Delete inventory items seamlessly.
- **Image Handling:** Direct image uploads converted and stored as highly optimized Base64 Data URIs.
- **Resilient Architecture:** Global error boundaries and database failure nets to ensure data integrity and prevent system crashes.

## 🛠️ Getting Started

Follow these steps to run the application on your local machine for development and testing.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) Running on Render

### Installation

1. **Clone the repository**
   ```bash
   git clone [https://github.com/your-repo/fresh-baked-fleet.git](https://github.com/your-repo/fresh-baked-fleet.git)
   cd fresh-baked-fleet/BlazorApp
   ```
