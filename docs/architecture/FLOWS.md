# Architecture Flows

## Runtime Flow

1. Application starts in `Program.Main`.
2. Main form (`Form1`) is created.
3. `Form1_Load` loads persisted products through `Inventory.LoadProductsFromFile()`.
4. Inventory grid binds non-null products.
5. Insights tab is built and populated when selected.

## Add Product Flow

1. User clicks Add in Inventory tab.
2. `AddProductForm` opens without existing product context.
3. User enters common fields and type-specific fields.
4. `SaveProduct()` validates input.
5. On success, form produces `CreatedProduct` instance.
6. `Form1` calls `Inventory.AddToProducts(newProduct)`.
7. Inventory writes full collection to disk with `SaveProductsToFile()`.
8. Grid refreshes via search/bind path.

## Edit Product Flow

1. User selects a row in the products grid.
2. User clicks Edit.
3. `AddProductForm` opens with selected product.
4. `PopulateFields()` preloads values.
5. User updates values and saves.
6. `Inventory.UpdateProduct(updatedProduct)` replaces matching `ProductID`.
7. Full collection is persisted again.
8. Grid refreshes.

## Delete Product Flow

1. User selects a product row.
2. User clicks Delete.
3. Confirmation dialog appears.
4. On confirmation, `Inventory.RemoveFromProducts(productID)` nulls matching slot.
5. Full collection is persisted again.
6. Grid refreshes.

## Search Flow

1. User types in search box.
2. Text changed event triggers `ApplyInventorySearch()`.
3. `Inventory.SearchByName(searchTerm)` returns matching list.
4. Grid binds filtered products.
5. During bind, the `Details` column is explicitly sized for readability.

If search text is empty, `SearchByName` returns all non-null products.

## Insights Refresh Flow

1. User navigates to Inventory Insights tab.
2. `tabControl1_SelectedIndexChanged` computes metrics:
- total products count
- total stock value
- product type summary
- low stock count
3. Top 10 products by quantity are bound to insights grid.

## Persistence Flow

Write path:
- Add/Update/Delete operations trigger `SaveProductsToFile()` immediately.

Read path:
- Application load triggers `LoadProductsFromFile()`.
- Each line is parsed and materialized as perishable or non-perishable product.
