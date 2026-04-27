# Domain Model

## Core Types

## `Product` (abstract)

Base entity for all products.

Fields/properties:
- `ProductID` (read-only after construction)
- `Name`
- `Price`
- `Quantity`
- `Category` (`Category` enum)
- `Status` (`ProductStatus` enum)
- `Details` (computed property that delegates to `GetDetails()`)

Behavior:
- `GetDetails()` virtual string summary
- `UpdateStock(int amount)` updates quantity and recalculates status
- `IsLowStock()` returns true for quantity in range 1..4
- `getProductStatus(int quantity)` maps quantity to stock status

UI projection note:
- The inventory grid consumes `Product.Details`, so subtype overrides of `GetDetails()` control what appears in the Details column.

Identity behavior:
- Uses static `_nextProductId` for auto-assigned IDs.
- Explicit-ID constructor updates `_nextProductId` when loading persisted data.

## `PerishableProduct` (sealed : `Product`)

Additional fields:
- `ExpiryDate` (stored as string)
- `StorageTemp` (stored as string)

Behavior:
- Overrides `GetDetails()` to return perishable-specific details (expiry and storage temperature).
- `IsExpired()` parses `ExpiryDate` and compares with current date/time.

## `NonPerishableProduct` (sealed : `Product`)

Additional fields:
- `ShelfLifeMonths` (int)
- `Barcode` (string)

Behavior:
- Overrides `GetDetails()` to return non-perishable-specific details (shelf life and barcode).

## `Inventory` (sealed)

Responsible for in-memory collection and persistence.

State:
- `_capacity`
- `Products` array of `Product` with nullable slots

Operations:
- `AddToProducts(Product product)`
- `UpdateProduct(Product updatedProduct)`
- `RemoveFromProducts(int productID)`
- `SearchByName(string name)`
- `GetLowStockItems()`
- `GetTotalProductsCount()`
- `GetTotalStockValue()`
- `GetProductTypesSummary()`
- `LoadProductsFromFile()`
- `SaveProductsToFile()`

## Enums

## `Category`
- Dairy
- Bakery
- Produce
- Beverages
- Frozen
- Household
- Snacks

## `ProductStatus`
- InStock
- LowStock
- OutOfStock
- Discontinued

## Model Relationships

```text
Product (abstract)
  |- PerishableProduct (sealed)
  |- NonPerishableProduct

Inventory
  |- owns Product[] collection
  |- persists Product variants as typed records (P or N)
```

## Domain Constraints

- Quantity cannot become negative (`UpdateStock` throws exception).
- Status is quantity-driven for in-stock/low-stock/out-of-stock transitions.
- Perishable records require expiry date and storage temperature.
- Non-perishable records require shelf life and barcode.
