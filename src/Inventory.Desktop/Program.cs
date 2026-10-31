using Inventory.Desktop.Forms;
using Inventory.Infrastructure.Persistence;

namespace Inventory.Desktop;

static class Program
{
    [STAThread]
    static void Main()
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        using var seedDb = new AppDbContext();
        DatabaseInitializer.EnsureSeed(seedDb, Di.PasswordHasher);

        if (new Login(Di.AuthService).ShowDialog() == DialogResult.OK)
        {
            System.Windows.Forms.Application.Run(new MainShell());
        }
    }
}
