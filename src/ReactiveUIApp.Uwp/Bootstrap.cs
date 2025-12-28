using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUIApp.Core.ViewModels;
using ReactiveUIApp.Uwp.Views;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ReactiveUIApp.Uwp;

internal static class Bootstrap
{
    private static IServiceProvider? _serviceProvider;

    public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Bootstrap not initialized");

    [RequiresUnreferencedCode("Calls ReactiveUI.DependencyResolverMixins.InitializeReactiveUI(params RegistrationNamespace[])")]
    [RequiresDynamicCode("Calls ReactiveUI.DependencyResolverMixins.InitializeReactiveUI(params RegistrationNamespace[])")]
    public static void Initialize()
    {
        if (_serviceProvider is not null)
            return;

        var services = new ServiceCollection();
        services.UseMicrosoftDependencyResolver();

        // ---------- IWantsToRegisterStuff ----------
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();

        // ---------- App services ----------

        // ---------- View and ViewModels ----------
        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<IScreen>(sp => sp.GetRequiredService<ShellViewModel>());

        services.AddTransient<SmileViewModel>();
        services.AddTransient<IViewFor<SmileViewModel>, SmileView>();

        services.AddTransient<InkCanvasViewModel>();
        services.AddTransient<IViewFor<InkCanvasViewModel>, InkCanvasView>();

        // ---------- Build container ----------
        _serviceProvider = services.BuildServiceProvider();

        // ---------- Bridge MS.DI -> Splat ----------
        _serviceProvider.UseMicrosoftDependencyResolver();

        // Optional global exception handling
        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
        {
            System.Diagnostics.Debug.WriteLine(ex);
        });
    }
}
