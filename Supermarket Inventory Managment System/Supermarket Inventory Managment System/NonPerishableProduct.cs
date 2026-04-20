namespace Supermarket_Inventory_Managment_System 
{
    //Inherts from Product class
    public class NonPerishableProuct : Product
    {
        //Shelf life in months (ex: 24 month)
        public int ShelfLifeMonths { get; set; }
        //Barcode for scanning
        public string Barcode { get; set; } = string.Empty;

        //Return + shelf life details
        public override string GetDetails()
        {
            return base.GetDetails() + $",Shelf Life:{ShelfLifeMonths}months";
        }
    }
}
