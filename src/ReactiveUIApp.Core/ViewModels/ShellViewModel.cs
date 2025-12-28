using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUIApp.Core.ViewModels;

public partial class ShellViewModel : ReactiveObject, IScreen
{
    [Reactive]
    private SmileViewModel _smileViewModel;

    [Reactive]
    private InkCanvasViewModel _inkCanvasViewModel;

    public RoutingState Router { get; }

    [RequiresDynamicCode("RoutingState uses RxApp and ReactiveCommand which require dynamic code generation")]
    [RequiresUnreferencedCode("RoutingState uses RxApp and ReactiveCommand which may require unreferenced code")]
    public ShellViewModel()
    {
        _smileViewModel = new SmileViewModel(this);
        _inkCanvasViewModel = new InkCanvasViewModel(this);
        Router = new RoutingState();
    }
}
