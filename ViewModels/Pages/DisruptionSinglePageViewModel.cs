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
    }
}
