using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Services.Pages;
using System;
using System.Collections.Generic;

namespace AvansApp.ViewModels.Pages
{
    public class ExamPageViewModel : ViewModelBase
    {
        public ObservableCollection<ExamVM> Items { get; private set; } = new ObservableCollection<ExamVM>();

        private ExamVM _selected;
        public ExamVM Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }
        private bool _isLoading;
        public bool IsLoading {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }
        private bool _hasNoResult;
        public bool HasNoResult {
            get { return _hasNoResult; }
            set { Set(ref _hasNoResult, value); }
        }
        private bool _examNotification;
        public bool ExamNotification {
            get { return _examNotification; }
            set { Set(ref _examNotification, value); }
        }
        public ICommand ItemClickCommand { get; private set; }
        public ICommand RefreshItemsCommand { get; private set; }
        private ExamService Service { get; set; }
        private DateTime refreshTime;

        public ExamPageViewModel()
        {
            Service = Singleton<ExamService>.Instance;
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            RefreshItemsCommand = new RelayCommand(OnRefreshItems);
            refreshTime = DateTime.Now;
            ExamNotification = false;
        }

        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_ExamsPage".GetLocalized());
        }
        public async Task LoadDataAsync()
        {
            if (Items.Count <= 0 || refreshTime <= DateTime.Now.AddMinutes(-5))
            {
                refreshTime = DateTime.Now;
                IsLoading = true;
                HasNoResult = false;
                Items.Clear();
                
                var data = await Service.GetExams();
                data = data.OrderByDescending(d => d.DateTime).ToList();
                ExamNotification = Service.ExamSubsriptionToken;

                foreach (var item in data)
                {
                    Items.Add(item);
                }
                
                Selected = Items.FirstOrDefault();
                IsLoading = false;
                HasNoResult = Items.Count <= 0;
            }
        }

        private void OnItemClick(ItemClickEventArgs e)
        {
            // TODO?
        }

        private async void OnRefreshItems()
        {
            // Pull-to-refresh will be available every 30 secs
            if (refreshTime <= DateTime.Now.AddSeconds(-30))
            {
                refreshTime = DateTime.Now;

                var data = await Service.GetExams();
                data = new List<ExamVM>(data.OrderByDescending(d => d.DateTime));

                var list = new List<ExamVM>();
                
                for (int i = 0; i < data.Count; i++)
                {
                    bool isFound = false;
                    for (int j = 0; j < Items.Count; j++)
                    {
                        if (Service.Compare(data[i], Items[j]) == true)
                        {
                            isFound = true;
                        }
                    }
                    if (!isFound)
                        list.Add(data[i]);
                }

                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        Items.Insert(0, item);
                    }
                    Selected = Items.FirstOrDefault();
                }
                HasNoResult = Items.Count <= 0;
            }
        }
    }
}
