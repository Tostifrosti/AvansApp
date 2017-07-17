using System;
using AvansApp.Models.ServerModels;
using Windows.UI.Xaml.Controls;

namespace AvansApp.ViewModels
{
    public class AnnouncementVM
    {
        private Announcement _announcement;
        
        public AnnouncementVM(Announcement a)
        {
            _announcement = a;
        }
        public AnnouncementVM()
        {
            _announcement = new Announcement();
        }

        public string Title
        {
            get { return _announcement.title; }
            set { _announcement.title = value; }
        }
        public string Course
        {
            get { return _announcement.course; }
            set { _announcement.course = value; }
        }
        public string Link
        {
            get { return _announcement.link; }
            set { _announcement.link = value; }
        }
        public DateTime DateTime
        {
            get { return _announcement.datetime; }
            set { _announcement.datetime = value; }
        }
        public string Message
        {
            get { return _announcement.message; }
            set { _announcement.message = value; }
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
