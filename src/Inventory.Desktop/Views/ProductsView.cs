using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventory.Application.DTOs;
using Inventory.Desktop;

namespace Inventory.Desktop.Views;

public partial class ProductsView : UserControl, IView
{
    private DataGridView _dgv = null!;
    private BindingSource _bs = new();

    public string Title => "Productos";

    public ProductsView()
    {
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        Padding = new Padding(20);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var lblTitle = new Label
        {
            Text = "GESTIÓN DE PRODUCTOS",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(52, 73, 94),
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Height = 40
        };

        _dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false
        };
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sku", HeaderText = "SKU", DataPropertyName = "Sku", Width = 120 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nombre", DataPropertyName = "Name", Width = 250 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Categoría", DataPropertyName = "Category", Width = 150 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Precio", DataPropertyName = "Price", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C" } });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock", Width = 80 });

        layout.Controls.Add(lblTitle, 0, 0);
        layout.Controls.Add(_dgv, 0, 1);

        Controls.Add(layout);
    }

    private void LoadData()
    {
        if (Session.Company is null) return;
        var list = Di.ProductService.GetByCompany(Session.Company.Id).ToList();
        _bs.DataSource = list;
        _dgv.DataSource = _bs;
    }

    public void Activate()
    {
        Visible = true;
        LoadData();
    }

    public void Deactivate() => Visible = false;
}
