using System.Drawing;
using System.Windows.Forms;
using Inventory.Application.DTOs;

namespace Inventory.Desktop.Forms;

public partial class ProductEditor : Form
{
    private readonly int _companyId;
    private readonly ProductDto? _editing;
    private TextBox txtPlu = null!;
    private TextBox txtSku = null!;
    private TextBox txtName = null!;
    private TextBox txtCategory = null!;
    private NumericUpDown numCost = null!;
    private NumericUpDown numPrice = null!;
    private NumericUpDown numMinimumStock = null!;
    private NumericUpDown numStock = null!;

    public ProductEditor(int companyId, ProductDto? dto = null)
    {
        _companyId = companyId;
        _editing = dto;
        InitializeComponent();
        if (_editing is not null)
            Fill();
    }

    private void InitializeComponent()
    {
        Text = _editing is null ? "Nuevo producto" : "Editar producto";
        Size = new Size(480, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9.5F);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(44, 62, 80),
            Padding = new Padding(24, 16, 24, 12)
        };

        var lblHeader = new Label
        {
            Text = _editing is null ? "Nuevo producto" : "Editar producto",
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        header.Controls.Add(lblHeader);

        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 9,
            Padding = new Padding(28, 20, 28, 20)
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        for (int i = 0; i < 8; i++)
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        txtPlu = CreateTextBox();
        txtSku = CreateTextBox();
        txtName = CreateTextBox();
        txtCategory = CreateTextBox();
        numCost = CreateMoneyInput();
        numPrice = CreateMoneyInput();
        numMinimumStock = CreateIntegerInput();
        numStock = CreateIntegerInput();

        AddRow(body, 0, "PLU", txtPlu);
        AddRow(body, 1, "SKU", txtSku);
        AddRow(body, 2, "Nombre", txtName);
        AddRow(body, 3, "Categoría", txtCategory);
        AddRow(body, 4, "Costo unitario", numCost);
        AddRow(body, 5, "Precio de venta", numPrice);
        AddRow(body, 6, "Stock mínimo", numMinimumStock);
        AddRow(body, 7, _editing is null ? "Stock inicial" : "Stock actual", numStock);

        if (_editing is not null)
        {
            txtPlu.Enabled = false;
            txtSku.Enabled = false;
            numStock.Enabled = false;
            txtPlu.BackColor = Color.FromArgb(245, 245, 245);
            txtSku.BackColor = Color.FromArgb(245, 245, 245);
            numStock.BackColor = Color.FromArgb(245, 245, 245);
        }

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };

        var btnSave = new Button
        {
            Text = "Guardar",
            Width = 130,
            Height = 38,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += (_, _) => Save();

        var btnCancel = new Button
        {
            Text = "Cancelar",
            Width = 110,
            Height = 38,
            DialogResult = DialogResult.Cancel,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.FromArgb(52, 73, 94),
            BackColor = Color.FromArgb(236, 240, 241)
        };
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(210, 218, 222);

        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);

        body.Controls.Add(buttons, 0, 8);
        body.SetColumnSpan(buttons, 2);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(body, 0, 1);

        Controls.Add(root);
        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }

    private static void AddRow(TableLayoutPanel layout, int row, string label, Control control)
    {
        layout.Controls.Add(new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(52, 73, 94)
        }, 0, row);

        layout.Controls.Add(control, 1, row);
    }

    private static TextBox CreateTextBox() => new()
    {
        Dock = DockStyle.Fill,
        Font = new Font("Segoe UI", 10F),
        Margin = new Padding(0, 7, 0, 7)
    };

    private static NumericUpDown CreateMoneyInput() => new()
    {
        Dock = DockStyle.Fill,
        DecimalPlaces = 2,
        Maximum = 1_000_000m,
        ThousandsSeparator = true,
        Font = new Font("Segoe UI", 10F),
        Margin = new Padding(0, 7, 0, 7)
    };

    private static NumericUpDown CreateIntegerInput() => new()
    {
        Dock = DockStyle.Fill,
        DecimalPlaces = 0,
        Maximum = 1_000_000m,
        ThousandsSeparator = true,
        Font = new Font("Segoe UI", 10F),
        Margin = new Padding(0, 7, 0, 7)
    };

    private void Fill()
    {
        txtPlu.Text = _editing!.Plu;
        txtSku.Text = _editing.Sku;
        txtName.Text = _editing.Name;
        txtCategory.Text = _editing.Category;
        numCost.Value = _editing.Cost;
        numPrice.Value = _editing.Price;
        numMinimumStock.Value = _editing.MinimumStock;
        numStock.Value = _editing.Stock;
    }

    private void Save()
    {
        try
        {
            var plu = txtPlu.Text.Trim();
            var sku = txtSku.Text.Trim();
            var name = txtName.Text.Trim();
            var category = txtCategory.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("El nombre y la categoría son obligatorios.", "Datos incompletos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_editing is null)
            {
                Di.ProductService.Create(_companyId, new CreateProductDto(
                    plu,
                    sku,
                    name,
                    category,
                    numCost.Value,
                    numPrice.Value,
                    (int)numMinimumStock.Value,
                    (int)numStock.Value));
            }
            else
            {
                Di.ProductService.Update(_editing.Id, _companyId, new UpdateProductDto(
                    name,
                    category,
                    numCost.Value,
                    numPrice.Value,
                    (int)numMinimumStock.Value));
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
