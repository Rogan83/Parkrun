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

        public ObservableCollection<ParkrunData> Data { get; set; } = new();
        private List<ParkrunData> pendingEntries = new();
        public Chart LineChart { get; private set; }

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

            UpdateChart();
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
                    UpdateChart();
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
            if (Data.Count == 0)
            {
                entries = new List<ChartEntry>
                {
                    new ChartEntry(0) { Label = "", ValueLabel = "", Color = SKColor.Parse("#FF5733") },
                };
            }
            else
            {
                // Liste nach Datum sortieren
                Data = new ObservableCollection<ParkrunData>(Data.OrderBy(d => d.Date));

                var maxTime = Data.Max(d => d.Time.TotalSeconds); // Höchster Wert bestimmen
                var minTime = Data.Min(d => d.Time.TotalSeconds); // Niedrigsten Wert bestimmen
                entries = Data.Select((result, index) =>
                {
                    float calculatedValue = (float)(maxTime - result.Time.TotalSeconds);

                    // Falls die Zeit die Bestzeit erreicht hat, dann wird diese anders eingefärbt
                    SKColor color;
                    if (result.Time.TotalSeconds == minTime)
                    {
                        color = SKColor.Parse("#2ecc71"); // Grün für die Bestzeit
                    }
                    else if (result.Time.TotalSeconds == maxTime)
                    {
                        color = SKColor.Parse("#e74c3c"); // Rot für die langsamste Zeit
                    }
                    else
                    {
                        color = SKColor.Parse("#f1c40f"); // Gelb für normale Zeiten
                    }

                    return new ChartEntry(calculatedValue)
                    {
                        Label = result.Date.ToShortDateString(),
                        ValueLabel = $"{result.Time}",
                        Color = color // Dynamische Farbänderung basierend auf der Bedingung
                    };
                }).ToList();
            }

            #region TestData
            ///
            //var entries = new List<ChartEntry>
            //{

            //    new ChartEntry(100) { Label = "Mo", ValueLabel = "100", Color = SKColor.Parse("#FF5733") },
            //    new ChartEntry(500) { Label = "Do", ValueLabel = "500", Color = SKColor.Parse("#FFD700") },
            //    new ChartEntry(1000) { Label = "Fr", ValueLabel = "1000", Color = SKColor.Parse("#A020F0") },
            //    new ChartEntry(500) { Label = "Do", ValueLabel = "500", Color = SKColor.Parse("#FFD700") },
            //};
            #endregion

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
