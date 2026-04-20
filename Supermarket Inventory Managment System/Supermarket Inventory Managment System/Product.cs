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
            Quantity += amount;
        }
        //Checks if the product is running low on stock
        public bool IsLowStock() => Quantity < 5;
    }
}