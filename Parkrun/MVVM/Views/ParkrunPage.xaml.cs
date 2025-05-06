using Parkrun.MVVM.ViewModels;

namespace Parkrun.MVVM.Views;

public partial class ParkrunPage : ContentPage
{
	public ParkrunPage()
	{
		InitializeComponent();
		BindingContext = new ParkrunViewModel();
	}
}