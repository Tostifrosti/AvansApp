using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Models;
using AvansApp.Models.Enums;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class SchedulePageViewModel : ViewModelBase
    {
        private int currentDayIndex;
        private int todayIndex;

        public List<ObservableCollection<ScheduleVM>> Items { get; private set; }
        public ObservableCollection<ScheduleVM> CurrentDay { get; private set; }
        private string _header;
        private string _headerDay;
        public string Header {
            get { return _header; }
            set { Set(ref _header, value); }
        }
        public string HeaderDay {
            get { return _headerDay; }
            set { Set(ref _headerDay, value); }
        }
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
        private bool _hasNoScheduleCode;
        public bool HasNoScheduleCode
        {
            get { return _hasNoScheduleCode; }
            set { Set(ref _hasNoScheduleCode, value); }
        }
        public bool ButtonsEnabled { get { return !_isLoading && !_hasNoResult && !_hasNoScheduleCode; } }
        public ICommand OnPreviousDayClickCommand { get; private set; }
        public ICommand OnNextDayClickCommand { get; private set; }
        public ICommand OnTodayClickCommand { get; private set; }

        private ScheduleService Service { get; set; }

        public SchedulePageViewModel()
        {
            Items = new List<ObservableCollection<ScheduleVM>>();
            CurrentDay = new ObservableCollection<ScheduleVM>();
            Service = new ScheduleService();

            todayIndex = 0;
            currentDayIndex = 0;

            OnPreviousDayClickCommand = new RelayCommand<ItemClickEventArgs>(OnPreviousDayClick, (e) => { return true; });
            OnNextDayClickCommand = new RelayCommand<ItemClickEventArgs>(OnNextDayClick, (e) => { return true; });
            OnTodayClickCommand = new RelayCommand<ItemClickEventArgs>(OnTodayClick, (e) => { return true; });
            //PreviousDayButton.IsEnabled = false;
            //NextDayButton.IsEnabled = false;
            //TodayButton.IsEnabled = false;

            // Remove old key (could clear a lot of storage)
            OAuth.GetInstance().Client.RemoveFromVault("ScheduleStorage");
            
            SetHeader();
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_SchedulePage".GetLocalized());
        }
        public async Task LoadDataAsync()
        {
            if (OAuth.GetInstance().Client.CheckTokenExists("ScheduleCode"))
            {
                IsLoading = true;
                HasNoResult = false;
                HasNoScheduleCode = false;

                string scheduleCode = OAuth.GetInstance().Client.GetTokenFromVault("ScheduleCode");
                bool withoutBlanks = OAuth.GetInstance().Client.CheckTokenExists("ScheduleWithoutBlanks");
                List<List<ScheduleVM>> data = await Service.GetSchedule(ScheduleType.Classroom, scheduleCode, DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(3), withoutBlanks);

                todayIndex = (withoutBlanks == true) ? Service.TodayIndex : 0;
                currentDayIndex = todayIndex;

                Items.Clear();

                if (data.Count > 0)
                {
                    foreach (List<ScheduleVM> list in data)
                    {
                        var l = new ObservableCollection<ScheduleVM>();
                        foreach (ScheduleVM s in list)
                        {
                            l.Add(s);
                        }
                        Items.Add(l);
                    }
                    CurrentDay = Items[todayIndex];
                }
                IsLoading = false;
                HasNoResult = Items.Count <= 0;
                HasNoScheduleCode = false;
            }
            else
            {
                Items.Clear();
                todayIndex = 0;
                currentDayIndex = 0;

                IsLoading = false;
                HasNoResult = false;
                HasNoScheduleCode = true;
            }
            OnPropertyChanged("ButtonsEnabled");
        }

        private void OnPreviousDayClick(ItemClickEventArgs e)
        {
            if (currentDayIndex > 0)
            {
                currentDayIndex -= 1;
                CurrentDay = Items[currentDayIndex];

                SetCurrentDay();
                SetHeader();
            }

        }
        private void OnNextDayClick(ItemClickEventArgs e)
        {
            if (currentDayIndex < (Items.Count - 1))
            {
                currentDayIndex += 1;
                CurrentDay = Items[currentDayIndex];

                SetCurrentDay();
                SetHeader();
            }
        }
        private void OnTodayClick(ItemClickEventArgs e)
        {
            if (CurrentDay != null && CurrentDay.Count > 0) //&& todayIndex != null
            {
                CurrentDay = Items[todayIndex];
                currentDayIndex = todayIndex;
                SetCurrentDay();
                SetHeader();
            }
        }
        private void SetCurrentDay()
        {
            if (CurrentDay.Count <= 0 || (CurrentDay.Count == 1 && CurrentDay[0].Id < 0))
            {
                //ScheduleListView.ItemsSource = new List<ScheduleVM>();
                //ScheduleListView.Visibility = Visibility.Collapsed;
                //NoScheduleText.Visibility = Visibility.Visible;
            }
            else
            {
                //ScheduleListView.ItemsSource = CurrentDay;
                //ScheduleListView.Visibility = Visibility.Visible;
                //NoScheduleText.Visibility = Visibility.Collapsed;
            }
        }

        private void SetHeader()
        {
            if (CurrentDay != null && CurrentDay.Count > 0)
            {
                Header = CurrentDay[0].Datum.Day + "-" + CurrentDay[0].Datum.Month + "-" + CurrentDay[0].Datum.Year; // Pak de datum van de eerste item
                HeaderDay = CurrentDay[0].Datum.DayOfWeek.ToString();
            }
            else
            {
                Header = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                HeaderDay = DateTime.Now.DayOfWeek.ToString();
            }
        }

        /*protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
            //FirstTimeText.Visibility = Visibility.Collapsed;
            if (e != null && e.Parameter != null)
            {
                //main = (MainPage)e.Parameter;
                //main.PageTitle = "Rooster";

                if(OAuth.GetInstance().Client.CheckTokenExists("ScheduleCode"))
                {
                    string scheduleCode = OAuth.GetInstance().Client.GetTokenFromVault("ScheduleCode");
                    DateTime date = DateTime.Now.AddMonths(-3); // If you like to look back. 
                    string previousDate = date.Day + "-" + date.Month + "-" + date.Year;
                    date = date.AddMonths(6);
                    string nextDate = date.Day + "-" + date.Month + "-" + date.Year;
                    string type = GetScheduleType(ScheduleType.Group);
                    if(OAuth.GetInstance().Client.CheckTokenExists("ScheduleWithoutBlanks"))
                        GetScheduleWithoutBlanks("?type=" + type + "&param=" + scheduleCode + "&start=" + previousDate + "&end=" + nextDate);
                    else
                        GetScheduleWithBlanks("?type=" + type + "&param=" + scheduleCode + "&start=" + previousDate + "&end=" + nextDate);
                }
                else
                {
                    todayIndex = 0;
                    currentDayIndex = 0;
                    PreviousDayButton.IsEnabled = false;
                    NextDayButton.IsEnabled = false;
                    TodayButton.IsEnabled = false;
                    SetHeader();
                    //ScheduleListView.Visibility = Visibility.Collapsed;
                    //NoScheduleText.Visibility = Visibility.Collapsed;
                    //FirstTimeText.Visibility = Visibility.Visible;
                }
            }
        }*/
    }
}
