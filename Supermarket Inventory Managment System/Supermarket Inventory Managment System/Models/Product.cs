namespace Supermarket_Inventory_Managment_System.Models
{
    public abstract class Product
    {
        // Constructor is protected to prevent direct instantiation of the abstract class, but allows derived classes to initialize properties
        protected Product(int productID, string name, double price, int quantity, Category category)
        {
            ProductID = productID;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
            Status = quantity == 0 ? ProductStatus.OutOfStock : ProductStatus.InStock;
        }

        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }

        // Here we will use the Enums we defined earlier
        public Category Category { get; set; }
        public ProductStatus Status { get; set; }
        public virtual string GetDetails()
        {
            return $"ID:{ProductID},Name:{Name},Price:{Price}";
        }
        public void UpdateStock(int amount)
        {
            int nextQuantity = Quantity + amount;
            // Validates that the new quantity won't be negative before updating 
            if (nextQuantity < 0)
            {
                throw new InvalidOperationException("Stock can't be negative!");
            }
            Quantity = nextQuantity;
            Status = getProductStatus(Quantity);
        }

        public static ProductStatus getProductStatus(int quantity)
        {
            if (quantity == 0)
            {
                return ProductStatus.OutOfStock;
            }
            else if (quantity < 5)
            {
                return ProductStatus.LowStock;
            }
            else
            {
                return ProductStatus.InStock;
            }
        }
        // Defines the condition for a "Low Stock" alert
        public bool IsLowStock() => Quantity > 0 && Quantity < 5;
    }
}
