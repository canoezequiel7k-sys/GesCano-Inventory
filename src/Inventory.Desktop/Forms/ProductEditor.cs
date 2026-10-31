using System.Drawing;
using System.Windows.Forms;
using Inventory.Application.DTOs;

namespace Inventory.Desktop.Forms;

public partial class ProductEditor : Form
{
    private readonly int _companyId;
    private readonly ProductDto? _editing;
    private TextBox txtSku = null!;
    private TextBox txtName = null!;
    private TextBox txtCategory = null!;
    private TextBox txtPrice = null!;
    private TextBox txtStock = null!;

    public ProductEditor(int companyId, ProductDto? dto = null)
    {
        _companyId = companyId;
        _editing = dto;
        InitializeComponent();
        if (_editing is not null) Fill();
    }

    private void InitializeComponent()
    {
        Text = _editing is null ? "Nuevo producto" : "Editar producto";
        Size = new Size(420, 340);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6, Padding = new Padding(12) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var fields = new[] { ("SKU:", "Sku"), ("Nombre:", "Name"), ("Categoría:", "Category"), ("Precio:", "Price"), ("Stock:", "Stock") };
        var boxes = new Dictionary<string, TextBox>();
        int row = 0;
        foreach (var (labelText, name) in fields)
        {
            layout.Controls.Add(new Label { Text = labelText, Dock = DockStyle.Fill, Anchor = AnchorStyles.Left }, 0, row);
            var box = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(box, 1, row);
            boxes[name] = box;
            row++;
        }
        txtSku = boxes["Sku"]; txtName = boxes["Name"]; txtCategory = boxes["Category"];
        txtPrice = boxes["Price"]; txtStock = boxes["Stock"];

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };
        var btnSave = new Button { Text = "Guardar", DialogResult = DialogResult.OK };
        btnSave.Click += (_, _) => Save();
        btnPanel.Controls.Add(btnCancel);
        btnPanel.Controls.Add(btnSave);

        layout.Controls.Add(btnPanel, 0, row);
        layout.SetColumnSpan(btnPanel, 2);

        Controls.Add(layout);
        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }

    private void Fill()
    {
        txtSku.Text = _editing!.Sku;
        txtName.Text = _editing.Name;
        txtCategory.Text = _editing.Category;
        txtPrice.Text = _editing.Price.ToString();
        txtStock.Text = _editing.Stock.ToString();
    }

    private void Save()
    {
        try
        {
            var sku = txtSku.Text.Trim();
            var name = txtName.Text.Trim();
            var category = txtCategory.Text.Trim();
            var price = decimal.Parse(txtPrice.Text);
            var stock = int.Parse(txtStock.Text);

            if (_editing is null)
                Di.ProductService.Create(_companyId, new CreateProductDto(sku, name, category, price, stock));
            else
                Di.ProductService.Update(_editing.Id, _companyId, new UpdateProductDto(sku, name, category, price, stock));

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
