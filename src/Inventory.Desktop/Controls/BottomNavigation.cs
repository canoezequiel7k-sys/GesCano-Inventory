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
            ("inicio", "INICIO"),
            ("productos", "PRODUCTOS"),
            ("stock", "STOCK"),
            ("ventas", "VENTAS"),
            ("consultas", "CONSULTAS"),
            ("administracion", "ADMINISTRACIÓN")
        };

        int i = 0;
        foreach (var (key, text) in modules)
        {
            var btn = new Button
            {
                Dock = DockStyle.Fill,
                Text = text,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = key
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(221, 226, 230);
            btn.Click += (_, _) => OnModuleClicked(key);

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

    private void UpdateButtonStates()
    {
        foreach (var (key, btn) in _buttons)
        {
            bool isSelected = string.Equals(key, _selectedModule, StringComparison.OrdinalIgnoreCase);
            btn.BackColor = isSelected ? Color.FromArgb(52, 152, 219) : Color.Transparent;
            btn.ForeColor = isSelected ? Color.White : Color.FromArgb(128, 128, 128);
            btn.Font = new Font("Segoe UI", 9F, isSelected ? FontStyle.Bold : FontStyle.Regular);
        }
    }
}
