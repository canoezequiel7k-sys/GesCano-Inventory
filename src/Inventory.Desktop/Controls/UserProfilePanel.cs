using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Controls;

public partial class UserProfilePanel : UserControl
{
    private readonly PictureBox _avatar = new();
    private readonly Label _lblName = new();
    private readonly Label _lblDetails = new();
    private readonly Label _lblCompany = new();

    public UserProfilePanel()
    {
        InitializeComponent();
        RefreshData();
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(44, 62, 80);
        AutoScroll = true;
        Padding = new Padding(12);

        // Tarjeta superior en fila (modo row)
        var card = new Panel
        {
            Dock = DockStyle.Top,
            Height = 140,
            BackColor = Color.FromArgb(52, 73, 94),
            Padding = new Padding(12)
        };

        var cardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84F));
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Foto a la izquierda
        _avatar.Size = new Size(72, 72);
        _avatar.SizeMode = PictureBoxSizeMode.Zoom;
        _avatar.BackColor = Color.FromArgb(64, 90, 115);
        _avatar.Image = CreateAvatarPlaceholder("U");
        _avatar.Dock = DockStyle.Fill;
        _avatar.Margin = new Padding(0, 12, 12, 0);

        // Datos a la derecha, apilados verticalmente
        var dataPanel = new Panel { Dock = DockStyle.Fill };
        var dataLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        dataLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        dataLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        dataLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
        dataLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        dataLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _lblName.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        _lblName.ForeColor = Color.White;
        _lblName.TextAlign = ContentAlignment.BottomLeft;
        _lblName.Dock = DockStyle.Fill;

        var lblCaption = new Label
        {
            Text = "Usuario del Sistema",
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(149, 165, 166),
            TextAlign = ContentAlignment.TopLeft,
            Dock = DockStyle.Fill
        };

        _lblDetails.Font = new Font("Segoe UI", 8.5F);
        _lblDetails.ForeColor = Color.FromArgb(189, 195, 199);
        _lblDetails.TextAlign = ContentAlignment.TopLeft;
        _lblDetails.Dock = DockStyle.Fill;

        dataLayout.Controls.Add(_lblName, 0, 0);
        dataLayout.Controls.Add(lblCaption, 0, 1);
        dataLayout.Controls.Add(_lblDetails, 0, 2);
        dataPanel.Controls.Add(dataLayout);

        cardLayout.Controls.Add(_avatar, 0, 0);
        cardLayout.Controls.Add(dataPanel, 1, 0);
        card.Controls.Add(cardLayout);

        // Tarjeta de empresa
        var companyCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 82,
            BackColor = Color.FromArgb(52, 73, 94),
            Padding = new Padding(14, 12, 14, 12),
            Margin = new Padding(0, 10, 0, 0)
        };

        var companyLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        companyLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        companyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        companyLayout.Controls.Add(new Label
        {
            Text = "EMPRESA",
            Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(149, 165, 166),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        _lblCompany.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        _lblCompany.ForeColor = Color.White;
        _lblCompany.Dock = DockStyle.Fill;
        _lblCompany.TextAlign = ContentAlignment.TopLeft;
        companyLayout.Controls.Add(_lblCompany, 0, 1);

        companyCard.Controls.Add(companyLayout);

        Controls.Add(companyCard);
        Controls.Add(card);
    }

    private static Image CreateAvatarPlaceholder(string initial)
    {
        var bmp = new Bitmap(72, 72);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.FromArgb(64, 90, 115));
        using var brush = new SolidBrush(Color.White);
        using var font = new Font("Segoe UI", 20F, FontStyle.Bold);
        var text = string.IsNullOrWhiteSpace(initial) ? "U" : initial[..1].ToUpperInvariant();
        var size = g.MeasureString(text, font);
        g.DrawString(text, font, brush, (72 - size.Width) / 2f, (72 - size.Height) / 2f);
        return bmp;
    }

    private void RefreshData()
    {
        var user = Session.User;
        var company = Session.Company;

        if (user is not null)
        {
            _lblName.Text = user.Username;
            _lblDetails.Text = $"Legajo: {user.EmployeeNumber}  •  {TranslateRole(user.Role)}\n{user.JobTitle}";
            _avatar.Image?.Dispose();
            _avatar.Image = CreateAvatarPlaceholder(user.Username);
        }
        else
        {
            _lblName.Text = "Invitado";
            _lblDetails.Text = "Sin sesión activa";
            _avatar.Image = CreateAvatarPlaceholder("U");
        }

        _lblCompany.Text = company?.Name ?? "Sin empresa";
    }

    private static string TranslateRole(string role) =>
        role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ? "Administrador" : "Empleado";

    public void UpdateProfile()
    {
        RefreshData();
    }
}
