using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventory.Application.DTOs;

namespace Inventory.Desktop.Views;

public partial class StockView : UserControl, IView
{
    private DataGridView _dgv = null!;
    private TextBox _txtSearch = null!;
    private CheckBox _chkLowStock = null!;
    private Label _lblCount = null!;
    private List<ProductDto> _allItems = new();

    public string Title => "Control de Stock";

    public StockView()
    {
        InitializeComponent();
        LoadData();
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
            RowCount = 4
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

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
            Text = "CONTROL DE STOCK",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            TextAlign = ContentAlignment.BottomLeft,
            Dock = DockStyle.Fill
        }, 0, 0);

        header.Controls.Add(new Label
        {
            Text = "Consultá disponibilidad por PLU o SKU y detectá faltantes.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(127, 140, 141),
            TextAlign = ContentAlignment.TopLeft,
            Dock = DockStyle.Fill
        }, 0, 1);

        var toolbar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 6, 0, 6)
        };
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _txtSearch = new TextBox
        {
            Dock = DockStyle.Fill,
            PlaceholderText = "Buscar por PLU, SKU o nombre...",
            Font = new Font("Segoe UI", 10F),
            Margin = new Padding(0, 4, 16, 4)
        };
        _txtSearch.TextChanged += (_, _) => ApplyFilter();

        _chkLowStock = new CheckBox
        {
            Text = "Solo stock bajo",
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(52, 73, 94),
            Margin = new Padding(0, 9, 0, 0)
        };
        _chkLowStock.CheckedChanged += (_, _) => ApplyFilter();

        toolbar.Controls.Add(_txtSearch, 0, 0);
        toolbar.Controls.Add(_chkLowStock, 1, 0);

        _dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            GridColor = Color.FromArgb(236, 240, 241),
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            RowTemplate = { Height = 34 },
            ColumnHeadersHeight = 38,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };

        _dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
        _dgv.DefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
        _dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 234, 248);
        _dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(44, 62, 80);
        _dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 252);
        _dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
        _dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Plu", HeaderText = "PLU", DataPropertyName = "Plu", Width = 85 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sku", HeaderText = "SKU", DataPropertyName = "Sku", Width = 115 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Producto", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 220 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Categoría", DataPropertyName = "Category", Width = 130 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "MinimumStock", HeaderText = "Mínimo", DataPropertyName = "MinimumStock", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Estado", DataPropertyName = "Status", Width = 110 });

        _dgv.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _dgv.Rows[e.RowIndex].DataBoundItem is not ProductDto dto)
                return;

            if (e.ColumnIndex == _dgv.Columns["Status"]!.Index)
            {
                e.CellStyle!.ForeColor = dto.IsLowStock ? Color.FromArgb(192, 57, 43) : Color.FromArgb(39, 174, 96);
                e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
            else if (e.ColumnIndex is int col && (col == _dgv.Columns["Stock"]!.Index || col == _dgv.Columns["MinimumStock"]!.Index))
            {
                e.CellStyle!.ForeColor = dto.IsLowStock ? Color.FromArgb(192, 57, 43) : Color.FromArgb(60, 60, 60);
            }
        };

        _lblCount = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(127, 140, 141)
        };

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(toolbar, 0, 1);
        root.Controls.Add(_dgv, 0, 2);
        root.Controls.Add(_lblCount, 0, 3);

        Controls.Add(root);
    }

    private void LoadData()
    {
        if (Session.Company is null)
            return;

        _allItems = Di.ProductService.GetByCompany(Session.Company.Id)
            .OrderBy(p => p.Name)
            .ToList();

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var term = _txtSearch?.Text?.Trim() ?? string.Empty;

        IEnumerable<ProductDto> filtered = _allItems;

        if (!string.IsNullOrWhiteSpace(term))
        {
            filtered = filtered.Where(p =>
                p.Plu.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (_chkLowStock?.Checked == true)
            filtered = filtered.Where(p => p.IsLowStock);

        var list = filtered.ToList();

        _dgv.DataSource = null;
        _dgv.DataSource = list;

        if (_lblCount is not null)
            _lblCount.Text = $"{list.Count} producto(s) listados   •   {list.Count(p => p.IsLowStock)} con stock bajo";
    }

    public void SetSearch(string term)
    {
        _txtSearch.Text = term;
    }

    public void Activate()
    {
        Visible = true;
        LoadData();
    }

    public void Deactivate() => Visible = false;
}
