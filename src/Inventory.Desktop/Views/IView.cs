namespace Inventory.Desktop.Views;

public interface IView
{
    string Title { get; }
    void Activate();
    void Deactivate();
}
