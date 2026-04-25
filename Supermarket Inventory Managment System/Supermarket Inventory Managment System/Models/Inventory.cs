using System.Diagnostics;

namespace Supermarket_Inventory_Managment_System.Models
{
    public sealed class Inventory
    {
        private readonly int _capacity;
        public Inventory(int capacity)
        {
            _capacity = capacity;
            Products = new Product[capacity];
        }

        public Product[] Products { get; private set; }

        public void AddToProducts(Product product)
        {
            int lastIndex = -1;
            for (int i = 0; i < _capacity; i++)
            {
                if (Products[i] == null)
                {
                    lastIndex = i;
                    break;
                }
            }
            if (lastIndex == -1)
            {
                throw new InvalidOperationException("Inventory is full. Cannot add more products.");
            }
            Products[lastIndex] = product;
            SaveProductsToFile();
        }

        public void UpdateProduct(Product updatedProduct)
        {
            for (int i = 0; i < Products.Length; i++)
            {
                if (Products[i] != null && Products[i].ProductID == updatedProduct.ProductID)
                {
                    Products[i] = updatedProduct;
                    SaveProductsToFile();
                    return;
                }
            }

            throw new InvalidOperationException("Product not found in inventory.");
        }

        public void RemoveFromProducts(int productID)
        {
            for (int i = 0; i < Products.Length; i++)
            {
                if (Products[i] != null && Products[i].ProductID == productID)
                {
                    Products[i] = null;
                    SaveProductsToFile();
                    return;
                }
            }
            throw new InvalidOperationException("Product not found in inventory.");
        }

        public void SaveProductsToFile()
        {
            string filePath = GetFilePath();

            // Clears the file first from previous data then writes the new data line by line.
            File.WriteAllText(filePath, "");
            foreach (Product p in Products)
            {
                if (p != null)
                {
                    string line;
                    if (p is PerishableProduct perishable)
                    {
                        line = $"P|{p.ProductID}|{p.Name}|{p.Category}|{p.Price}|{p.Quantity}|{perishable.ExpiryDate}|{perishable.StorageTemp}";
                    }
                    else if (p is NonPerishableProduct nonPerishable)
                    {
                        line = $"N|{p.ProductID}|{p.Name}|{p.Category}|{p.Price}|{p.Quantity}|{nonPerishable.ShelfLifeMonths}|{nonPerishable.Barcode}";
                    }
                    else
                    {
                        continue;
                    }

                    File.AppendAllText(filePath, line + Environment.NewLine);
                }
            }
        }

        public Product[] GetLowStockItems()
        {
            return Products.Where(p => p != null && p.IsLowStock()).ToArray();
        }

        public Product SearchByName(string name)
        {
            // Will be implemented later.
            return null;
        }

        public void LoadProductsFromFile()
        {
            string filePath = GetFilePath();
            string[] products = File.ReadAllLines(filePath);
            if (products.Length > 0)
            {
                Debug.WriteLine(products[0]);
            }

            foreach (string product in products)
            {
                if (string.IsNullOrWhiteSpace(product))
                {
                    continue;
                }

                string[] details = product.Split('|');
                if (details.Length < 8)
                {
                    continue;
                }

                string type = details[0];
                int productID = int.Parse(details[1]);
                string name = details[2];
                Category category = Enum.Parse<Category>(details[3]);
                double price = double.Parse(details[4]);
                int quantity = int.Parse(details[5]);

                if (type == "P")
                {
                    string expiryDate = details[6];
                    string storageTemp = details[7];
                    Products[productID - 1] = new PerishableProduct(productID, name, price, quantity, category, expiryDate, storageTemp);
                }
                else if (type == "N")
                {
                    int shelfLifeMonths = int.Parse(details[6]);
                    string barcode = details[7];
                    Products[productID - 1] = new NonPerishableProduct(productID, name, price, quantity, category, shelfLifeMonths, barcode);
                }
            }
        }

        // This method returns the file path for the product data file
        private static string GetFilePath()
        {
            // Gets the home path from the OS eg.C:\Users\hamza
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Creates a new directory to store the program data IF THE DIRECTORY ALREADY EXISTS IT DOES NOTHING
            Directory.CreateDirectory(Path.Combine(homePath, ".SuperMarketInventorySystem"));

            string filePath = Path.Combine(homePath, ".SuperMarketInventorySystem", "products.txt");

            // Creates a new file to store our products if the file doesn't exist
            // Closes the stream after creating the file
            if (!File.Exists(filePath)) File.Create(filePath).Close();

            return filePath;
        }
    }
}
