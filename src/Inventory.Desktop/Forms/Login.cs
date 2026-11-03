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
        Text = "GesCan - Iniciar sesión";
        Size = new Size(460, 430);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9.5F);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Encabezado
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(44, 62, 80),
            Padding = new Padding(30, 22, 30, 18)
        };

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));

        var lblBrand = new Label
        {
            Text = "GesCan",
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft
        };

        var lblBrandSub = new Label
        {
            Text = "Sistema de Gestión Empresarial",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(189, 195, 199),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft
        };

        headerLayout.Controls.Add(lblBrand, 0, 0);
        headerLayout.Controls.Add(lblBrandSub, 0, 1);
        header.Controls.Add(headerLayout);

        // Formulario
        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(40, 30, 40, 28)
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105F));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

        var lblUser = new Label
        {
            Text = "Usuario",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(52, 73, 94)
        };

        txtUser = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            Margin = new Padding(0, 8, 0, 8)
        };

        var lblPass = new Label
        {
            Text = "Contraseña",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(52, 73, 94)
        };

        txtPass = new TextBox
        {
            Dock = DockStyle.Fill,
            PasswordChar = '●',
            Font = new Font("Segoe UI", 11F),
            Margin = new Padding(0, 8, 0, 8)
        };

        var lblHint = new Label
        {
            Text = "Demo: admin / 123456   •   ecano / 123456",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(149, 165, 166),
            Font = new Font("Segoe UI", 8.5F)
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 6, 0, 0)
        };

        var btnLogin = new Button
        {
            Text = "Ingresar",
            Width = 130,
            Height = 38,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += (_, _) => OnLogin();

        var btnCancel = new Button
        {
            Text = "Cancelar",
            Width = 110,
            Height = 38,
            DialogResult = DialogResult.Cancel,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.FromArgb(52, 73, 94),
            BackColor = Color.FromArgb(236, 240, 241)
        };
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(210, 218, 222);

        buttons.Controls.Add(btnLogin);
        buttons.Controls.Add(btnCancel);

        body.Controls.Add(lblUser, 0, 0);
        body.Controls.Add(txtUser, 1, 0);
        body.Controls.Add(lblPass, 0, 1);
        body.Controls.Add(txtPass, 1, 1);
        body.Controls.Add(lblHint, 0, 2);
        body.SetColumnSpan(lblHint, 2);
        body.Controls.Add(buttons, 0, 4);
        body.SetColumnSpan(buttons, 2);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(body, 0, 1);

        Controls.Add(root);
        AcceptButton = btnLogin;
        CancelButton = btnCancel;

        Shown += (_, _) => txtUser.Focus();
    }

    private void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
        {
            MessageBox.Show("Ingresá usuario y contraseña.", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var response = _authService.Login(new LoginRequest(txtUser.Text.Trim(), txtPass.Text));

            Session.User = new UserDto(
                response.UserId,
                response.Username,
                response.CompanyId,
                response.Role,
                response.EmployeeNumber,
                response.JobTitle);
            Session.Company = new CompanyDto(response.CompanyId, response.CompanyName);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error de acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtPass.SelectAll();
            txtPass.Focus();
        }
    }
}
