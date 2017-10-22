using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AvansApp.Helpers;
using AvansApp.Services;
using System.Collections.ObjectModel;
using AvansApp.Services.Pages;
using AvansApp.Models.Enums;

namespace AvansApp.ViewModels.Pages
{
    public class EmployeeScheduleSinglePageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        private EmployeeVM _item;
        public EmployeeVM Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public ObservableCollection<ScheduleVM> Items { get; private set; }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }
        private bool _hasNoResult;
        public bool HasNoResult
        {
            get { return _hasNoResult; }
            set { Set(ref _hasNoResult, value); }
        }
        public bool ScheduleEnabled { get { return !_isLoading && !_hasNoResult; } }

        private ScheduleService Service { get; set; }

        public EmployeeScheduleSinglePageViewModel()
        {
            Items = new ObservableCollection<ScheduleVM>();
            Service = Singleton<ScheduleService>.Instance;

            IsLoading = false;
            HasNoResult = false;
        }

        public void Initialize()
        {
            MainPageViewModel.SetPageTitle(Item.FullName);
        }

        public async Task LoadDataAsync()
        {
            if (Item == null)
                return;
            
            IsLoading = true;
            HasNoResult = false;

            List<ScheduleVM> data = await Service.GetScheduleToday(ScheduleType.Teacher, Item.Login);

            Items.Clear();

            if (data.Count > 0)
            {
                foreach (ScheduleVM s in data)
                {
                    if (s != null && s.Id != -1)
                        Items.Add(s);
                }

                IsLoading = false;
                HasNoResult = Items.Count <= 0;
            }
            else
            {
                Items.Clear();

                IsLoading = false;
                HasNoResult = true;
            }
            OnPropertyChanged("ScheduleEnabled");
        }
    }
}
