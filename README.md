# Supermarket Inventory Management System

A Windows Forms desktop application built in C# (.NET 8) for managing supermarket product inventory. Developed as a course project for CS123P — Object-Oriented Programming, Spring 2026, Mansoura University.

## Overview

The system allows store managers to add, edit, and delete products, distinguish between perishable and non-perishable items, monitor stock levels, and view inventory insights — all persisted locally to a file with no database required.

## Features

- Add, edit, and delete perishable and non-perishable products
- Real-time search across the product inventory
- Automatic low-stock and out-of-stock status based on quantity
- Inventory insights: total stock value, product counts, type distribution, top 10 by quantity
- File-based persistence — data is saved immediately on every change and loaded on startup
- Input validation to prevent negative quantities and missing required fields

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | .NET 8 |
| UI | Windows Forms |
| Persistence | Local flat file (pipe-delimited) |
| IDE | Visual Studio |

## Project Structure

```
OOP-Project/
├── Supermarket Inventory Management System/
│   └── Supermarket Inventory Management System/
│       ├── Program.cs
│       ├── Form1.cs
│       ├── Forms/
│       │   └── AddProductForm.cs
│       └── Models/
│           ├── Product.cs
│           ├── PerishableProduct.cs
│           ├── NonPerishableProduct.cs
│           ├── Inventory.cs
│           └── Enums.cs
└── docs/
    ├── README.md
    ├── architecture/
    │   ├── ARCHITECTURE_OVERVIEW.md
    │   ├── DOMAIN_MODEL.md
    │   └── FLOWS.md
    ├── assets/
    │   └── diagrams/
    │       └── CLASS_DIAGRAM.png
    └── data/
        ├── DATA_PERSISTENCE.md
        ├── FILE_FORMAT_SPEC.md
        └── DUMMY_PRODUCTS.txt
```

## Domain Model

```
Product (abstract)
├── PerishableProduct (sealed)   — ExpiryDate, StorageTemp, IsExpired()
└── NonPerishableProduct (sealed) — ShelfLifeMonths, Barcode

Inventory (sealed)               — manages Product[] of capacity 500
```

See [`docs/architecture/DOMAIN_MODEL.md`](docs/architecture/DOMAIN_MODEL.md) for full class definitions and [`docs/assets/diagrams/CLASS_DIAGRAM.png`](docs/assets/diagrams/CLASS_DIAGRAM.png) for the class diagram.

## Data Persistence

Products are saved to a pipe-delimited `.txt` file under the user profile. Each line starts with a type flag (`P` for perishable, `N` for non-perishable) so the correct subclass is instantiated on load.

See [`docs/data/FILE_FORMAT_SPEC.md`](docs/data/FILE_FORMAT_SPEC.md) for the full format spec and [`docs/data/DATA_PERSISTENCE.md`](docs/data/DATA_PERSISTENCE.md) for the read/write strategy.

## Running the Project

1. Open the `.sln` file in Visual Studio 2022 or later
2. Ensure the target framework is set to `net8.0-windows`
3. Build and run — no additional setup required

**Supervised by:** Eng. Nadeen Anwar  
**Academic year:** 2025 / 2026