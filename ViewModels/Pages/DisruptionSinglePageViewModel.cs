using AvansApp.Helpers;

namespace AvansApp.ViewModels.Pages
{
    public class DisruptionSinglePageViewModel : ViewModelBase
    {
        private DisruptionItemVM _item;
        public DisruptionItemVM Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }
        public DisruptionSinglePageViewModel()
        {

        }


        /*private void GetDisruption()
        {
            HTMLview.CanDrag = true;
            HTMLview.IsRightTapEnabled = false;
            HTMLview.IsDoubleTapEnabled = true;
            HTMLview.IsTapEnabled = true;

            HTMLview.Settings.IsJavaScriptEnabled = false;
            HTMLview.Settings.IsIndexedDBEnabled = true;
            HTMLview.ManipulationMode = ManipulationModes.System;

            HTMLview.NavigateToString(Uri.UnescapeDataString(SelectedDisruption.Description));
            HTMLview.NavigationStarting += HTMLview_NavigationStarting;
        }

        private async void HTMLview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri != null)
            {
                args.Cancel = true;
                await Launcher.LaunchUriAsync(args.Uri);
            }
        }*/
    }
}
