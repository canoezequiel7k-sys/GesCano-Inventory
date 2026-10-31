using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Views;

public partial class StockView : UserControl, IView
{
    public string Title => "Control de Stock";

    public StockView()
    {
        InitializeComponent();
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
            Text = "CONTROL DE STOCK",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(52, 73, 94),
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Height = 40
        };

        var lblSub = new Label
        {
            Text = "Módulo de control de inventario y movimientos de stock. Funcionalidad en desarrollo.",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(128, 128, 128),
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Height = 60
        };

        layout.Controls.Add(lblTitle, 0, 0);
        layout.Controls.Add(lblSub, 0, 1);

        Controls.Add(layout);
    }

    public void Activate() => Visible = true;
    public void Deactivate() => Visible = false;
}
