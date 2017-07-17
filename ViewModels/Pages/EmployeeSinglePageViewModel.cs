using AvansApp.Helpers;
using Windows.UI.Xaml.Media;

namespace AvansApp.ViewModels.Pages
{
    public class EmployeeSinglePageViewModel : ViewModelBase
    {
        private EmployeeVM _item;
        public EmployeeVM Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }
        public ImageSource ProfileImage { get; set; }

        public EmployeeSinglePageViewModel()
        {
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle(Item.FullName);
        }

        public async void SetEmployeeImage()
        {
            try
            {
                Item.ProfilePicture.bitmap = await Item.ProfilePicture.Base64StringToBitmap(Item.ProfilePicture.image);
                ProfileImage = Item.ProfilePicture.bitmap;
            }
            catch (System.Exception Error)
            {
                System.Diagnostics.Debug.WriteLine(Error.Message);
            }
        }
    }
}
