using System.Drawing;
using System.Windows.Forms;
using Inventory.Desktop.Controls;
using Inventory.Desktop.Views;

namespace Inventory.Desktop;

public partial class MainShell : Form
{
    private readonly BottomNavigation _bottomNav = new();
    private readonly TerminalHeader _terminalHeader = new();
    private readonly UserProfilePanel _profilePanel = new();
    private readonly Panel _contentPanel = new();
    private readonly TableLayoutPanel _mainLayout = new();
    private readonly TableLayoutPanel _centerLayout = new();

    private IView? _currentView;
    private readonly Dictionary<string, Func<IView>> _viewFactory = new(StringComparer.OrdinalIgnoreCase);

    public MainShell()
    {
        Text = "Sistema de Gestión Empresarial - GESCOM";
        Size = new Size(1100, 700);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(236, 240, 241);

        RegisterViews();
        BuildLayout();
        WireEvents();

        // Módulo inicial
        NavigateTo("inicio");
    }

    private void RegisterViews()
    {
        _viewFactory["inicio"] = () => new HomeView();
        _viewFactory["productos"] = () => new ProductsView();
        _viewFactory["stock"] = () => new StockView();
        _viewFactory["ventas"] = () => new SalesView();
        _viewFactory["consultas"] = () => new QueriesView();
        _viewFactory["administracion"] = () => new AdministrationView();
    }

    private void BuildLayout()
    {
        // ===== TOP BAR =====
        var topBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(52, 73, 94),
            Padding = new Padding(12, 0, 12, 0)
        };

        var topLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        topLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var lblSystem = new Label
        {
            Text = "GesCan - Sistema de Gestión Empresarial",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill
        };

        var txtBuscar = new TextBox
        {
            Width = 220,
            PlaceholderText = "Buscar...",
            Font = new Font("Segoe UI", 9F),
            Anchor = AnchorStyles.Right | AnchorStyles.Top
        };

        topLayout.Controls.Add(lblSystem, 0, 0);
        topLayout.Controls.Add(txtBuscar, 1, 0);
        topBar.Controls.Add(topLayout);

        // ===== CENTER: PERFIL | TERMINAL =====
        _centerLayout.Dock = DockStyle.Fill;
        _centerLayout.ColumnCount = 2;
        _centerLayout.RowCount = 1;
        _centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
        _centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _centerLayout.Padding = new Padding(0);

        // Panel de perfil (izquierda)
        _profilePanel.Dock = DockStyle.Fill;
        _centerLayout.Controls.Add(_profilePanel, 0, 0);

        // Terminal (derecha)
        var terminalPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 245, 245)
        };
        var terminalLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        terminalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        terminalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
        terminalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _terminalHeader.Dock = DockStyle.Fill;
        terminalLayout.Controls.Add(_terminalHeader, 0, 0);

        _contentPanel.Dock = DockStyle.Fill;
        _contentPanel.BackColor = Color.White;
        terminalLayout.Controls.Add(_contentPanel, 0, 1);

        terminalPanel.Controls.Add(terminalLayout);
        _centerLayout.Controls.Add(terminalPanel, 1, 0);

        // ===== BOTTOM NAVIGATION =====
        _bottomNav.Dock = DockStyle.Bottom;

        // ===== ASSEMBLE =====
        _mainLayout.Dock = DockStyle.Fill;
        _mainLayout.ColumnCount = 1;
        _mainLayout.RowCount = 3;
        _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

        _mainLayout.Controls.Add(topBar, 0, 0);
        _mainLayout.Controls.Add(_centerLayout, 0, 1);
        _mainLayout.Controls.Add(_bottomNav, 0, 2);

        Controls.Add(_mainLayout);
    }

    private void WireEvents()
    {
        _bottomNav.ModuleSelected += NavigateTo;
    }

    private void NavigateTo(string moduleKey)
    {
        if (!_viewFactory.TryGetValue(moduleKey, out var factory))
            return;

        // Desactivar vista actual
        if (_currentView is not null)
        {
            _currentView.Deactivate();
            if (_currentView is Control currentCtrl)
                _contentPanel.Controls.Remove(currentCtrl);
        }

        // Activar nueva vista
        var next = factory();
        _currentView = next;
        _terminalHeader.SetModule(next.Title);

        if (next is Control nextCtrl)
        {
            nextCtrl.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(nextCtrl);
        }
        next.Activate();
    }
}
