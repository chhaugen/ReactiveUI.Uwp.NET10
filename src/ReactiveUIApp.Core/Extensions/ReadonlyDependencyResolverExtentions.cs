using ReactiveUIApp.Core.Exceptions;
using Splat;

namespace ReactiveUIApp.Core.Extensions;

public static class ReadonlyDependencyResolverExtentions
{
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver resolver, string? contract = null)
        => resolver.GetService<T>(contract)
        ?? throw new ServiceMissingException<T>();

    public static T SetValueOrGetRequiredService<T>(this IReadonlyDependencyResolver resolver, T? value, string? contract = null)
        => value ?? resolver.GetRequiredService<T>(contract);
}
