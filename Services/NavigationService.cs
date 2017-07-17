using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace AvansApp.Services
{
    public class NavigationService
    {
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        private Frame _frame;
        public Frame Frame
        {
            get {
                if (_frame == null) {
                    _frame = Window.Current.Content as Frame;
                }
                return _frame;
            }
            set { _frame = value; }
        }
        public bool CanGoBack => Frame.CanGoBack;
        public bool CanGoForward => Frame.CanGoForward;
        public void GoBack() => Frame.GoBack();
        public void GoForward() => Frame.GoForward();
        public bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            lock(_pages)
            {
                if (!_pages.ContainsKey(pageKey)) {
                    throw new ArgumentException($"Page not found: {pageKey}. Did you forget to call NavigationService.Configure?", "pagekey");
                }
                var navigationResult = Frame.Navigate(_pages[pageKey], parameter, infoOverride);
                return navigationResult;
            }
        }
        public bool NavigateToFrame(string pageKey, UIElement frame, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            lock (_pages)
            {
                if (!_pages.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"Page not found: {pageKey}. Did you forget to call NavigationService.Configure?", "pagekey");
                }
                if (frame != null)
                {
                    Window.Current.Content = frame;
                    Window.Current.Activate();

                    Frame.BackStack.Clear();
                    Frame.NavigationFailed += (sender, e) =>
                    {
                        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
                    };
                    Frame.Navigated += OnFrameNavigated;
                    if (SystemNavigationManager.GetForCurrentView() != null)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested += OnAppViewBackButtonRequested;
                    }
                }
                var navigationResult = Frame.Navigate(_pages[pageKey], parameter, infoOverride);
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                return navigationResult;
            }
        }
        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key)) {
                    throw new ArgumentException($"The key {key} is already configured in NavigationService");
                }

                if (_pages.Any(p => p.Value == pageType)) {
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");
                }

                _pages.Add(key, pageType);
            }
        }

        public string GetNameOfRegisteredPage(Type page)
        {
            lock (_pages)
            {
                if (_pages.ContainsValue(page)) {
                    return _pages.FirstOrDefault(p => p.Value == page).Key;
                } else {
                    throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
                }
            }
        }
        public void RemoveHistory()
        {
            Frame.BackStack.Clear();
            //Frame.CacheSize = 1;
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = (CanGoBack) ?
                AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private void OnAppViewBackButtonRequested(object sender, BackRequestedEventArgs e)
        {
            if (CanGoBack)
            {
                GoBack();
                e.Handled = true;
            }
        }
    }
}
