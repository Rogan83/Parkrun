using Parkrun.MVVM.Models;
using Parkrun.MVVM.ViewModels;
using Parkrun.Services;

namespace Parkrun.MVVM.Views;

public partial class ParkrunPage : ContentPage
{
	public ParkrunPage()
	{
		InitializeComponent();
		BindingContext = new ParkrunViewModel();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Daten neu laden
        LoadDataSync();
    }

    private void LoadDataSync()
    {
        if (BindingContext is ParkrunViewModel parkrunViewModel)
        {
            var data = DatabaseService.GetDataSync();
            if (data != null)
            {
                parkrunViewModel.Data = new System.Collections.ObjectModel.ObservableCollection<ParkrunData>(data.OrderBy(x => x.Date));
            }
        }
    }
}