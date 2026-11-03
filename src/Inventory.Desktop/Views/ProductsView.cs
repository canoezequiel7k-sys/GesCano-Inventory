using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventory.Application.DTOs;
using Inventory.Desktop.Forms;

namespace Inventory.Desktop.Views;

public partial class ProductsView : UserControl, IView
{
    private DataGridView _dgv = null!;
    private TextBox _txtSearch = null!;
    private Label _lblCount = null!;
    private List<ProductDto> _allItems = new();

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
            Text = "PRODUCTOS",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            TextAlign = ContentAlignment.BottomLeft,
            Dock = DockStyle.Fill
        }, 0, 0);

        header.Controls.Add(new Label
        {
            Text = "Catálogo de solo lectura. Hacé doble clic en un producto para ver su detalle.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(127, 140, 141),
            TextAlign = ContentAlignment.TopLeft,
            Dock = DockStyle.Fill
        }, 0, 1);

        // Barra de acciones
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

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            Margin = new Padding(0)
        };

        var btnNew = CreateActionButton("Nuevo", Color.FromArgb(46, 204, 113));
        btnNew.Click += (_, _) => AddProduct();

        var btnPreview = CreateActionButton("Ver detalle", Color.FromArgb(52, 152, 219));
        btnPreview.Click += (_, _) => PreviewProduct();

        var btnDelete = CreateActionButton("Eliminar", Color.FromArgb(231, 76, 60));
        btnDelete.Click += (_, _) => DeleteProduct();

        var btnRefresh = CreateActionButton("Actualizar", Color.FromArgb(127, 140, 141));
        btnRefresh.Click += (_, _) => LoadData();

        actions.Controls.Add(btnNew);
        actions.Controls.Add(btnPreview);
        actions.Controls.Add(btnDelete);
        actions.Controls.Add(btnRefresh);

        toolbar.Controls.Add(_txtSearch, 0, 0);
        toolbar.Controls.Add(actions, 1, 0);

        // Grilla
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
        _dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
        _dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 252);
        _dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
        _dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Plu", HeaderText = "PLU", DataPropertyName = "Plu", Width = 80 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sku", HeaderText = "SKU", DataPropertyName = "Sku", Width = 110 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nombre", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 220 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Categoría", DataPropertyName = "Category", Width = 130 });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cost", HeaderText = "Costo", DataPropertyName = "Cost", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Precio", DataPropertyName = "Price", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock", Width = 70, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "MinimumStock", HeaderText = "Mínimo", DataPropertyName = "MinimumStock", Width = 70, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Estado", DataPropertyName = "Status", Width = 105 });

        _dgv.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
                PreviewProduct();
        };

        _dgv.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _dgv.Rows[e.RowIndex].DataBoundItem is not ProductDto dto)
                return;

            if (e.ColumnIndex == _dgv.Columns["Status"]!.Index)
            {
                e.CellStyle!.ForeColor = dto.IsLowStock ? Color.FromArgb(192, 57, 43) : Color.FromArgb(39, 174, 96);
                e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
            else if (e.ColumnIndex == _dgv.Columns["Stock"]!.Index)
            {
                e.CellStyle!.ForeColor = dto.IsLowStock ? Color.FromArgb(192, 57, 43) : Color.FromArgb(60, 60, 60);
                e.CellStyle.Font = new Font("Segoe UI", 9F, dto.IsLowStock ? FontStyle.Bold : FontStyle.Regular);
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

    private static Button CreateActionButton(string text, Color backColor) => new()
    {
        Text = text,
        Width = 104,
        Height = 34,
        BackColor = backColor,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
        Cursor = Cursors.Hand,
        Margin = new Padding(6, 0, 0, 0),
        UseVisualStyleBackColor = false
    };

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

        var filtered = string.IsNullOrWhiteSpace(term)
            ? _allItems
            : _allItems.Where(p =>
                p.Plu.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

        _dgv.DataSource = null;
        _dgv.DataSource = filtered;

        if (_lblCount is not null)
        {
            var lowStock = filtered.Count(p => p.IsLowStock);
            _lblCount.Text = $"{filtered.Count} producto(s) mostrados" +
                (lowStock > 0 ? $"   •   {lowStock} con stock bajo" : string.Empty);
        }
    }

    public void SetSearch(string term)
    {
        _txtSearch.Text = term;
    }

    private void AddProduct()
    {
        if (Session.Company is null)
            return;

        var editor = new ProductEditor(Session.Company.Id);
        if (editor.ShowDialog(FindForm()) == DialogResult.OK)
            LoadData();
    }

    private void PreviewProduct()
    {
        if (_dgv.CurrentRow?.DataBoundItem is not ProductDto dto)
            return;

        using var preview = new ProductPreview(dto);
        preview.ShowDialog(FindForm());
    }

    private void DeleteProduct()
    {
        if (Session.Company is null || _dgv.CurrentRow?.DataBoundItem is not ProductDto dto)
            return;

        var confirm = MessageBox.Show(
            $"¿Eliminar el producto '{dto.Name}'?\nEsta acción también borra su historial de stock.",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
            return;

        try
        {
            Di.ProductService.Delete(dto.Id, Session.Company.Id);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void Activate()
    {
        Visible = true;
        LoadData();
    }

    public void Deactivate() => Visible = false;
}
