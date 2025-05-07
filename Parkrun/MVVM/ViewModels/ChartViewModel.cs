using Parkrun.MVVM.Models;
using PropertyChanged;
using System;
using Microcharts;
using SkiaSharp;        // Wird für die Grafiken benötigt
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkrun.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class ChartViewModel
    {
        internal List<ParkrunData> Data { get; set; } = new List<ParkrunData>();
        public Chart LineChart { get; private set; }

        public ChartViewModel()
        {
            LineChart = new LineChart();
        }

        /// <summary>
        /// Aktualisiert das Diagramm.
        /// </summary>
        internal void UpdateChart()
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
                //Data = new ObservableCollection<ParkrunData>(Data.OrderBy(d => d.Date));
                Data = Data.OrderBy(d => d.Date).ToList();

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
