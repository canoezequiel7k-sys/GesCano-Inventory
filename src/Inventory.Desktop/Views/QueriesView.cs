using System.Drawing;
using System.Windows.Forms;
using Inventory.Application.DTOs;
using Inventory.Desktop.Imaging;

namespace Inventory.Desktop.Views;

public partial class QueriesView : UserControl, IView
{
    private TextBox _txtCode = null!;
    private Label _lblMessage = null!;
    private Label _lblName = null!;
    private Label _lblPlu = null!;
    private Label _lblSku = null!;
    private Label _lblCategory = null!;
    private Label _lblCost = null!;
    private Label _lblPrice = null!;
    private Label _lblStock = null!;
    private Label _lblMinimum = null!;
    private Label _lblStatus = null!;
    private Label _lblValue = null!;
    private PictureBox _imgProduct = null!;
    private Image? _image;

    public string Title => "Consultas";

    public QueriesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        Padding = new Padding(24, 18, 24, 18);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 310F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0)
        };
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

        header.Controls.Add(new Label
        {
            Text = "CONSULTAS",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            TextAlign = ContentAlignment.BottomLeft,
            Dock = DockStyle.Fill
        }, 0, 0);

        header.Controls.Add(new Label
        {
            Text = "Buscá un producto por su PLU o SKU y consultá todos sus datos.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(127, 140, 141),
            TextAlign = ContentAlignment.TopLeft,
            Dock = DockStyle.Fill
        }, 0, 1);

        // Buscador
        var searchPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(0, 6, 0, 6)
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));

        var lblCode = new Label
        {
            Text = "PLU o SKU",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(52, 73, 94)
        };

        _txtCode = new TextBox
        {
            Dock = DockStyle.Fill,
            PlaceholderText = "Ej.: 1001 o TEC-001",
            Font = new Font("Segoe UI", 10.5F),
            Margin = new Padding(0, 6, 12, 6)
        };
        _txtCode.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Search();
            }
        };

        var btnSearch = new Button
        {
            Text = "Buscar",
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 4, 0, 4)
        };
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.Click += (_, _) => Search();

        searchPanel.Controls.Add(lblCode, 0, 0);
        searchPanel.Controls.Add(_txtCode, 1, 0);
        searchPanel.Controls.Add(btnSearch, 2, 0);

        _lblMessage = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(127, 140, 141),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Resultado
        var result = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(20),
            Margin = new Padding(0)
        };

        var resultLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4
        };
        resultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        resultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        resultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        resultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        for (int i = 0; i < 4; i++)
            resultLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

        _lblName = CreateValueLabel(true);
        resultLayout.Controls.Add(CreateFieldLabel("PRODUCTO"), 0, 0);
        resultLayout.Controls.Add(_lblName, 1, 0);
        resultLayout.SetColumnSpan(_lblName, 3);

        _lblPlu = CreateValueLabel(false);
        _lblSku = CreateValueLabel(false);
        AddField(resultLayout, "PLU", _lblPlu, "SKU", _lblSku, 1);

        _lblCategory = CreateValueLabel(false);
        _lblStatus = CreateValueLabel(false);
        AddField(resultLayout, "CATEGORÍA", _lblCategory, "ESTADO", _lblStatus, 2);

        _lblCost = CreateValueLabel(false);
        _lblPrice = CreateValueLabel(false);
        AddField(resultLayout, "COSTO", _lblCost, "PRECIO", _lblPrice, 3);

        var stockRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 1
        };
        stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
        stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
        stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        stockRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));

        _lblStock = CreateValueLabel(false);
        _lblMinimum = CreateValueLabel(false);
        _lblValue = CreateValueLabel(false);

        stockRow.Controls.Add(CreateFieldLabel("STOCK"), 0, 0);
        stockRow.Controls.Add(_lblStock, 1, 0);
        stockRow.Controls.Add(CreateFieldLabel("MÍNIMO"), 2, 0);
        stockRow.Controls.Add(_lblMinimum, 3, 0);
        stockRow.Controls.Add(CreateFieldLabel("VALOR"), 4, 0);
        stockRow.Controls.Add(_lblValue, 5, 0);

        var resultContainer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        resultContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        resultContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        resultContainer.Controls.Add(resultLayout, 0, 0);
        resultContainer.Controls.Add(stockRow, 0, 1);

        // Imagen del producto a la derecha, sin mover los datos existentes.
        var imageCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10),
            Margin = new Padding(16, 0, 0, 0)
        };

        var imageLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        imageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
        imageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        imageLayout.Controls.Add(new Label
        {
            Text = "IMAGEN DEL PRODUCTO",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(127, 140, 141),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        _imgProduct = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(245, 247, 250)
        };
        imageLayout.Controls.Add(_imgProduct, 0, 1);
        imageCard.Controls.Add(imageLayout);

        var resultArea = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        resultArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        resultArea.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F));
        resultArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        resultArea.Controls.Add(resultContainer, 0, 0);
        resultArea.Controls.Add(imageCard, 1, 0);

        result.Controls.Add(resultArea);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(searchPanel, 0, 1);
        root.Controls.Add(_lblMessage, 0, 2);
        root.Controls.Add(result, 0, 3);

        Controls.Add(root);
        ShowImage(null);
    }

    private static Label CreateFieldLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        Font = new Font("Segoe UI", 8F, FontStyle.Bold),
        ForeColor = Color.FromArgb(127, 140, 141),
        TextAlign = ContentAlignment.MiddleLeft
    };

    private static Label CreateValueLabel(bool large) => new()
    {
        Text = "—",
        Dock = DockStyle.Fill,
        Font = new Font("Segoe UI", large ? 15F : 10F, large ? FontStyle.Bold : FontStyle.Regular),
        ForeColor = large ? Color.FromArgb(44, 62, 80) : Color.FromArgb(60, 60, 60),
        TextAlign = ContentAlignment.MiddleLeft
    };

    private static void AddField(TableLayoutPanel layout, string label1, Label value1,
        string label2, Label value2, int row)
    {
        layout.Controls.Add(CreateFieldLabel(label1), 0, row);
        layout.Controls.Add(value1, 1, row);
        layout.Controls.Add(CreateFieldLabel(label2), 2, row);
        layout.Controls.Add(value2, 3, row);
    }

    private void Search()
    {
        if (Session.Company is null)
            return;

        var code = _txtCode.Text.Trim();
        if (string.IsNullOrWhiteSpace(code))
        {
            _lblMessage.Text = "Ingresá un PLU o SKU para consultar.";
            _lblMessage.ForeColor = Color.FromArgb(192, 57, 43);
            return;
        }

        var product = Di.ProductService.GetByCode(code, Session.Company.Id);

        if (product is null)
        {
            _lblMessage.Text = $"No se encontró ningún producto con el código '{code}'.";
            _lblMessage.ForeColor = Color.FromArgb(192, 57, 43);
            ClearResult();
            return;
        }

        _lblMessage.Text = $"Resultado para '{code}'.";
        _lblMessage.ForeColor = Color.FromArgb(39, 174, 96);

        _lblName.Text = product.Name;
        _lblPlu.Text = product.Plu;
        _lblSku.Text = product.Sku;
        _lblCategory.Text = product.Category;
        _lblStatus.Text = product.Status;
        _lblStatus.ForeColor = product.IsLowStock ? Color.FromArgb(192, 57, 43) : Color.FromArgb(39, 174, 96);
        _lblCost.Text = product.Cost.ToString("C2");
        _lblPrice.Text = product.Price.ToString("C2");
        _lblStock.Text = product.Stock.ToString();
        _lblMinimum.Text = product.MinimumStock.ToString();
        _lblValue.Text = (product.Cost * product.Stock).ToString("C2");
        ShowImage(product);
    }

    private void ClearResult()
    {
        _lblName.Text = "—";
        _lblPlu.Text = "—";
        _lblSku.Text = "—";
        _lblCategory.Text = "—";
        _lblStatus.Text = "—";
        _lblStatus.ForeColor = Color.FromArgb(60, 60, 60);
        _lblCost.Text = "—";
        _lblPrice.Text = "—";
        _lblStock.Text = "—";
        _lblMinimum.Text = "—";
        _lblValue.Text = "—";
        ShowImage(null);
    }

    private void ShowImage(ProductDto? product)
    {
        _image?.Dispose();
        _image = product is null
            ? ProductImages.CreatePlaceholder("?", "Sin producto", 360, 360)
            : ProductImages.LoadForProduct(product.ImagePath, product.Plu, product.Sku)
              ?? ProductImages.CreatePlaceholder(product.Name, product.Category, 360, 360);

        _imgProduct.Image = _image;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _image?.Dispose();

        base.Dispose(disposing);
    }

    public void Activate()
    {
        Visible = true;
        _txtCode.Focus();
    }

    public void Deactivate() => Visible = false;
}
