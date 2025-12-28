using ReactiveUI;
using ReactiveUIApp.Core.Extensions;
using ReactiveUIApp.Core.ViewModels;
using Splat;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ReactiveUIApp.Uwp.Views;

public abstract class ReactiveSmileView : ReactiveUserControl<SmileViewModel>;

public sealed partial class SmileView : ReactiveSmileView
{
    public SmileView(SmileViewModel? viewModel = null)
    {
        ViewModel = Locator.Current.SetValueOrGetRequiredService(viewModel);
        this.InitializeComponent();
    }


}
