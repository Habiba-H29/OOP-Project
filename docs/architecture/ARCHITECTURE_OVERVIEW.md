# Architecture Overview

## System Type

Desktop application using .NET 8 WinForms.

- UI framework: Windows Forms
- Target framework: `net8.0-windows`
- Application style: event-driven, single-process desktop app
- Persistence: local file-based storage under user profile

## High-Level Components

1. Entry and bootstrap
- `Program.Main` initializes application configuration and opens the main form.

2. UI shell and navigation
- `Form1` hosts the main tab structure:
  - Dashboard
  - Inventory
  - Inventory Insights
- Tabs are built programmatically in `Form1`.

3. Inventory service layer
- `Inventory` encapsulates core operations:
  - add/update/remove products
  - search by name
  - compute stock metrics
  - load/save persisted records

4. Domain model layer
- `Product` (abstract base)
- `PerishableProduct` (sealed derived)
- `NonPerishableProduct` (sealed derived)
- `Category` and `ProductStatus` enums

## Startup Sequence

1. `Program.Main` calls:
- `ApplicationConfiguration.Initialize()`
- `Application.Run(new Form1())`

2. `Form1` constructor:
- Initializes controls
- Builds Dashboard and Inventory tabs

3. `Form1_Load`:
- Calls `Inventory.LoadProductsFromFile()`
- Binds loaded products to `DataGridView`
- Builds Insights tab

## Layer Boundaries

- UI (`Form1`, `AddProductForm`) is responsible for user interaction and input collection.
- Domain (`Product` hierarchy) represents business entities and type-specific data.
- Service (`Inventory`) owns storage and collection operations.

Current architecture intentionally keeps storage logic inside `Inventory` instead of introducing repositories/services split.

## Notable Architectural Characteristics

- Fixed in-memory capacity (`Inventory` uses `Product[]` initialized with capacity 500).
- Product identity auto-assignment in `Product` via static counter (`_nextProductId`).
- Immediate persistence on mutating operations (`AddToProducts`, `UpdateProduct`, `RemoveFromProducts`).
