using System.Drawing;
using System.Windows.Forms;
using Inventory.Application.DTOs;
using Inventory.Desktop.Imaging;

namespace Inventory.Desktop.Forms;

// Vista de solo lectura con toda la información del producto.
// Reemplaza a la edición: el producto se puede consultar pero no modificar.
public partial class ProductPreview : Form
{
    private readonly ProductDto _product;
    private Image? _image;

    public ProductPreview(ProductDto product)
    {
        _product = product;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Detalle de producto";
        ClientSize = new Size(780, 600);
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
            RowCount = 3,
            Margin = new Padding(0)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));

        // Encabezado
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(44, 62, 80),
            Padding = new Padding(26, 14, 26, 12),
            Margin = new Padding(0)
        };

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0)
        };
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));

        headerLayout.Controls.Add(new Label
        {
            Text = "DETALLE DE PRODUCTO",
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            Margin = new Padding(0)
        }, 0, 0);

        headerLayout.Controls.Add(new Label
        {
            Text = "Vista de solo lectura. Los datos del catálogo no se pueden modificar.",
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(189, 195, 199),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
            Margin = new Padding(0)
        }, 0, 1);

        header.Controls.Add(headerLayout);

        // Cuerpo: imagen | datos
        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(24, 20, 24, 16)
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 292F));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Imagen
        var imageCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 247, 250),
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 20, 0)
        };

        var picture = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(245, 247, 250)
        };

        _image = ProductImages.LoadForProduct(_product.ImagePath, _product.Plu, _product.Sku)
            ?? ProductImages.CreatePlaceholder(_product.Name, _product.Category, 480, 480);
        picture.Image = _image;

        imageCard.Controls.Add(picture);

        // Datos
        var details = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 10,
            Margin = new Padding(0)
        };
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132F));
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        details.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        for (var i = 0; i < 9; i++)
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));

        var nameLabel = new Label
        {
            Text = _product.Name,
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
        details.Controls.Add(nameLabel, 0, 0);
        details.SetColumnSpan(nameLabel, 2);

        AddRow(details, 1, "PLU", _product.Plu);
        AddRow(details, 2, "SKU", _product.Sku);
        AddRow(details, 3, "Categoría", _product.Category);
        AddRow(details, 4, "Costo unitario", _product.Cost.ToString("C2"));
        AddRow(details, 5, "Precio de venta", _product.Price.ToString("C2"));
        AddRow(details, 6, "Stock actual", _product.Stock.ToString());
        AddRow(details, 7, "Stock mínimo", _product.MinimumStock.ToString());
        AddRow(details, 8, "Estado",
            _product.Status,
            _product.IsLowStock ? Color.FromArgb(192, 57, 43) : Color.FromArgb(39, 174, 96));
        AddRow(details, 9, "Valor del stock", _product.InventoryValue.ToString("C2"));

        body.Controls.Add(imageCard, 0, 0);
        body.Controls.Add(details, 1, 0);

        // Pie
        var footer = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 10, 24, 14),
            BackColor = Color.White
        };

        var footerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var lblHint = new Label
        {
            Text = "El catálogo es de solo lectura. Para agregar un producto nuevo usá el botón «Nuevo».",
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(149, 165, 166),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var btnClose = new Button
        {
            Text = "Cerrar",
            Width = 120,
            Height = 38,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.OK
        };
        btnClose.FlatAppearance.BorderSize = 0;

        footerLayout.Controls.Add(lblHint, 0, 0);
        footerLayout.Controls.Add(btnClose, 1, 0);
        footer.Controls.Add(footerLayout);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(body, 0, 1);
        root.Controls.Add(footer, 0, 2);

        Controls.Add(root);
        AcceptButton = btnClose;
        CancelButton = btnClose;
    }

    private static void AddRow(TableLayoutPanel layout, int row, string caption, string value,
        Color? valueColor = null)
    {
        layout.Controls.Add(new Label
        {
            Text = caption.ToUpperInvariant(),
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(127, 140, 141),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, row);

        layout.Controls.Add(new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 10F),
            ForeColor = valueColor ?? Color.FromArgb(60, 60, 60),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 1, row);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _image?.Dispose();

        base.Dispose(disposing);
    }
}
