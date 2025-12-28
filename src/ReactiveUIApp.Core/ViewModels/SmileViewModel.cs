using ReactiveUI;
using ReactiveUIApp.Core.Extensions;
using Splat;

namespace ReactiveUIApp.Core.ViewModels;

public partial class SmileViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "smile";

    public IScreen HostScreen { get; }

    public SmileViewModel(IScreen? screen = null)
    {
        HostScreen = Locator.Current.SetValueOrGetRequiredService(screen);
    }
}
