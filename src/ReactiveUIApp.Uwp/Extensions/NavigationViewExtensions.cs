using Microsoft.UI.Xaml.Controls;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Windows.Foundation;

namespace ReactiveUIApp.Uwp.Extensions;

public static class NavigationViewExtensions
{
    extension(NavigationView navigationView)
    {
        public IObservable<EventPattern<NavigationViewItemInvokedEventArgs>> ItemInvokedObservable =>
            Observable.FromEventPattern<TypedEventHandler<NavigationView, NavigationViewItemInvokedEventArgs>, NavigationViewItemInvokedEventArgs>
            (
                h => navigationView.ItemInvoked += h,
                h => navigationView.ItemInvoked -= h
            );

        public IObservable<EventPattern<NavigationViewBackRequestedEventArgs>> BackRequestedObservable =>
            Observable.FromEventPattern<TypedEventHandler<NavigationView, NavigationViewBackRequestedEventArgs>, NavigationViewBackRequestedEventArgs>
            (
                h => navigationView.BackRequested += h,
                h => navigationView.BackRequested -= h
            );
    }
}
