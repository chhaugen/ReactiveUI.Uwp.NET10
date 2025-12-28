using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUIApp.Core.Extensions;
using ReactiveUIApp.Core.Models;
using System;
using Splat;
using System.Reactive;
using System.Reactive.Threading.Tasks;

namespace ReactiveUIApp.Core.ViewModels;

public partial class InkCanvasViewModel : ReactiveObject, IRoutableViewModel
{
    [Reactive]
    private InkDataModel? _inkData;

    public string? UrlPathSegment => "inkCanvas";

    public IScreen HostScreen { get; }

    public Interaction<byte[], Unit> ShowSaveFilePicker { get; } = new();

    public Interaction<Unit, byte[]> ShowOpenFilePicker { get; } = new();

    public InkCanvasViewModel(IScreen? screen = null)
    {
        HostScreen = Locator.Current.SetValueOrGetRequiredService(screen);
    }

    [ReactiveCommand]
    private async Task SaveAsync()
    {
        if (InkData is null)
            return;

        await ShowSaveFilePicker.Handle(InkData.GifData).ToTask();
    }

    [ReactiveCommand]
    private async Task OpenAsync()
    {
        var bytes = await ShowOpenFilePicker.Handle(Unit.Default).ToTask();
        var hash = bytes.Length == 0
            ? bytes.GetHashCode()
            : bytes
                .Select(x => x.GetHashCode())
                .Aggregate(bytes.GetHashCode(), (current, next) => HashCode.Combine(current, next));

        InkData = new(bytes, bytes.GetHashCode());
    }
}
