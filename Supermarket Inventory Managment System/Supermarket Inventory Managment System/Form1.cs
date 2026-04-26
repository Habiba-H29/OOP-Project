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
        private Label lblLowStock = null!;
        private DataGridView _insightsTopProducts = null!;
        private TabPage? _insightsTab;
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

            if (_products.Columns["Details"] is DataGridViewColumn detailsColumn)
            {
                detailsColumn.Width = 320;
                detailsColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
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
                RowCount = 5,
                Padding = new Padding(20),
                BackColor = Color.Gainsboro
            };

            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // Takes up the remaining space to push everything up

            Label header = new()
            {
                Text = "Welcome to the Supermarket Inventory Management System",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 12)
            };

            Label subText = new()
            {
                Text = "Manage products, track stock, and keep your inventory up to date.",
                Font = new Font("Arial", 12, FontStyle.Regular),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 20)
            };

            Button goToInventoryBtn = new Button
            {
                Text = "Go to Inventory",
                Width = 220,
                Height = 55,
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 0, 15)
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

            Button goToInsightsBtn = new Button
            {
                Text = "Go to Insights",
                Width = 220,
                Height = 55,
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0)
            };

            goToInsightsBtn.Click += (s, e) =>
            {
                TabPage? insightsTab = _tabs.TabPages
                    .Cast<TabPage>()
                    .FirstOrDefault(t => t.Text == "Inventory Insights");

                if (insightsTab != null)
                {
                    _tabs.SelectedTab = insightsTab;
                }
            };

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(subText, 0, 1);
            root.Controls.Add(goToInventoryBtn, 0, 2);
            root.Controls.Add(goToInsightsBtn, 0, 3);

            tab.Controls.Add(root);
            _tabs.TabPages.Add(tab);
        }
        private void BuildInsightsTab()
        {
            _insightsTab = new TabPage("Inventory Insights");

            TableLayoutPanel root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 65));

            TableLayoutPanel metricsRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0, 0, 0, 8)
            };

            for (int i = 0; i < 4; i++)
            {
                metricsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            }

            metricsRow.Controls.Add(CreateMetricCard("Total Products", Color.FromArgb(41, 128, 185), out lblTotalProducts), 0, 0);
            metricsRow.Controls.Add(CreateMetricCard("Stock Value", Color.FromArgb(39, 174, 96), out lblTotalValue), 1, 0);
            metricsRow.Controls.Add(CreateMetricCard("Summary", Color.FromArgb(142, 68, 173), out lblTypeSummary), 2, 0);
            metricsRow.Controls.Add(CreateMetricCard("Low Stock Items", Color.FromArgb(211, 84, 0), out lblLowStock), 3, 0);

            _insightsTopProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            GroupBox productsBox = new GroupBox
            {
                Text = "Top 10 Products",
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            productsBox.Controls.Add(_insightsTopProducts);

            root.Controls.Add(metricsRow, 0, 0);
            root.Controls.Add(productsBox, 0, 1);

            _insightsTab.Controls.Add(root);
            _tabs.TabPages.Add(_insightsTab);
            _tabs.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
        }

        private Panel CreateMetricCard(string title, Color backColor, out Label valueLabel)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = backColor,
                Margin = new Padding(6)
            };

            Label titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 32,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            valueLabel = new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Arial", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            return card;
        }

        //Updates inventory statistics whenever the insights tab is selected
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(_tabs.SelectedIndex ==2)
            if (_tabs.SelectedTab == _insightsTab)
            {
                lblTotalProducts.Text = "Total Products:" + _inventory.GetTotalProductsCount();
                lblTotalValue.Text = "Stock Value: $" + _inventory.GetTotalStockValue().ToString("N2");
                lblTypeSummary.Text = _inventory.GetProductTypesSummary();
                lblLowStock.Text = "Low Stock Items:" + _inventory.GetLowStockItems().Length;

                Product[] allProducts = _inventory.Products.OfType<Product>().ToArray();
                _insightsTopProducts.DataSource = null;
                _insightsTopProducts.DataSource = allProducts
                    .OrderByDescending(p => p.Quantity)
                    .Take(10)
                    .ToList();
            }
        }
    }
}
