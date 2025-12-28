using System;
using System.Reactive;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input.Inking;

namespace ReactiveUIApp.Uwp.Extensions;

public static class InkStrokeInputExtensions
{
    extension(InkStrokeInput inkStrokeInput)
    {
        public IObservable<EventPattern<PointerEventArgs>> StrokeCanceledObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkStrokeInput, PointerEventArgs>, PointerEventArgs>
            (
                h => inkStrokeInput.StrokeCanceled += h,
                h => inkStrokeInput.StrokeCanceled -= h
            );

        public IObservable<EventPattern<PointerEventArgs>> StrokeContinuedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkStrokeInput, PointerEventArgs>, PointerEventArgs>
            (
                h => inkStrokeInput.StrokeContinued += h,
                h => inkStrokeInput.StrokeContinued -= h
            );

        public IObservable<EventPattern<PointerEventArgs>> StrokeEndedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkStrokeInput, PointerEventArgs>, PointerEventArgs>
            (
                h => inkStrokeInput.StrokeEnded += h,
                h => inkStrokeInput.StrokeEnded -= h
            );

        public IObservable<EventPattern<PointerEventArgs>> StrokeStartedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkStrokeInput, PointerEventArgs>, PointerEventArgs>
            (
                h => inkStrokeInput.StrokeStarted += h,
                h => inkStrokeInput.StrokeStarted -= h
            );

    }

}
