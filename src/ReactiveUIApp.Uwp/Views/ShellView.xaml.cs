using Microsoft.UI.Xaml.Controls;
using ReactiveUI;
using ReactiveUIApp.Core.Extensions;
using ReactiveUIApp.Core.ViewModels;
using ReactiveUIApp.Uwp.Extensions;
using Splat;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ReactiveUIApp.Uwp.Views;

public abstract class ReactiveShellView : ReactivePage<ShellViewModel>;

/// <summary>
/// An empty page that can be used on its own or navigated to within a <see cref="Frame">.
/// </summary>
public sealed partial class ShellView : ReactiveShellView
{
    public ShellView() : this(null) { }

    [RequiresUnreferencedCode("WhenActivated may reference types that could be trimmed")]
    [RequiresDynamicCode("WhenActivated uses reflection which requires dynamic code generation")]
    public ShellView(ShellViewModel? viewModel = null)
    {
        ViewModel = Locator.Current.SetValueOrGetRequiredService(viewModel);

        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            this
                .OneWayBind(ViewModel, x => x.Router, x => x.routedViewHost.Router)
                .DisposeWith(disposables);


            navigationView
                .ItemInvokedObservable
                .Select(x => x.EventArgs.InvokedItemContainer)
                .StartWith((NavigationViewItemBase)navigationView.SelectedItem)
                .Select<NavigationViewItemBase, IRoutableViewModel?>(item => item switch
                {
                    _ when item == smileNavigationViewItem => ViewModel.SmileViewModel,
                    _ when item == inkCanvasNavigationViewItem => ViewModel.InkCanvasViewModel,
                    _ => null
                })
                .WhereNotNull()
                .InvokeCommand(ViewModel.Router.Navigate)
                .DisposeWith(disposables);

            navigationView
                .BackRequestedObservable
                .Where(_ => ViewModel.Router.NavigationStack.Count > 1)
                .Signal
                .InvokeCommand(ViewModel.Router.NavigateBack)
                .DisposeWith(disposables);

            ViewModel
                .Router
                .NavigationChanged
                .Select(_ => ViewModel.Router.NavigationStack.Count > 1)
                .BindTo(this, x => x.navigationView.IsBackEnabled)
                .DisposeWith(disposables);

            ViewModel
                .Router
                .CurrentViewModel
                .Subscribe(vm =>
                {
                    navigationView.SelectedItem = vm switch
                    {
                        SmileViewModel => smileNavigationViewItem,
                        InkCanvasViewModel => inkCanvasNavigationViewItem,
                        _ => null
                    };
                })
                .DisposeWith(disposables);
        });
        navigationView.SelectedItem = smileNavigationViewItem;
    }
}
