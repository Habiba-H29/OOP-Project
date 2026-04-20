namespace Supermarket_Inventory_Managment_System
{
    //PerishableProduct inherits from Product class (Inheritance)
    public class PerishableProuct : Product
    {
        public string ExpiryDate { get; set; } = string.Empty;
        public string StorageTemp { get; set; } = string.Empty;

        //To include expiration date information
        public override string GetDetails()
        {
            return base.GetDetails() + $",Expiry:{ExpiryDate}";
        }
        public bool IsExpired()
        {
            //Date comparison logic
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