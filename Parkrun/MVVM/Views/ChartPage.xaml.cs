using Parkrun.MVVM.ViewModels;

namespace Parkrun.MVVM.Views;

public partial class ChartPage : ContentPage
{
	public ChartPage()
	{
		InitializeComponent();
        BindingContext = new ParkrunViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ParkrunViewModel viewModel)
        {
            viewModel.UpdateChart();
        }
    }

}