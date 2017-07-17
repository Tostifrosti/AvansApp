using AvansApp.Helpers;

namespace AvansApp.ViewModels.Pages
{
    public class ClassroomAvailabilityPageDetailViewModel : ViewModelBase
    {
        private ClassroomVM _item;
        public ClassroomVM Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public ClassroomAvailabilityPageDetailViewModel()
        {

        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle(Item.Classroom);
        }
    }
}
