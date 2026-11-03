using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventory.Application.DTOs;

namespace Inventory.Desktop.Views;

public partial class HomeView : UserControl, IView
{
    private Label _lblProductsValue = null!;
    private Label _lblUnitsValue = null!;
    private Label _lblLowStockValue = null!;
    private Label _lblValueValue = null!;

    public string Title => "Inicio";

    public HomeView()
    {
        InitializeComponent();
        LoadStats();
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        Padding = new Padding(24, 18, 24, 18);
        AutoScroll = true;

        // El contenido usa filas con altura fija y AutoSize para que nada se
        // comprima cuando la ventana es chica; si no entra, aparece scroll.
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 3,
            Margin = new Padding(0)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 248F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 152F));

        // Encabezado
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
            Text = "PANEL DE INICIO",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            TextAlign = ContentAlignment.BottomLeft,
            Dock = DockStyle.Fill
        }, 0, 0);

        header.Controls.Add(new Label
        {
            Text = "Resumen general del inventario de la empresa.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(127, 140, 141),
            TextAlign = ContentAlignment.TopLeft,
            Dock = DockStyle.Fill
        }, 0, 1);

        // Tarjetas
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = new Padding(0, 8, 0, 0)
        };
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        cards.Controls.Add(CreateCard("PRODUCTOS ACTIVOS", "Productos en el catálogo.",
            Color.FromArgb(52, 152, 219), out _lblProductsValue), 0, 0);
        cards.Controls.Add(CreateCard("UNIDADES EN STOCK", "Unidades disponibles.",
            Color.FromArgb(46, 204, 113), out _lblUnitsValue), 1, 0);
        cards.Controls.Add(CreateCard("STOCK BAJO", "En o por debajo del mínimo.",
            Color.FromArgb(231, 76, 60), out _lblLowStockValue), 0, 1);
        cards.Controls.Add(CreateCard("VALOR DEL INVENTARIO", "Valorizado al costo.",
            Color.FromArgb(155, 89, 182), out _lblValueValue), 1, 1);

        // Bienvenida
        var welcome = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 247, 250),
            Padding = new Padding(20),
            Margin = new Padding(0, 12, 0, 0)
        };

        var welcomeText = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(90, 100, 110),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var user = Session.User?.Username ?? "usuario";
        welcomeText.Text = $"Bienvenido/a, {user}.\n\n" +
            "Gestioná Productos, consultá Stock y revisá Ventas desde la barra inferior. " +
            "También podés buscar información por PLU o SKU en Consultas.";

        welcome.Controls.Add(welcomeText);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(cards, 0, 1);
        root.Controls.Add(welcome, 0, 2);

        Controls.Add(root);
    }

    private static Panel CreateCard(string title, string subtitle, Color accent, out Label valueLabel)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 12, 12),
            Padding = new Padding(16, 12, 16, 12)
        };

        var accentBar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 5,
            BackColor = accent
        };

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(12, 0, 0, 0)
        };
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));

        content.Controls.Add(new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(127, 140, 141),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        valueLabel = new Label
        {
            Text = "—",
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            ForeColor = accent,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        content.Controls.Add(valueLabel, 0, 1);

        content.Controls.Add(new Label
        {
            Text = subtitle,
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(149, 165, 166),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft
        }, 0, 2);

        card.Controls.Add(content);
        card.Controls.Add(accentBar);

        return card;
    }

    private void LoadStats()
    {
        if (Session.Company is null)
            return;

        var products = Di.ProductService.GetByCompany(Session.Company.Id).ToList();

        _lblProductsValue.Text = products.Count.ToString();
        _lblUnitsValue.Text = products.Sum(p => p.Stock).ToString("N0");
        _lblLowStockValue.Text = products.Count(p => p.IsLowStock).ToString();
        _lblValueValue.Text = products.Sum(p => p.Cost * p.Stock).ToString("C0");
    }

    public void Activate()
    {
        Visible = true;
        LoadStats();
    }

    public void Deactivate() => Visible = false;
}
