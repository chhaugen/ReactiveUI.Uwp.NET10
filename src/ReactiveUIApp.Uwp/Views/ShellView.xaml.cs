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
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

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

        // Hide default title bar.
        CoreApplicationViewTitleBar coreTitleBar =
            CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.ExtendViewIntoTitleBar = true;

        // Set caption buttons background to transparent.
        ApplicationViewTitleBar titleBar =
            ApplicationView.GetForCurrentView().TitleBar;
        titleBar.ButtonBackgroundColor = Colors.Transparent;

        // Set XAML element as a drag region.
        Window.Current.SetTitleBar(AppTitleBar);

        // Register a handler for when the size of the overlaid caption control changes.
        coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

        // Register a handler for when the title bar visibility changes.
        // For example, when the title bar is invoked in full screen mode.
        coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

        // Register a handler for when the window activation changes.
        //Window.Current.CoreWindow.Activated += CoreWindow_Activated;

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

    private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
        // Get the size of the caption controls and set padding.
        LeftPaddingColumn.Width = new GridLength(sender.SystemOverlayLeftInset);
        RightPaddingColumn.Width = new GridLength(sender.SystemOverlayRightInset);
    }

    private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
    {
        if (sender.IsVisible)
        {
            AppTitleBar.Visibility = Visibility.Visible;
        }
        else
        {
            AppTitleBar.Visibility = Visibility.Collapsed;
        }
    }

    private void CoreWindow_Activated(CoreWindow sender, WindowActivatedEventArgs args)
    {
        UISettings settings = new UISettings();
        if (args.WindowActivationState == CoreWindowActivationState.Deactivated)
        {
            AppTitleTextBlock.Foreground =
               new SolidColorBrush(settings.UIElementColor(UIElementType.GrayText));
        }
        else
        {
            AppTitleTextBlock.Foreground =
               new SolidColorBrush(settings.UIElementColor(UIElementType.WindowText));
        }
    }
}
