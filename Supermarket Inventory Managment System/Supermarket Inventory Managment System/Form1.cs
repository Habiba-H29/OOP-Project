using Supermarket_Inventory_Managment_System.Models;
using Supermarket_Inventory_Managment_System.Forms;
using System.Diagnostics;

namespace Supermarket_Inventory_Managment_System
{
    public partial class Form1 : Form
    {
        private Label lblTotalProducts = null!;
        private Label lblTotalValue = null!;
        private Label lblTypeSummary = null!;
        private readonly Inventory _inventory = new(500);
        private readonly TabControl _tabs = new() { Dock = DockStyle.Fill};
        private readonly DataGridView _products = new() { 
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
        };
        private readonly TextBox _inventorySearchBox = new() { Width = 260 };

        public Form1()
        {
            InitializeComponent();
            Text = "Supermarket Inventory Management System";
            Width = 1200;
            Height = 700;

            Controls.Add(_tabs);
            BuildDashboardTab();
            BuildInventoryTab();


        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _inventory.LoadProductsFromFile();
            BindProductsGrid();
            BuildInsightsTab();
            if (_inventory.Products[0] != null)
            {
                MessageBox.Show($"Successfully loaded {_inventory.Products} products.");
            }
            else
            {
                MessageBox.Show("Warning: Inventory Products collection is null or empty after loading.");
            }
        }

        private void BuildInventoryTab()
        {
            TabPage tab = new TabPage("Inventory");
            TableLayoutPanel root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            FlowLayoutPanel toolBar = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };

            Button addBtn = new Button { Text = "Add", BackColor = Color.FromName("green"), Width = 100, Height = 50 };
            addBtn.Click += (s, e) =>
            {
                // Open Add Product Form
                AddProductForm addForm = new AddProductForm();
                if (addForm.ShowDialog() == DialogResult.OK && addForm.CreatedProduct is Product newProduct)
                {
                    // Gets CreatedProduct from "AddProductForm" and adds it to the inventory
                    _inventory.AddToProducts(newProduct);
                    ApplyInventorySearch();
                }
            };
            Button editBtn = new Button { Text = "Edit", BackColor = Color.FromName("blue"), Width = 100, Height = 50 };
            editBtn.Click += (s, e) =>
            {
                Product? selectedProduct = GetSelectedProduct();
                if (selectedProduct == null)
                {
                    MessageBox.Show("Please select a product to edit.");
                    return;
                }

                AddProductForm editForm = new AddProductForm(selectedProduct);
                if (editForm.ShowDialog() == DialogResult.OK && editForm.CreatedProduct is Product updatedProduct)
                {
                    _inventory.UpdateProduct(updatedProduct);
                    ApplyInventorySearch();
                }
            };

            Button deleteBtn = new Button { Text = "Delete", BackColor = Color.FromName("red"), Width = 100, Height = 50 };
            deleteBtn.Click += (s, e) =>
            {
                Product? selectedProduct = GetSelectedProduct();
                if (selectedProduct != null)
                {
                    var confirmResult = MessageBox.Show($"Are you sure to delete '{selectedProduct.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        _inventory.RemoveFromProducts(selectedProduct.ProductID);
                        ApplyInventorySearch();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a product to delete.");
                }
            };

            Label searchLabel = new Label
            {
                Text = "Search",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 15, 0, 0)
            };

            _inventorySearchBox.Height = 30;
            _inventorySearchBox.Margin = new Padding(8, 12, 0, 0);
            _inventorySearchBox.PlaceholderText = "Type product name...";
            _inventorySearchBox.TextChanged += (_, _) => ApplyInventorySearch();

            toolBar.Controls.Add(addBtn);
            toolBar.Controls.Add(deleteBtn);
            toolBar.Controls.Add(editBtn);
            toolBar.Controls.Add(searchLabel);
            toolBar.Controls.Add(_inventorySearchBox);

            Debug.WriteLine(_inventory.Products);

            root.Controls.Add(toolBar, 0, 0);
            root.Controls.Add(_products, 0, 1);

            tab.Controls.Add(root);

            _tabs.TabPages.Add(tab);

        }

        private void ApplyInventorySearch()
        {
            string searchTerm = _inventorySearchBox.Text;
            Product[] filtered = _inventory.SearchByName(searchTerm);
            BindProductsGrid(filtered);
        }

        // Removes null values from getting rendered into the table
        private void BindProductsGrid(IEnumerable<Product>? products = null)
        {
            _products.DataSource = null;
            _products.DataSource = (products ?? _inventory.Products.OfType<Product>()).ToList();
        }

        private Product? GetSelectedProduct()
        {
            if (_products.CurrentRow?.DataBoundItem is Product selectedProduct)
            {
                return selectedProduct;
            }

            return null;
        }

        private void BuildDashboardTab()
        {
            TabPage tab = new TabPage("Dashboard");
            TableLayoutPanel root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(20)
            };

            root.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label header = new()
            {
                Text = "Welcome to the Supermarket Inventory Management System",
                Font = new Font("Arial", 24, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label subText = new()
            {
                Text = "Manage products, track stock, and keep your inventory up to date.",
                Font = new Font("Arial", 12, FontStyle.Regular),
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button goToInventoryBtn = new Button
            {
                Text = "Go to Inventory",
                Width = 180,
                Height = 45,
                Anchor = AnchorStyles.None,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            goToInventoryBtn.Click += (s, e) =>
            {
                TabPage? inventoryTab = _tabs.TabPages
                    .Cast<TabPage>()
                    .FirstOrDefault(t => t.Text == "Inventory");

                if (inventoryTab != null)
                {
                    _tabs.SelectedTab = inventoryTab;
                }
            };

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(subText, 0, 1);
            root.Controls.Add(goToInventoryBtn, 0, 2);

            tab.Controls.Add(root);

            _tabs.TabPages.Add(tab);
        }
        private void BuildInsightsTab()
        {
            TabPage insightsTab = new TabPage("Inventory Insights"); 

            lblTotalProducts = new Label
            {
                Text = "Total Products:0",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            lblTotalValue = new Label
            {
                Text = "Stock Value:$0",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            lblTypeSummary = new Label
            {
                Text = "Summary:",
                Location = new Point(20, 100),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            insightsTab.Controls.Add(lblTotalProducts);
            insightsTab.Controls.Add(lblTotalValue);
            insightsTab.Controls.Add(lblTypeSummary);
            _tabs.TabPages.Add(insightsTab);
            _tabs.SelectedIndexChanged += tabControl1_SelectedIndexChanged;


        }

        private void BuildReportsTab()
        {
            // Will include reports like: total_products, total_stock_value, etc
        }
        //Updates inventory statistics whenever the insights tab is selected
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(_tabs.SelectedIndex ==2)
            {
                lblTotalProducts.Text = "Total Products:" + _inventory.GetTotalProductsCount();
                lblTotalValue.Text = "Stock Value: $" + _inventory.GetTotalStockValue().ToString("N2");
                lblTypeSummary.Text = _inventory.GetProductTypesSummary();
            }
        }
    }
}
