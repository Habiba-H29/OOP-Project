namespace Supermarket_Inventory_Managment_System
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        //Here we will use the Enums we defined earlier
        public Category Category { get; set; }
        public ProductStatus Status { get; set; }
        public virtual string GetDetails()
        {
            return $"ID:{ProductID},Name:{Name},Price:{Price}";
        }
        public void UpadateStock(int amount) 
        {
            int nextQuantity = Quantity + amount;
            //Validates that the new quantity won't be negative before updating 
            if (nextQuantity <0)
            {
                throw new InvalidOperationException("Stock can't be negative!");
            }
            Quantity = nextQuantity;

            if (Quantity == 0)
            {
                Status = ProductStatus.OutOfStock;
            }
            else if (IsLowStock())
            {
                Status = ProductStatus.LowStock;
            }
            else
            {
                Status = ProductStatus.InStock;
            }
        }
        //Defines the condition for a "Low Stock" alert
        public bool IsLowStock() => Quantity > 0 && Quantity < 5;
    }
}