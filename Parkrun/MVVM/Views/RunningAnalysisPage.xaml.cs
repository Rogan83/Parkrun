using Parkrun.MVVM.ViewModels;
using Parkrun.Services;

namespace Parkrun.MVVM.Views;

public partial class RunningAnalysisPage : ContentPage
{
	public RunningAnalysisPage()
	{
		InitializeComponent();
		BindingContext = new RunningAnalysisViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSync();
    }

    private void LoadDataSync()
    {
        if (BindingContext is RunningAnalysisViewModel runningAnalysisViewModel)
        {
            var data = DatabaseService.GetDataSync();
            if (data != null)
            {
                runningAnalysisViewModel.Data = data;
                runningAnalysisViewModel.SelectedRun = data.FirstOrDefault();
            }

            runningAnalysisViewModel.CalculateStatistics();
        }
    }
}