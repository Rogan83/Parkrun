using Parkrun.MVVM.Helpers;
using Parkrun.MVVM.ViewModels;
using Parkrun.Services;
using System.Diagnostics;

namespace Parkrun.MVVM.Views;

public partial class ChartPage : ContentPage
{
	public ChartPage()
	{
		InitializeComponent();
        BindingContext = new ChartViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSync(); 
    }

    private void LoadDataSync()
    {
        if (BindingContext is ChartViewModel chartViewModel)
        {
            var data = DatabaseService.GetDataSync();
            if (data != null)
            {
                chartViewModel.Data = data;
                chartViewModel.UpdateChart();
                chartViewModel.UpdateChartWidth();
            }
        }
    }

    //void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    //{
    //    if (e.Status == GestureStatus.Running)
    //    {
    //        var view = (View)sender;
    //        double newScale = Math.Max(1, Math.Min(3, view.Scale * e.Scale)); // Zoom zwischen 1x und 3x begrenzen
    //        view.Scale = newScale;
    //    }
    //}

}