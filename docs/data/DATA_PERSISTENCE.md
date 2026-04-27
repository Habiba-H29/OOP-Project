# Data Persistence

## Storage Strategy

The application uses local file-based persistence.

- Base folder: `%USERPROFILE%/.SuperMarketInventorySystem`
- Data file: `%USERPROFILE%/.SuperMarketInventorySystem/products.txt`

Path creation behavior is implemented in `Inventory.GetFilePath()`:
- Creates directory if missing.
- Creates data file if missing.

## Persistence Lifecycle

## Load

- Trigger: `Form1_Load` on application startup.
- Method: `Inventory.LoadProductsFromFile()`.
- Source: all lines in `products.txt`.
- Behavior:
  - Ignores blank lines.
  - Ignores lines with fewer than 8 fields.
  - Parses typed record prefix (`P` or `N`) and materializes matching product subtype.

## Save

- Trigger: every mutating inventory operation:
  - `AddToProducts`
  - `UpdateProduct`
  - `RemoveFromProducts`
- Method: `Inventory.SaveProductsToFile()`.
- Behavior:
  - Clears file contents first.
  - Iterates current in-memory array.
  - Writes one line per non-null product.
  - Uses subtype-specific serialization format.

## In-Memory to File Mapping

- In-memory store is `Product[]` with nullable slots.
- Serialized form is a flat line-oriented text file.
- On load, product ID is used as array index offset (`Products[productID - 1]`).
