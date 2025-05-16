using Parkrun.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Parkrun.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class RunningAnalysisViewModel
    {
        public List<ParkrunData> Data { get; set; } = new List<ParkrunData>();

        public ParkrunData SelectedRun { get; set; } = new ParkrunData();

       

        private int parkrunIndex;
        public int ParkrunIndex
        {
            get => parkrunIndex;
            set
            {
                parkrunIndex = value;
                CalculateStatistics();
            }
        }

        public double KmH { get; set; } = 0;
        public double MaxKmH { get; set; } = 0;
        public double MinKmH { get; set; } = 0;
        public double MS { get; set; } = 0;
        public double BestTimeMS { get; set; } = 0;

        public int NumberOfRuns { get; set; }

        public int BestTime { get; set; }
        public int WorstTime { get; set; }

        public ICommand CreateAnalysis { get; }

        public RunningAnalysisViewModel()
        {
            CreateAnalysis = new Command(() =>
            {
                CalculateStatistics();
            });
        }

        public void CalculateStatistics()
        {
            if (Data.Count > 0 && parkrunIndex < Data.Count)
            {
                KmH = Data[parkrunIndex].DistanceKm / Data[parkrunIndex].Time.TotalHours;
                MS = Data[parkrunIndex].DistanceKm * 1000 / Data[parkrunIndex].Time.TotalSeconds;
                NumberOfRuns = Data.Count;

                BestTime = Data.Min(d => d.Time.Minutes);
                WorstTime = Data.Max(d => d.Time.Minutes);

                MaxKmH = Data.Max(d => d.DistanceKm / d.Time.TotalHours);
                MinKmH = Data.Min(d => d.DistanceKm / d.Time.TotalHours);
                BestTimeMS = Data.Min(d => d.DistanceKm * 1000 / d.Time.TotalSeconds);
            }
        }
    }
}
