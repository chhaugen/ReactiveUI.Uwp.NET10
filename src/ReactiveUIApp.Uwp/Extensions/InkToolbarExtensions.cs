using System;
using System.Reactive;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace ReactiveUIApp.Uwp.Extensions;

public static class InkToolbarExtensions
{
    extension(InkToolbar inkToolbar)
    {
        public IObservable<EventPattern<object>> ActiveToolChangedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkToolbar, object>, object>
            (
                h => inkToolbar.ActiveToolChanged += h,
                h => inkToolbar.ActiveToolChanged -= h
            );

        public IObservable<EventPattern<object>> EraseAllClickedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkToolbar, object>, object>
            (
                h => inkToolbar.EraseAllClicked += h,
                h => inkToolbar.EraseAllClicked -= h
            );

        public IObservable<EventPattern<object>> InkDrawingAttributesChangedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkToolbar, object>, object>
            (
                h => inkToolbar.InkDrawingAttributesChanged += h,
                h => inkToolbar.InkDrawingAttributesChanged -= h
            );

        public IObservable<EventPattern<object>> IsRulerButtonCheckedChangedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkToolbar, object>, object>
            (
                h => inkToolbar.IsRulerButtonCheckedChanged += h,
                h => inkToolbar.IsRulerButtonCheckedChanged -= h
            );

        public IObservable<EventPattern<InkToolbarIsStencilButtonCheckedChangedEventArgs>> IsStencilButtonCheckedChangedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkToolbar, InkToolbarIsStencilButtonCheckedChangedEventArgs>, InkToolbarIsStencilButtonCheckedChangedEventArgs>
            (
                h => inkToolbar.IsStencilButtonCheckedChanged += h,
                h => inkToolbar.IsStencilButtonCheckedChanged -= h
            );
    }
}
