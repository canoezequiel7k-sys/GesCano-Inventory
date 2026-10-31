using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Forms;

public partial class Main : Form
{
    private MenuStrip menu = null!;
    private StatusStrip status = null!;
    private ToolStripStatusLabel lblCompany = null!;

    public Main()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "App de Inventario - Escritorio";
        Size = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;

        menu = new MenuStrip();
        var mProductos = new ToolStripMenuItem("Productos");
        mProductos.Click += (_, _) => OpenProducts();
        var mSalir = new ToolStripMenuItem("Salir");
        mSalir.Click += (_, _) => System.Windows.Forms.Application.Exit();
        menu.Items.AddRange(new ToolStripItem[] { mProductos, mSalir });
        MainMenuStrip = menu;
        Controls.Add(menu);

        status = new StatusStrip();
        lblCompany = new ToolStripStatusLabel("Empresa: ");
        status.Items.Add(lblCompany);
        Controls.Add(status);

        if (Session.Company is not null)
            lblCompany.Text = $"Empresa: {Session.Company.Name}";
    }

    private void OpenProducts()
    {
        var frm = new ProductList();
        frm.ShowDialog();
    }
}
