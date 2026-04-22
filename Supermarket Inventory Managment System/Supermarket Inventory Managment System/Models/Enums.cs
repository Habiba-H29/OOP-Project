using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket_Inventory_Managment_System.Models
{
    // Product Category List
    public enum Category
    {
        Dairy,
        Bakery,
        Produce,
        Beverages,
        Frozen,
        Household,
        Snacks
    }
    // Product Stock Status
    public enum ProductStatus
    {
        InStock,
        LowStock,
        OutOfStock,
        Discontinued
    }
}
