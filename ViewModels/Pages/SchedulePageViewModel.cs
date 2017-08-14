using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
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
        public bool ScheduleEnabled { get { return !_isLoading && !_hasNoResult && !_hasNoScheduleCode; } }
        public ICommand OnPreviousDayClickCommand { get; private set; }
        public ICommand OnNextDayClickCommand { get; private set; }
        public ICommand OnTodayClickCommand { get; private set; }

        private ScheduleService Service { get; set; }
        private SettingsService Settings { get; set; }

        public SchedulePageViewModel()
        {
            Items = new List<ObservableCollection<ScheduleVM>>();
            CurrentDay = new ObservableCollection<ScheduleVM>();
            Service = Singleton<ScheduleService>.Instance;
            Settings = Singleton<SettingsService>.Instance;

            todayIndex = 0;
            currentDayIndex = 0;

            OnPreviousDayClickCommand = new RelayCommand<ItemClickEventArgs>(OnPreviousDayClick, (e) => { return true; });
            OnNextDayClickCommand = new RelayCommand<ItemClickEventArgs>(OnNextDayClick, (e) => { return true; });
            OnTodayClickCommand = new RelayCommand<ItemClickEventArgs>(OnTodayClick, (e) => { return true; });
            
            SetHeader();
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_SchedulePage".GetLocalized());
        }
        public async Task LoadDataAsync()
        {
            if (Settings.KeyExists(SettingsService.ScheduleCodeKey))
            {
                IsLoading = true;
                HasNoResult = false;
                HasNoScheduleCode = false;

                string scheduleCode = await Settings.ReadScheduleCode();
                bool withoutBlanks = await Settings.ReadScheduleBlanks();
                List<List<ScheduleVM>> data = await Service.GetSchedule(ScheduleType.Group, scheduleCode, DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(3), withoutBlanks);

                todayIndex = (withoutBlanks == true) ? Service.TodayIndex : 0;
                currentDayIndex = todayIndex;

                Items.Clear();
                CurrentDay.Clear();

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

                    foreach (ScheduleVM s in Items[todayIndex]) {
                        if (s != null && s.Id != -1)
                            CurrentDay.Add(s);
                    }
                }
                IsLoading = false;
                HasNoResult = Items.Count <= 0;
                HasNoScheduleCode = false;

                SetCurrentDay();
                SetHeader();
            }
            else
            {
                Items.Clear();
                CurrentDay.Clear();
                todayIndex = 0;
                currentDayIndex = 0;

                IsLoading = false;
                HasNoResult = false;
                HasNoScheduleCode = true;
            }
            OnPropertyChanged("ScheduleEnabled");
        }

        private void OnPreviousDayClick(ItemClickEventArgs e)
        {
            if (currentDayIndex > 0)
            {
                currentDayIndex -= 1;
                CurrentDay.Clear();
                foreach (ScheduleVM s in Items[currentDayIndex]) {
                    if (s != null && s.Id != -1)
                        CurrentDay.Add(s);
                }
                SetCurrentDay();
                SetHeader();
            }

        }
        private void OnNextDayClick(ItemClickEventArgs e)
        {
            if (currentDayIndex < (Items.Count - 1))
            {
                currentDayIndex += 1;
                CurrentDay.Clear();
                foreach (ScheduleVM s in Items[currentDayIndex]) {
                    if (s != null && s.Id != -1)
                        CurrentDay.Add(s);
                }

                SetCurrentDay();
                SetHeader();
            }
        }
        private void OnTodayClick(ItemClickEventArgs e)
        {
            if (Items.Count > 0)
            {
                CurrentDay.Clear();
                foreach (ScheduleVM s in Items[todayIndex]) {
                    if (s != null && s.Id != -1)
                        CurrentDay.Add(s);
                }
                currentDayIndex = todayIndex;
                SetCurrentDay();
                SetHeader();
            }
        }
        private void SetCurrentDay()
        {
            // TODO
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
            OnPropertyChanged("CurrentDay");
        }

        private void SetHeader()
        {
            if (Items.Count > 0 && currentDayIndex < Items.Count && Items[currentDayIndex].Count > 0)
            {
                Header = Items[currentDayIndex][0].Datum.Day + "-" + Items[currentDayIndex][0].Datum.Month + "-" + Items[currentDayIndex][0].Datum.Year; // Pak de datum van het eerste item
                HeaderDay = Items[currentDayIndex][0].Datum.DayOfWeek.ToString();
            }
            else
            {
                Header = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                HeaderDay = DateTime.Now.DayOfWeek.ToString();
            }
        }
    }
}
