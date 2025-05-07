using Parkrun.MVVM.ViewModels;
using Parkrun.Services;

namespace Parkrun.MVVM.Views;

public partial class ChartPage : ContentPage
{
	public ChartPage()
	{
		InitializeComponent();
        BindingContext = new ChartViewModel();
    }

    protected override async void OnAppearing()
    {
        DatabaseService databaseService = new();

        base.OnAppearing();
        if (BindingContext is ChartViewModel chartViewModel)
        {
            chartViewModel.Data = await databaseService.GetDataAsync();
            chartViewModel.UpdateChart();
        }
    }
}