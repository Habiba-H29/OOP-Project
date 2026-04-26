using Supermarket_Inventory_Managment_System.Models;
using System.Diagnostics;
using System.Text;

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
        private const int MinPrice = 1;
        private const int MaxPrice = 100000;
        private const int MinQuantity = 0;
        private const int MaxQuantity = 10000;
        private const int MinStorageTemp = -30;
        private const int MaxStorageTemp = 35;
        private const int MinShelfLifeMonths = 1;
        private const int MaxShelfLifeMonths = 120;

        private readonly Product? _productToEdit;
        private readonly TextBox _id = new();
        private readonly TextBox _name = new();
        private readonly NumericUpDown _price = new()
        {
            Minimum = MinPrice,
            Maximum = MaxPrice,
            DecimalPlaces = 2,
            Increment = 0.50m,
            ThousandsSeparator = true
        };
        private readonly NumericUpDown _qty = new()
        {
            Minimum = MinQuantity,
            Maximum = MaxQuantity
        };
        private readonly ComboBox _category = new();
        private readonly RadioButton _perishable = new() { Text = "Perishable", AutoSize = true };
        private readonly RadioButton _nonPerishable = new() { Text = "Non-Perishable", AutoSize = true };

        // Perishable Product Fields
        private readonly Label _expiryDateLabel = new() { Text = "Expiry Date" };
        private readonly DateTimePicker _expiryDate = new() { Format = DateTimePickerFormat.Short };

        private readonly Label _storageTempLabel = new() { Text = "StorageTemp" };
        private readonly NumericUpDown _storageTemp = new() { Maximum = MaxStorageTemp, Minimum = MinStorageTemp };

        // NonPerishable Product Fields
        private readonly Label _shelfLifeMonthsLabel = new() { Text = "Shelf Life Months" };
        private readonly NumericUpDown _shelfLifeMonths = new() { Minimum = MinShelfLifeMonths, Maximum = MaxShelfLifeMonths };


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
            Width = 520;
            Height = 470;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            ConfigureControlsAppearance();
            ConfigureTableLayout();

            _category.DataSource = Enum.GetValues<Category>();

            _table.Controls.Add(new Label { Text = "ID", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            _table.Controls.Add(_id, 1, 0);

            _table.Controls.Add(new Label { Text = "Name", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            _table.Controls.Add(_name, 1, 1);

            _table.Controls.Add(new Label { Text = "Price", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
            _table.Controls.Add(_price, 1, 2);

            _table.Controls.Add(new Label { Text = "Quantity", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
            _table.Controls.Add(_qty, 1, 3);

            _table.Controls.Add(new Label { Text = "Category", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
            _table.Controls.Add(_category, 1, 4);

            _table.Controls.Add(new Label { Text = "Type", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 5);

            FlowLayoutPanel typePanel = new()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            typePanel.Controls.Add(_perishable);
            typePanel.Controls.Add(_nonPerishable);
            _table.Controls.Add(typePanel, 1, 5);

            _perishable.CheckedChanged += (_, _) => RenderTypeFields();
            _nonPerishable.CheckedChanged += (_, _) => RenderTypeFields();
            _perishable.Checked = true;
            RenderTypeFields();

            FlowLayoutPanel panelButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0),
                Margin = new Padding(0, 14, 0, 0)
            };
            Button btnSave = new Button { Text = "Save", Width = 110, Height = 38 };
            Button btnCancel = new Button { Text = "Cancel", Width = 110, Height = 38 };

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

        private void ConfigureTableLayout()
        {
            _table.ColumnStyles.Clear();
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _table.RowStyles.Clear();
            for (int i = 0; i < _table.RowCount; i++)
            {
                _table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
        }

        private void ConfigureControlsAppearance()
        {
            _id.Dock = DockStyle.Fill;
            _name.Dock = DockStyle.Fill;
            _category.Dock = DockStyle.Fill;
            _expiryDate.Dock = DockStyle.Fill;
            _barcode.Dock = DockStyle.Fill;

            _price.Dock = DockStyle.Left;
            _qty.Dock = DockStyle.Left;
            _storageTemp.Dock = DockStyle.Left;
            _shelfLifeMonths.Dock = DockStyle.Left;

            _price.Width = 180;
            _qty.Width = 180;
            _storageTemp.Width = 180;
            _shelfLifeMonths.Width = 180;

            _name.Margin = new Padding(0, 4, 0, 4);
            _id.Margin = new Padding(0, 4, 0, 4);
            _price.Margin = new Padding(0, 4, 0, 4);
            _qty.Margin = new Padding(0, 4, 0, 4);
            _category.Margin = new Padding(0, 4, 0, 4);
            _expiryDate.Margin = new Padding(0, 4, 0, 4);
            _storageTemp.Margin = new Padding(0, 4, 0, 4);
            _shelfLifeMonths.Margin = new Padding(0, 4, 0, 4);
            _barcode.Margin = new Padding(0, 4, 0, 4);
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

            _expiryDateLabel.Text = "Expiry Date";
            _expiryDateLabel.AutoSize = true;
            _expiryDateLabel.Anchor = AnchorStyles.Left;

            _storageTempLabel.Text = "Storage Temp (°C)";
            _storageTempLabel.AutoSize = true;
            _storageTempLabel.Anchor = AnchorStyles.Left;

            _shelfLifeMonthsLabel.Text = "Shelf Life (Months)";
            _shelfLifeMonthsLabel.AutoSize = true;
            _shelfLifeMonthsLabel.Anchor = AnchorStyles.Left;

            _barcodeLabel.Text = "Barcode";
            _barcodeLabel.AutoSize = true;
            _barcodeLabel.Anchor = AnchorStyles.Left;

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

        private bool ValidateInput(out string validationMessage)
        {
            StringBuilder errors = new();

            if (string.IsNullOrWhiteSpace(_name.Text))
            {
                errors.AppendLine("- Product name is required.");
            }
            else if (_name.Text.Trim().Length < 2)
            {
                errors.AppendLine("- Product name must be at least 2 characters.");
            }

            if (_price.Value < MinPrice || _price.Value > MaxPrice)
            {
                errors.AppendLine($"- Price must be between {MinPrice:N2} and {MaxPrice:N2}.");
            }

            if (_qty.Value < MinQuantity || _qty.Value > MaxQuantity)
            {
                errors.AppendLine($"- Quantity must be between {MinQuantity} and {MaxQuantity}.");
            }

            if (_category.SelectedItem is null)
            {
                errors.AppendLine("- Please select a category.");
            }

            if (!_perishable.Checked && !_nonPerishable.Checked)
            {
                errors.AppendLine("- Please select a product type (Perishable or NonPerishable).");
            }

            if (_perishable.Checked)
            {
                if (_expiryDate.Value.Date < DateTime.Today)
                {
                    errors.AppendLine("- Expiry date cannot be in the past.");
                }

                if (_storageTemp.Value < MinStorageTemp || _storageTemp.Value > MaxStorageTemp)
                {
                    errors.AppendLine($"- Storage temperature must be between {MinStorageTemp} and {MaxStorageTemp} °C.");
                }
            }
            else if (_nonPerishable.Checked)
            {
                if (_shelfLifeMonths.Value < MinShelfLifeMonths || _shelfLifeMonths.Value > MaxShelfLifeMonths)
                {
                    errors.AppendLine($"- Shelf life must be between {MinShelfLifeMonths} and {MaxShelfLifeMonths} months.");
                }

                if (string.IsNullOrWhiteSpace(_barcode.Text))
                {
                    errors.AppendLine("- Barcode is required for non-perishable products.");
                }
                else if (_barcode.Text.Trim().Length < 4)
                {
                    errors.AppendLine("- Barcode must contain at least 4 characters.");
                }
            }

            validationMessage = errors.ToString().TrimEnd();
            return validationMessage.Length == 0;
        }

        // Creates a new product object and assignes it to "CreatedProduct" variable
        private void SaveProduct()
        {
            if (!ValidateInput(out string validationMessage))
            {
                MessageBox.Show($"Please correct the following:\n\n{validationMessage}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string name = _name.Text.Trim();
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
                    string barcode = _barcode.Text.Trim();
                    CreatedProduct = existingId.HasValue
                        ? new NonPerishableProduct(existingId.Value, name, price, quantity, category, shelfLifeMonths, barcode)
                        : new NonPerishableProduct(name, price, quantity, category, shelfLifeMonths, barcode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save product. Reason: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}