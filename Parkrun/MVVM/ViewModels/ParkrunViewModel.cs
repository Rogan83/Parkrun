using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microcharts;
using SkiaSharp; // Wird für die Grafiken benötigt

using Parkrun.MVVM.Models;
using PropertyChanged;
using Parkrun.Services;

namespace Parkrun.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ParkrunViewModel
    {
        #region Properties and Fields
        DatabaseService databaseService = new();

        public ObservableCollection<ParkrunData> Results { get; set; } = new();
        private List<ParkrunData> pendingEntries = new();
        public Chart LineChart { get; private set; }

        public DateTime SelectedDate { get; set; } = DateTime.Now;

        public List<int> Hours { get; } = Enumerable.Range(0, 24).ToList();
        public List<int> Minutes { get; } = Enumerable.Range(0, 60).ToList();
        public List<int> Seconds { get; } = Enumerable.Range(0, 60).ToList();

        public int SelectedHour { get; set; }
        public int SelectedMinute { get; set; }
        public int SelectedSecond { get; set; }

        public TimeSpan SelectedTime => new TimeSpan(SelectedHour, SelectedMinute, SelectedSecond);


        public ICommand AddResultCommand { get; }
        public ICommand RemoveResultCommand { get; }


        private bool isDataLoaded = false;
        #endregion

        public async Task LoadDataAsync()
        {
            isDataLoaded = false;
            Results.Clear();

            var data = await databaseService.GetDataAsync();
            foreach (var item in data)
            {
                Results.Add(item);
            }

            isDataLoaded = true;

            // Jetzt füge die zwischengespeicherten neuen Einträge hinzu
            foreach (var pending in pendingEntries)
            {
                Results.Add(pending);
            }
            pendingEntries.Clear(); // Warteschlange leeren

            UpdateChart();
        }

        public ParkrunViewModel()
        {
            AddResultCommand = new Command( async () =>
            {
                Debug.WriteLine($"Neuer Eintrag: {SelectedDate} - {SelectedTime}");

                var parkrunData = new ParkrunData { Date = SelectedDate, Time = SelectedTime };

                await databaseService.SaveDataAsync(parkrunData);

                if (isDataLoaded)
                {
                    Results.Add(parkrunData);
                    UpdateChart();
                }
                else
                {
                    pendingEntries.Add(parkrunData);        // Speichere das neue Element temporär, so dass es nach dem Laden der Daten von der Datenbank in der "LoadDataAsync" hinzugefügt wird.
                }

            });
            RemoveResultCommand = new Command<ParkrunData>(async (parkrunData) =>
            {
                if (parkrunData != null)
                {
                    await databaseService.DeleteDataAsync(parkrunData);
                    Results.Remove(parkrunData);
                }
                UpdateChart();
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
        /// <summary>
        /// Aktualisiert das Diagramm.
        /// </summary>
        private void UpdateChart()
        {
            List<ChartEntry> entries;
            if (Results.Count == 0)
            {
                entries = new List<ChartEntry>
                {
                    new ChartEntry(0) { Label = "", ValueLabel = "", Color = SKColor.Parse("#FF5733") },
                };
            }
            else
            {
                var maxTime = Results.Max(r => r.Time.TotalSeconds); // Höchster Wert bestimmen
                entries = Results.Select((result, index) =>
                    new ChartEntry((float)(maxTime - result.Time.TotalSeconds))
                    {
                        Label = result.Date.ToShortDateString(),
                        ValueLabel = $"{result.Time}",
                        Color = SKColor.Parse("#3498db") // Blaue Farbe für die Linie
                    }).ToList();
            }
            //Testdaten
            ///
            //var entries = new List<ChartEntry>
            //{

            //    new ChartEntry(100) { Label = "Mo", ValueLabel = "100", Color = SKColor.Parse("#FF5733") },
            //    new ChartEntry(500) { Label = "Do", ValueLabel = "500", Color = SKColor.Parse("#FFD700") },
            //    new ChartEntry(1000) { Label = "Fr", ValueLabel = "1000", Color = SKColor.Parse("#A020F0") },
            //    new ChartEntry(500) { Label = "Do", ValueLabel = "500", Color = SKColor.Parse("#FFD700") },
            //};

            LineChart = new LineChart 
            { 
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,

                //MaxValue = 1000,  // Höchster Wert ein wenig über deinem höchsten Punkt setzen
                //MinValue = 0   // Niedrigster Wert nahe deinem kleinsten Punkt setzen
            };
        }
    }
}
