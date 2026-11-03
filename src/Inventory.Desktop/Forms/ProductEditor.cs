using System.Drawing;
using System.Windows.Forms;
using Inventory.Application.DTOs;
using Inventory.Desktop.Imaging;

namespace Inventory.Desktop.Forms;

// Alta de productos nuevos. La edición de productos existentes se reemplazó
// por ProductPreview (solo lectura).
public partial class ProductEditor : Form
{
    private readonly int _companyId;
    private TextBox txtPlu = null!;
    private TextBox txtSku = null!;
    private TextBox txtName = null!;
    private TextBox txtCategory = null!;
    private NumericUpDown numCost = null!;
    private NumericUpDown numPrice = null!;
    private NumericUpDown numMinimumStock = null!;
    private NumericUpDown numStock = null!;
    private PictureBox _imgPreview = null!;
    private string? _imagePath;
    private Image? _image;

    public ProductEditor(int companyId)
    {
        _companyId = companyId;
        InitializeComponent();
        RefreshImagePreview();
    }

    private void InitializeComponent()
    {
        Text = "Nuevo producto";
        ClientSize = new Size(540, 640);
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
            Text = "Nuevo producto",
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
            RowCount = 10,
            Padding = new Padding(28, 18, 28, 18)
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        for (int i = 0; i < 8; i++)
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
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
        AddRow(body, 7, "Stock inicial", numStock);

        // Imagen del producto
        body.Controls.Add(new Label
        {
            Text = "Imagen",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(52, 73, 94)
        }, 0, 8);

        var imagePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 6, 0, 6)
        };
        imagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 106F));
        imagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        _imgPreview = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(245, 247, 250),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 12, 0)
        };

        var imageButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = new Padding(0)
        };

        var btnPick = new Button
        {
            Text = "Elegir imagen...",
            Width = 158,
            Height = 34,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 2, 0, 6)
        };
        btnPick.FlatAppearance.BorderSize = 0;
        btnPick.Click += (_, _) => PickImage();

        var btnClear = new Button
        {
            Text = "Quitar imagen",
            Width = 158,
            Height = 32,
            BackColor = Color.FromArgb(236, 240, 241),
            ForeColor = Color.FromArgb(52, 73, 94),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F),
            Cursor = Cursors.Hand,
            Margin = new Padding(0)
        };
        btnClear.FlatAppearance.BorderColor = Color.FromArgb(210, 218, 222);
        btnClear.Click += (_, _) =>
        {
            _imagePath = null;
            RefreshImagePreview();
        };

        imageButtons.Controls.Add(btnPick);
        imageButtons.Controls.Add(btnClear);

        imagePanel.Controls.Add(_imgPreview, 0, 0);
        imagePanel.Controls.Add(imageButtons, 1, 0);
        body.Controls.Add(imagePanel, 1, 8);

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

        body.Controls.Add(buttons, 0, 9);
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

    private void PickImage()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Elegir imagen del producto",
            Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Todos los archivos|*.*"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        var saved = ProductImages.Save(dialog.FileName);
        if (saved is null)
        {
            MessageBox.Show("No se pudo guardar la imagen seleccionada.", "Imagen",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _imagePath = saved;
        RefreshImagePreview();
    }

    private void RefreshImagePreview()
    {
        _image?.Dispose();
        _image = ProductImages.Load(_imagePath)
            ?? ProductImages.CreatePlaceholder("?", "Sin imagen", 240, 240);

        _imgPreview.Image = _image;
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

            Di.ProductService.Create(_companyId, new CreateProductDto(
                plu,
                sku,
                name,
                category,
                numCost.Value,
                numPrice.Value,
                (int)numMinimumStock.Value,
                (int)numStock.Value,
                _imagePath));

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _image?.Dispose();

        base.Dispose(disposing);
    }
}
