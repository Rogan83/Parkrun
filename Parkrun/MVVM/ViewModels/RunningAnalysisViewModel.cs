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
        public double MS { get; set; } = 0;

        public ICommand CreateAnalysis { get; }

        public RunningAnalysisViewModel()
        {
            CreateAnalysis = new Command(() =>
            {
                KmH = (int)(SelectedRun.DistanceKm / SelectedRun.Time.TotalHours);
            });
        }

        public void CalculateStatistics()
        {
            if (Data.Count > 0)
            {
                if (parkrunIndex < 0)
                    parkrunIndex = 0;
                KmH = (double)(Data[parkrunIndex].DistanceKm / Data[parkrunIndex].Time.TotalHours);
                MS = (double)(Data[parkrunIndex].DistanceKm * 1000 / Data[parkrunIndex].Time.TotalSeconds);
            }
        }
    }
}
