using ReactiveUI;
using ReactiveUIApp.Core.Extensions;
using ReactiveUIApp.Core.Models;
using ReactiveUIApp.Core.ViewModels;
using ReactiveUIApp.Uwp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Input.Inking;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ReactiveUIApp.Uwp.Views;

public abstract class ReactiveInkCanvasView : ReactivePage<InkCanvasViewModel>;

public sealed partial class InkCanvasView : ReactiveInkCanvasView
{
    private readonly InkPresenter _inkPresenter;

    public InkCanvasView()
    {
        this.InitializeComponent();

        _inkPresenter = inkCanvas.InkPresenter;
        _inkPresenter.InputDeviceTypes =
            CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;

        this.WhenActivated(disposables =>
        {
            IObservable<InkDataModel> inkFromView = Observable
                .Merge
                (
                    _inkPresenter.StrokesCollectedObservable.Signal,
                    _inkPresenter.StrokesErasedObservable.Signal,
                    // Wait for the container to actually empty on erase all.
                    inkToolbar.EraseAllClickedObservable.SelectMany(_ =>
                        Observable
                            .Interval(TimeSpan.FromMilliseconds(20))
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Where(_ => _inkPresenter.StrokeContainer.GetStrokes().Count == 0)
                            .Take(1)
                    ).Signal
                )
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(_ => Observable.FromAsync(CaptureInkAsync));

            inkFromView
                .BindTo(ViewModel, vm => vm.InkData)
                .DisposeWith(disposables);

            ViewModel
                .WhenAnyValue(vm => vm.InkData)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(data => data.HashCodeOfStrokes != _inkPresenter.StrokeContainer.GetHashCodeOfStrokes())
                .Subscribe(async data => await RestoreInkAsync(data))
                .DisposeWith(disposables);

            this
                .BindCommand(ViewModel, x => x.SaveCommand, x => x.saveButton)
                .DisposeWith(disposables);

            this
                .BindCommand(ViewModel, x => x.OpenCommand, x => x.openButton)
                .DisposeWith(disposables);

            this
                .BindInteraction(ViewModel, x => x.ShowSaveFilePicker, ShowSaveFilePickerHandler)
                .DisposeWith(disposables);

            this
                .BindInteraction(ViewModel, x => x.ShowOpenFilePicker, ShowOpenFilePickerHandler)
                .DisposeWith(disposables);
        });
    }

    private async Task ShowSaveFilePickerHandler(IInteractionContext<byte[], Unit> context)
    {
        // Create the save picker
        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };

        // Set file type choices
        savePicker.FileTypeChoices.Add("GIF File", new List<string>() { ".gif" });
        savePicker.SuggestedFileName = "Drawing";

        // Show the picker
        StorageFile? file = await savePicker.PickSaveFileAsync();

        if (file is null){
            context.SetOutput(Unit.Default);
            return;
        }

        await FileIO.WriteBytesAsync(file, context.Input);
        context.SetOutput(Unit.Default);
    }

    private async Task ShowOpenFilePickerHandler(IInteractionContext<Unit, byte[]?> context)
    {
        // Create the open picker
        var openPicker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };

        // Set file type choices
        openPicker.FileTypeFilter.Add(".gif");

        StorageFile? file = await openPicker.PickSingleFileAsync();

        if (file is null){
            context.SetOutput(null);
            return;
        }

        var buffer = await FileIO.ReadBufferAsync(file);
        context.SetOutput(buffer.ToArray());
    }

    private async Task<InkDataModel> CaptureInkAsync()
    {
        var strokeContainer = _inkPresenter.StrokeContainer;

        var data = await strokeContainer.SaveToBytesAsync(InkPersistenceFormat.GifWithEmbeddedIsf);
        var hash = strokeContainer.GetHashCodeOfStrokes();

        return new InkDataModel(data, hash);
    }

    private async Task RestoreInkAsync(InkDataModel inkDataModel)
    {
        var strokeContainer = _inkPresenter.StrokeContainer;

        strokeContainer.Clear();
        await strokeContainer.LoadFromBytesAsync(inkDataModel.GifData);
    }
}
