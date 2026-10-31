using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Controls;

public partial class TerminalHeader : UserControl
{
    private readonly Label _lblTitle = new();
    private readonly Label _lblSubtitle = new();

    public TerminalHeader()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Top;
        Height = 60;
        BackColor = Color.White;
        Padding = new Padding(20, 0, 20, 0);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        _lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
        _lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        _lblTitle.Dock = DockStyle.Fill;
        _lblTitle.Text = "TERMINAL DE TRABAJO";

        _lblSubtitle.Font = new Font("Segoe UI", 9F);
        _lblSubtitle.ForeColor = Color.FromArgb(128, 128, 128);
        _lblSubtitle.TextAlign = ContentAlignment.MiddleRight;
        _lblSubtitle.Dock = DockStyle.Fill;
        _lblSubtitle.Text = "Sistema de Gestión Empresarial";

        layout.Controls.Add(_lblTitle, 0, 0);
        layout.Controls.Add(_lblSubtitle, 1, 0);

        Controls.Add(layout);
    }

    public void SetModule(string moduleTitle)
    {
        _lblTitle.Text = moduleTitle;
    }
}
