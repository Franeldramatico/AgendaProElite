using AgendaProElite.ViewModels;

namespace AgendaProElite.Views;

public partial class CalendarPage : ContentPage
{
	public CalendarPage(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is CalendarViewModel vm)
		{
			vm.LoadEventsCommand.Execute(null);
		}
	}
}

