using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Parkrun.MVVM.Models;
using PropertyChanged;
using Parkrun.Services;
using Parkrun.MVVM.Views;

namespace Parkrun.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ParkrunViewModel
    {
        #region Properties and Fields
        DatabaseService databaseService = new();

        public ObservableCollection<ParkrunData> Data { get; set; } = new();
        private List<ParkrunData> pendingEntries = new();

        public DateTime SelectedDate { get; set; } = DateTime.Now.Date;

        public List<int> Hours { get; } = Enumerable.Range(0, 24).ToList();
        public List<int> Minutes { get; } = Enumerable.Range(0, 60).ToList();
        public List<int> Seconds { get; } = Enumerable.Range(0, 60).ToList();

        public int SelectedHour { get; set; }
        public int SelectedMinute { get; set; }
        public int SelectedSecond { get; set; }

        public TimeSpan SelectedTime => new TimeSpan(SelectedHour, SelectedMinute, SelectedSecond);

        public ICommand AddDataCommand { get; }
        public ICommand RemoveDataCommand { get; }


        private bool isDataLoaded = false;
        #endregion

        public Command OpenChartPageCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync("///ChartPage");
        });


        public async Task LoadDataAsync()
        {
            isDataLoaded = false;
            Data.Clear();

            var data = await databaseService.GetDataAsync();
            foreach (var item in data)
            {
                Data.Add(item);
            }

            isDataLoaded = true;

            // Jetzt füge die zwischengespeicherten neuen Einträge hinzu
            foreach (var pending in pendingEntries)
            {
                Data.Add(pending);
            }
            pendingEntries.Clear(); // Warteschlange leeren
        }

        public ParkrunViewModel()
        {
            AddDataCommand = new Command( async () =>
            {
                Debug.WriteLine($"Neuer Eintrag: {SelectedDate} - {SelectedTime}");

                var parkrunData = new ParkrunData { Date = SelectedDate, Time = SelectedTime };

                await databaseService.SaveDataAsync(parkrunData);

                if (isDataLoaded)
                {
                    Data.Add(parkrunData);
                }
                else
                {
                    pendingEntries.Add(parkrunData);        // Speichere das neue Element temporär, so dass es nach dem Laden der Daten von der Datenbank in der "LoadDataAsync" hinzugefügt wird.
                }
            });
            RemoveDataCommand = new Command<ParkrunData>(async (parkrunData) =>
            {
                if (parkrunData != null)
                {
                    await databaseService.DeleteDataAsync(parkrunData);
                    Data.Remove(parkrunData);
                }
            });

            // Laden der Daten aus der Datenbank im seperaten Task.
            Task.Run(async () =>
            {
                try
                {
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Fehler beim Laden der Daten: {ex.Message}");
                }
            });
        }

        
    }
}
