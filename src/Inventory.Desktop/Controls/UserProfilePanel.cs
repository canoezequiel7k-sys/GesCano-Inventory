using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Controls;

public partial class UserProfilePanel : UserControl
{
    private readonly PictureBox _avatar = new();
    private readonly Label _lblName = new();
    private readonly Label _lblDetails = new();

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
            Height = 96,
            BackColor = Color.FromArgb(52, 73, 94),
            Padding = new Padding(12)
        };

        var cardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Foto a la izquierda
        _avatar.Size = new Size(72, 72);
        _avatar.SizeMode = PictureBoxSizeMode.Zoom;
        _avatar.BackColor = Color.FromArgb(64, 90, 115);
        _avatar.Image = CreateAvatarPlaceholder();
        _avatar.Dock = DockStyle.Fill;
        _avatar.Margin = new Padding(0, 0, 12, 0);

        // Datos a la derecha, apilados verticalmente
        var dataPanel = new Panel { Dock = DockStyle.Fill };
        var dataLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        dataLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        dataLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        dataLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        dataLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _lblName.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblName.ForeColor = Color.White;
        _lblName.TextAlign = ContentAlignment.MiddleLeft;
        _lblName.Dock = DockStyle.Fill;

        _lblDetails.Font = new Font("Segoe UI", 8.5F);
        _lblDetails.ForeColor = Color.FromArgb(189, 195, 199);
        _lblDetails.TextAlign = ContentAlignment.MiddleLeft;
        _lblDetails.Dock = DockStyle.Fill;

        dataLayout.Controls.Add(_lblName, 0, 0);
        dataLayout.Controls.Add(_lblDetails, 0, 1);
        dataPanel.Controls.Add(dataLayout);

        cardLayout.Controls.Add(_avatar, 0, 0);
        cardLayout.Controls.Add(dataPanel, 1, 0);
        card.Controls.Add(cardLayout);

        Controls.Add(card);
    }

    private Image CreateAvatarPlaceholder()
    {
        var bmp = new Bitmap(72, 72);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.FromArgb(64, 90, 115));
        using var brush = new SolidBrush(Color.White);
        using var font = new Font("Segoe UI", 20F, FontStyle.Bold);
        var text = "U";
        var size = g.MeasureString(text, font);
        g.DrawString(text, font, brush, (72 - size.Width) / 2f, (72 - size.Height) / 2f);
        return bmp;
    }

    private void RefreshData()
    {
        var user = Session.User;
        _lblName.Text = user is not null ? "Usuario del Sistema" : "Invitado";
        _lblDetails.Text = user is not null
            ? $"Legajo: 80    Sucursal: 80    Tipo: {user.Role}"
            : "Legajo: --    Sucursal: --    Tipo: --";
    }

    public void UpdateProfile()
    {
        RefreshData();
    }
}
