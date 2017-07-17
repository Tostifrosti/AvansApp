using System.Windows.Input;
using Windows.UI.Xaml;

using AvansApp.Helpers;
using AvansApp.Services;

namespace AvansApp.ViewModels.Pages
{
    public class ResultPageDetailViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        private ResultVM _item;
        public ResultVM Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }
        public ICommand StateChangedCommand { get; private set; }


        public ResultPageDetailViewModel()
        {
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle(Item.CursusNaam);
        }

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            if (args.OldState.Name == NarrowStateName && args.NewState.Name == WideStateName)
            {
                NavigationService.GoBack();
            }
        }
    }
}
