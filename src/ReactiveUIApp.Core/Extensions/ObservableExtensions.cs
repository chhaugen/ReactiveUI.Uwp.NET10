using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUIApp.Core.Extensions;

public static class ObservableExtensions
{
    extension<T>(IObservable<T> observable)
    {
        public IObservable<Unit> Signal => observable.Select(_ => Unit.Default);
    }
}
