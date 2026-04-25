namespace Supermarket_Inventory_Managment_System.Models
{
    // PerishableProduct inherits from Product class (Inheritance)
    public sealed class PerishableProduct : Product
    {
        public PerishableProduct(string name, double price, int quantity, Category category, string expiryDate, string storageTemp)
            : base(name, price, quantity, category)
        {
            ExpiryDate = expiryDate;
            StorageTemp = storageTemp;
        }

        public PerishableProduct(int productID, string name, double price, int quantity, Category category, string expiryDate, string storageTemp)
            : base(productID, name, price, quantity, category)
        {
            ExpiryDate = expiryDate;
            StorageTemp = storageTemp;
        }

        public string ExpiryDate { get; set; } = string.Empty;
        public string StorageTemp { get; set; } = string.Empty;

        // To include expiration date information
        public override string GetDetails()
        {
            return base.GetDetails() + $"|Expiry:{ExpiryDate}";
        }
        public bool IsExpired()
        {
            // Date comparison logic
            DateTime expiry;
            bool isValidDate = DateTime.TryParse(ExpiryDate, out expiry);

            if (isValidDate)
            {
                return expiry < DateTime.Now;
            }
            return false;
        }
    }
}
