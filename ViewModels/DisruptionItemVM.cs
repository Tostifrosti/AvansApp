using System;
using Windows.UI.Xaml.Controls;

using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class DisruptionItemVM
    {
        private DisruptionItem _disruptionItem;

        public DisruptionItemVM(DisruptionItem item)
        {
            _disruptionItem = item;
        }
        public DisruptionItemVM()
        {
            _disruptionItem = new DisruptionItem();
        }

        public string Title
        {
            get { return _disruptionItem.Title; }
            set { _disruptionItem.Title = value; }
        }
        public string Link
        {
            get { return _disruptionItem.Link; }
            set { _disruptionItem.Link = value; }
        }
        public string GuId
        {
            get { return _disruptionItem.GuId; }
            set { _disruptionItem.GuId = value; }
        }
        public DateTime PublicationDate
        {
            get { return _disruptionItem.PublicationDate; }
            set { _disruptionItem.PublicationDate = value; }
        }
        public int Type
        {
            get { return _disruptionItem.Type; }
            set { _disruptionItem.Type = value; }
        }
        public string Description
        {
            get { return _disruptionItem.Description; }
            set { _disruptionItem.Description = value; }
        }
        public string Icon
        {
            get
            {
                switch (Type)
                {
                    case 0: // Error
                        return "\uEA39"; // &#xE814; of &#xEA39;
                    case 1: // Maintenance
                        return "\uE90F";  // &#xE90F;
                    case 2: // Ok?
                        return "\uE930";  // &#xE8FB; of &#xE930;
                    default: // Default
                        return "\uE946";  // &#xE946;
                }
            }
        }

        public async void HTMLview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri != null)
            {
                args.Cancel = true;
                await Windows.System.Launcher.LaunchUriAsync(args.Uri);
            }
        }
    }
}
