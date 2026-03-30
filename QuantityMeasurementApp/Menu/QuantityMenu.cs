using QuantityMeasurementApp.Controllers;
using QuantityMeasurementApp.Interfaces;

namespace QuantityMeasurementApp.Menu;

// UC16: Thin IMenu implementation — delegates Run() to the controller.
public class QuantityMenu : IMenu
{
    private readonly QuantityMeasurementController _controller;

    public QuantityMenu(QuantityMeasurementController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    public void Run() => _controller.Run();
}
