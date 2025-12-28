using System;
using System.Reactive;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.UI.Input.Inking;

namespace ReactiveUIApp.Uwp.Extensions;

public static class InkPresenterExtensions
{
    extension(InkPresenter inkPresenter)
    {
        public IObservable<EventPattern<InkStrokesCollectedEventArgs>> StrokesCollectedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkPresenter, InkStrokesCollectedEventArgs>, InkStrokesCollectedEventArgs>
            (
                h => inkPresenter.StrokesCollected += h,
                h => inkPresenter.StrokesCollected -= h
            );

        public IObservable<EventPattern<InkStrokesErasedEventArgs>> StrokesErasedObservable =>
            Observable.FromEventPattern<TypedEventHandler<InkPresenter, InkStrokesErasedEventArgs>, InkStrokesErasedEventArgs>
            (
                h => inkPresenter.StrokesErased += h,
                h => inkPresenter.StrokesErased -= h
            );
    }
}
