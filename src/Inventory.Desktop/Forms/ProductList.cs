using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventory.Application.DTOs;

namespace Inventory.Desktop.Forms;

public partial class ProductList : Form
{
    private DataGridView dgv = null!;
    private ToolStrip toolStrip = null!;
    private BindingSource bs = new();

    public ProductList()
    {
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Text = Session.Company is not null ? $"Productos - {Session.Company.Name}" : "Productos";
        Size = new Size(900, 520);
        StartPosition = FormStartPosition.CenterParent;

        toolStrip = new ToolStrip();
        var btnAdd = new ToolStripButton("Agregar");
        btnAdd.Click += (_, _) => AddProduct();
        var btnEdit = new ToolStripButton("Editar");
        btnEdit.Click += (_, _) => EditProduct();
        var btnDel = new ToolStripButton("Eliminar");
        btnDel.Click += (_, _) => DeleteProduct();
        var btnRefresh = new ToolStripButton("Actualizar");
        btnRefresh.Click += (_, _) => LoadData();
        var btnLogout = new ToolStripButton("Cerrar sesión");
        btnLogout.Click += (_, _) => Logout();
        toolStrip.Items.AddRange(new ToolStripItem[] { btnAdd, btnEdit, btnDel, btnRefresh, btnLogout });
        Controls.Add(toolStrip);

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sku", HeaderText = "SKU", DataPropertyName = "Sku" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nombre", DataPropertyName = "Name" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Categoría", DataPropertyName = "Category" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Precio", DataPropertyName = "Price", DefaultCellStyle = new DataGridViewCellStyle { Format = "C" } });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock" });
        Controls.Add(dgv);
    }

    private void LoadData()
    {
        if (Session.Company is null) return;
        var list = Di.ProductService.GetByCompany(Session.Company.Id).ToList();
        bs.DataSource = list;
        dgv.DataSource = bs;
    }

    private void AddProduct()
    {
        var editor = new ProductEditor(Session.Company!.Id);
        if (editor.ShowDialog() == DialogResult.OK) LoadData();
    }

    private void EditProduct()
    {
        if (dgv.CurrentRow?.DataBoundItem is not ProductDto dto) return;
        var editor = new ProductEditor(Session.Company!.Id, dto);
        if (editor.ShowDialog() == DialogResult.OK) LoadData();
    }

    private void DeleteProduct()
    {
        if (dgv.CurrentRow?.DataBoundItem is not ProductDto dto) return;
        if (MessageBox.Show($"¿Eliminar el producto '{dto.Name}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                Di.ProductService.Delete(dto.Id, Session.Company!.Id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void Logout()
    {
        Session.Clear();
        Close();
    }
}
