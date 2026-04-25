using Supermarket_Inventory_Managment_System.Models;
using System.Diagnostics;

namespace Supermarket_Inventory_Managment_System.Forms
{
    /* 
     AddProductForm is used for both adding a new product and editing
     It accepts a product value in the constructor defaulting to "null"
     If a product is provided the fields get populated with the value of the provided product (eg. name, price)
     If there's no product provided "null" the Form uses empty fields and creates a new product
     
     Basic validation already implemented but might make it better later
     */
    public sealed class AddProductForm : Form
    {
        private readonly Product? _productToEdit;
        private readonly TextBox _id = new();
        private readonly TextBox _name = new();
        private readonly NumericUpDown _price = new();
        private readonly NumericUpDown _qty = new();
        private readonly ComboBox _category = new();
        private readonly RadioButton _perishable = new() { Text = "Perishable" };
        private readonly RadioButton _nonPerishable = new() { Text = "NonPerishable" };

        // Perishable Product Fields
        private readonly Label _expiryDateLabel = new() { Text = "Expiry Date" };
        private readonly DateTimePicker _expiryDate = new() { Format = DateTimePickerFormat.Short };

        private readonly Label _storageTempLabel = new() { Text = "StorageTemp" };
        private readonly NumericUpDown _storageTemp = new() { Maximum = 35, Minimum = 0 };

        // NonPerishable Product Fields
        private readonly Label _shelfLifeMonthsLabel = new() { Text = "Shelf Life Months" };
        private readonly NumericUpDown _shelfLifeMonths = new() { Minimum = 1, Maximum = 36 };


        private readonly Label _barcodeLabel = new() { Text = "Barcode" };
        private readonly TextBox _barcode = new();

        private readonly TableLayoutPanel _table = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 12,
            Padding = new Padding(12),
            AutoSize = true
        };

        public Product? CreatedProduct { get; private set; }

        public AddProductForm(Product? product = null)
        {
            _productToEdit = product;

            Text = product == null ? "Add product" : "Edit product";
            Width = 420;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;

            _category.DataSource = Enum.GetValues<Category>();

            _table.Controls.Add(new Label { Text = "id" }, 0, 0);
            _table.Controls.Add(_id, 1, 0);

            _table.Controls.Add(new Label { Text = "Name" }, 0, 1);
            _table.Controls.Add(_name, 1, 1);

            _table.Controls.Add(new Label { Text = "Price" }, 0, 2);
            _table.Controls.Add(_price, 1, 2);

            _table.Controls.Add(new Label { Text = "Quantity" }, 0, 3);
            _table.Controls.Add(_qty, 1, 3);

            _table.Controls.Add(new Label { Text = "Category" }, 0, 4);
            _table.Controls.Add(_category, 1, 4);

            _table.Controls.Add(new Label { Text = "Type" }, 0, 5);
            _table.Controls.Add(_perishable, 0, 6);
            _table.Controls.Add(_nonPerishable, 1, 6);

            _perishable.CheckedChanged += (_, _) => RenderTypeFields();
            _nonPerishable.CheckedChanged += (_, _) => RenderTypeFields();
            _perishable.Checked = true;
            RenderTypeFields();

            FlowLayoutPanel panelButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            Button btnSave = new Button { Text = "Save", Width = 90, Height = 60 };
            Button btnCancel = new Button { Text = "Cancel", Width = 90, Height = 60 };

            btnSave.Click += (_, _) => SaveProduct();
            btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

            panelButtons.Controls.Add(btnSave);
            panelButtons.Controls.Add(btnCancel);

            _table.Controls.Add(panelButtons, 0, 11);
            _table.SetColumnSpan(panelButtons, 2);

            Controls.Add(_table);

            if (_productToEdit != null)
            {
                PopulateFields(_productToEdit);
            }
            else
            {
                _id.Text = "Auto-assigned";
                _id.ReadOnly = true;
                _id.Enabled = false;
            }
        }

        private void PopulateFields(Product product)
        {
            _id.Text = product.ProductID.ToString();
            _id.ReadOnly = true;
            _id.Enabled = false;

            _name.Text = product.Name;
            _price.Value = Convert.ToDecimal(product.Price);
            _qty.Value = product.Quantity;
            _category.SelectedItem = product.Category;

            if (product is PerishableProduct perishable)
            {
                _perishable.Checked = true;
                if (DateTime.TryParse(perishable.ExpiryDate, out DateTime dt))
                {
                    _expiryDate.Value = dt;
                }
                if (decimal.TryParse(perishable.StorageTemp, out decimal storage))
                {
                    _storageTemp.Value = Math.Clamp(storage, _storageTemp.Minimum, _storageTemp.Maximum);
                }
            }
            else if (product is NonPerishableProduct nonPerishable)
            {
                _nonPerishable.Checked = true;
                _shelfLifeMonths.Value = Math.Clamp(nonPerishable.ShelfLifeMonths, (int)_shelfLifeMonths.Minimum, (int)_shelfLifeMonths.Maximum);
                _barcode.Text = nonPerishable.Barcode;
            }

            RenderTypeFields();
        }

        // Used to render fileds according to the type of the product (perishable, nonPerishable)
        private void RenderTypeFields()
        {
            _table.SuspendLayout();
            _table.Controls.Remove(_expiryDateLabel);
            _table.Controls.Remove(_expiryDate);
            _table.Controls.Remove(_storageTempLabel);
            _table.Controls.Remove(_storageTemp);

            _table.Controls.Remove(_shelfLifeMonthsLabel);
            _table.Controls.Remove(_shelfLifeMonths);
            _table.Controls.Remove(_barcodeLabel);
            _table.Controls.Remove(_barcode);
            if (_perishable.Checked)
            {
                _table.Controls.Add(_expiryDateLabel, 0, 7);
                _table.Controls.Add(_expiryDate, 1, 7);
                _table.Controls.Add(_storageTempLabel, 0, 8);
                _table.Controls.Add(_storageTemp, 1, 8);
            }
            else if (_nonPerishable.Checked)
            {
                _table.Controls.Add(_shelfLifeMonthsLabel, 0, 7);
                _table.Controls.Add(_shelfLifeMonths, 1, 7);
                _table.Controls.Add(_barcodeLabel, 0, 8);
                _table.Controls.Add(_barcode, 1, 8);
            }
            _table.ResumeLayout();
        }

        // Creates a new product object and assignes it to "CreatedProduct" variable
        private void SaveProduct()
        {
            // Validation logic here
            if (string.IsNullOrWhiteSpace(_name.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }
            try
            {
                string name = _name.Text;
                double price = (double)_price.Value;
                int quantity = (int)_qty.Value;
                Category category = (Category)_category.SelectedItem;

                int? existingId = _productToEdit?.ProductID;

                if (_perishable.Checked)
                {
                    string expiryDate = _expiryDate.Value.ToShortDateString();
                    string storageTemp = _storageTemp.Value.ToString();
                    Debug.WriteLine(expiryDate);
                    CreatedProduct = existingId.HasValue
                        ? new PerishableProduct(existingId.Value, name, price, quantity, category, expiryDate, storageTemp)
                        : new PerishableProduct(name, price, quantity, category, expiryDate, storageTemp);
                }
                else
                {
                    int shelfLifeMonths = (int)_shelfLifeMonths.Value;
                    string barcode = _barcode.Text;
                    CreatedProduct = existingId.HasValue
                        ? new NonPerishableProduct(existingId.Value, name, price, quantity, category, shelfLifeMonths, barcode)
                        : new NonPerishableProduct(name, price, quantity, category, shelfLifeMonths, barcode);
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Data Error: {ex.Message}");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}