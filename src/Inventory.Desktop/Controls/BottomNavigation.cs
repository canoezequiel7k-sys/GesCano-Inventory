using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Desktop.Controls;

public partial class BottomNavigation : UserControl
{
    private readonly TableLayoutPanel _layout = new();
    private readonly Dictionary<string, Button> _buttons = new(StringComparer.OrdinalIgnoreCase);
    private string _selectedModule = string.Empty;

    public event Action<string>? ModuleSelected;

    public BottomNavigation()
    {
        InitializeComponent();
        CreateButtons();
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Bottom;
        Height = 60;
        BackColor = Color.FromArgb(236, 240, 241);

        _layout.Dock = DockStyle.Fill;
        _layout.ColumnCount = 6;
        _layout.RowCount = 1;
        for (int i = 0; i < 6; i++)
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 6F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layout.Padding = new Padding(8, 4, 8, 4);

        Controls.Add(_layout);
    }

    private void CreateButtons()
    {
        var modules = new[]
        {
            ("inicio", "INICIO", true),
            ("productos", "PRODUCTOS", true),
            ("stock", "STOCK", true),
            ("ventas", "VENTAS", true),
            ("consultas", "CONSULTAS", true),
            ("administracion", "ADMINISTRACIÓN", false)
        };

        var toolTip = new ToolTip();

        int i = 0;
        foreach (var (key, text, available) in modules)
        {
            var btn = new Button
            {
                Dock = DockStyle.Fill,
                Text = text,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Cursor = available ? Cursors.Hand : Cursors.Default,
                Enabled = available,
                Tag = key
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(221, 226, 230);

            if (available)
                btn.Click += (_, _) => OnModuleClicked(key);
            else
                toolTip.SetToolTip(btn, "No disponible por ahora");

            _buttons[key] = btn;
            _layout.Controls.Add(btn, i, 0);
            i++;
        }

        UpdateButtonStates();
    }

    private void OnModuleClicked(string key)
    {
        _selectedModule = key;
        UpdateButtonStates();
        ModuleSelected?.Invoke(key);
    }

    public void SetSelected(string key)
    {
        _selectedModule = key;
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        foreach (var (key, btn) in _buttons)
        {
            if (!btn.Enabled)
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(189, 195, 199);
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                continue;
            }

            bool isSelected = string.Equals(key, _selectedModule, StringComparison.OrdinalIgnoreCase);
            btn.BackColor = isSelected ? Color.FromArgb(52, 152, 219) : Color.Transparent;
            btn.ForeColor = isSelected ? Color.White : Color.FromArgb(128, 128, 128);
            btn.Font = new Font("Segoe UI", 9F, isSelected ? FontStyle.Bold : FontStyle.Regular);
        }
    }
}
