namespace Supermarket_Inventory_Managment_System.Models
{
    public abstract class Product
    {
        private static int _nextProductId = 1;

        // Auto-assign ProductID
        protected Product(string name, double price, int quantity, Category category)
            : this(_nextProductId++, name, price, quantity, category)
        {
        }

        // Constructor with explicit ProductID (used for loading existing products)
        protected Product(int productID, string name, double price, int quantity, Category category)
        {
            ProductID = productID;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
            Status = getProductStatus(quantity);

            if (productID >= _nextProductId)
            {
                _nextProductId = productID + 1;
            }
        }

        public int ProductID { get; private set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }

        // Here we will use the Enums we defined earlier
        public Category Category { get; set; }
        public ProductStatus Status { get; set; }
        public string Details => GetDetails();

        public virtual string GetDetails()
        {
            return $"ID:{ProductID}|Name:{Name}|Price:{Price}";
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
