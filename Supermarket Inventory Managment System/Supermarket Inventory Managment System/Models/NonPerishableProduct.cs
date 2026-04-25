namespace Supermarket_Inventory_Managment_System.Models
{
    // Inherits from Product class
    public class NonPerishableProduct : Product
    {
        public NonPerishableProduct(string name, double price, int quantity, Category category, int shelfLifeMonths, string barcode)
            : base(name, price, quantity, category)
        {
            ShelfLifeMonths = shelfLifeMonths;
            Barcode = barcode;
        }

        public NonPerishableProduct(int productID, string name, double price, int quantity, Category category, int shelfLifeMonths, string barcode)
            : base(productID, name, price, quantity, category)
        {
            ShelfLifeMonths = shelfLifeMonths;
            Barcode = barcode;
        }

        // Shelf life in months (ex: 24 month)
        public int ShelfLifeMonths { get; set; }
        // Barcode for scanning
        public string Barcode { get; set; } = string.Empty;

        // Return + shelf life details
        public override string GetDetails()
        {
            return base.GetDetails() + $"|Shelf Life: {ShelfLifeMonths} months|Barcode: {Barcode}";
        }
    }
}
