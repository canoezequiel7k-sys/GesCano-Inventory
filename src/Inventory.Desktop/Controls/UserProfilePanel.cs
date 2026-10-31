using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Controls;

public partial class UserProfilePanel : UserControl
{
    private readonly PictureBox _avatar = new();
    private readonly Label _lblName = new();
    private readonly Label _lblLegajo = new();
    private readonly Label _lblBranch = new();
    private readonly Label _lblUser = new();

    public UserProfilePanel()
    {
        InitializeComponent();
        RefreshData();
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(44, 62, 80);
        Padding = new Padding(16, 20, 16, 20);
        AutoScroll = true;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _avatar.Size = new Size(96, 96);
        _avatar.SizeMode = PictureBoxSizeMode.Zoom;
        _avatar.BackColor = Color.FromArgb(52, 73, 94);
        _avatar.Image = CreateAvatarPlaceholder();

        _lblName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        _lblName.ForeColor = Color.White;
        _lblName.TextAlign = ContentAlignment.MiddleCenter;
        _lblName.Dock = DockStyle.Fill;

        _lblLegajo.Font = new Font("Segoe UI", 9F);
        _lblLegajo.ForeColor = Color.FromArgb(189, 195, 199);
        _lblLegajo.TextAlign = ContentAlignment.MiddleCenter;
        _lblLegajo.Dock = DockStyle.Fill;

        _lblBranch.Font = new Font("Segoe UI", 9F);
        _lblBranch.ForeColor = Color.FromArgb(189, 195, 199);
        _lblBranch.TextAlign = ContentAlignment.MiddleCenter;
        _lblBranch.Dock = DockStyle.Fill;

        _lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        _lblUser.ForeColor = Color.FromArgb(144, 164, 174);
        _lblUser.TextAlign = ContentAlignment.MiddleCenter;
        _lblUser.Dock = DockStyle.Fill;

        layout.Controls.Add(_avatar, 0, 0);
        layout.Controls.Add(_lblName, 0, 1);
        layout.Controls.Add(_lblLegajo, 0, 2);
        layout.Controls.Add(_lblBranch, 0, 3);
        layout.Controls.Add(_lblUser, 0, 4);

        Controls.Add(layout);
    }

    private Image CreateAvatarPlaceholder()
    {
        var bmp = new Bitmap(96, 96);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.FromArgb(52, 73, 94));
        using var brush = new SolidBrush(Color.White);
        using var font = new Font("Segoe UI", 24F, FontStyle.Bold);
        var text = "U";
        var size = g.MeasureString(text, font);
        g.DrawString(text, font, brush, (96 - size.Width) / 2f, (96 - size.Height) / 2f);
        return bmp;
    }

    private void RefreshData()
    {
        var user = Session.User;
        _lblName.Text = user is not null ? "Usuario del Sistema" : "Invitado";
        _lblLegajo.Text = "Legajo: 80";
        _lblBranch.Text = "Sucursal: 80";
        _lblUser.Text = user is not null ? $"Usuario: {user.Username}" : "Usuario: --";
    }

    public void UpdateProfile()
    {
        RefreshData();
    }
}
