using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Views;

public partial class AdministrationView : UserControl, IView
{
    public string Title => "Administración";

    public AdministrationView()
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
            RowCount = 2
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
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
            Text = "ADMINISTRACIÓN",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            TextAlign = ContentAlignment.BottomLeft,
            Dock = DockStyle.Fill
        }, 0, 0);

        header.Controls.Add(new Label
        {
            Text = "Usuarios, empresas y configuración general del sistema.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(127, 140, 141),
            TextAlign = ContentAlignment.TopLeft,
            Dock = DockStyle.Fill
        }, 0, 1);

        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(28)
        };

        var cardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));

        cardLayout.Controls.Add(new Label
        {
            Text = "🔒",
            Font = new Font("Segoe UI Emoji", 34F),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomCenter
        }, 0, 0);

        cardLayout.Controls.Add(new Label
        {
            Text = "No disponible por ahora",
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        }, 0, 1);

        cardLayout.Controls.Add(new Label
        {
            Text = "Este módulo se habilitará en una próxima versión. La barra inferior\n" +
                   "lo muestra deshabilitado para dejar claro que todavía no está listo.",
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(127, 140, 141),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopCenter
        }, 0, 2);

        card.Controls.Add(cardLayout);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(card, 0, 1);

        Controls.Add(root);
    }

    public void Activate() => Visible = true;
    public void Deactivate() => Visible = false;
}
