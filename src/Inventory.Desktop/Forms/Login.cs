using System.Drawing;
using System.Windows.Forms;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;

namespace Inventory.Desktop.Forms;

public partial class Login : Form
{
    private readonly IAuthService _authService;
    private TextBox txtUser = null!;
    private TextBox txtPass = null!;

    public Login(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Iniciar sesión";
        Size = new Size(400, 260);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4, Padding = new Padding(20) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var lblUser = new Label { Text = "Usuario:", Dock = DockStyle.Fill, Anchor = AnchorStyles.Left };
        txtUser = new TextBox { Dock = DockStyle.Fill };
        var lblPass = new Label { Text = "Contraseña:", Dock = DockStyle.Fill, Anchor = AnchorStyles.Left };
        txtPass = new TextBox { Dock = DockStyle.Fill, PasswordChar = '*' };

        layout.Controls.Add(lblUser, 0, 0);
        layout.Controls.Add(txtUser, 1, 0);
        layout.Controls.Add(lblPass, 0, 1);
        layout.Controls.Add(txtPass, 1, 1);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };
        var btnLogin = new Button { Text = "Iniciar", DialogResult = DialogResult.OK };
        btnLogin.Click += (_, _) => OnLogin();
        btnPanel.Controls.Add(btnCancel);
        btnPanel.Controls.Add(btnLogin);

        layout.Controls.Add(btnPanel, 0, 2);
        layout.SetColumnSpan(btnPanel, 2);

        Controls.Add(layout);
        AcceptButton = btnLogin;
        CancelButton = btnCancel;
    }

    private void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
        {
            MessageBox.Show("Ingresa usuario y contraseña.", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var response = _authService.Login(new LoginRequest(txtUser.Text.Trim(), txtPass.Text));
            Session.User = new UserDto(response.UserId, response.Username, response.CompanyId, response.Role);
            Session.Company = new CompanyDto(response.CompanyId, response.CompanyName);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error de acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
