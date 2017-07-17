using System.Windows.Input;
using Windows.UI.Xaml;

using AvansApp.Services;
using AvansApp.Helpers;

namespace AvansApp.ViewModels.Pages
{
    public class AnnouncementPageDetailViewModel: ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }

        public ICommand StateChangedCommand { get; private set; }

        private AnnouncementVM _item;
        public AnnouncementVM Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public AnnouncementPageDetailViewModel()
        {
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            if (args.OldState.Name == "NarrowState" && args.NewState.Name == "WideState")
            {
                NavigationService.GoBack();
            }
        }
    }
}
