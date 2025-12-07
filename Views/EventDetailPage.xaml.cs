using AgendaProElite.ViewModels;

namespace AgendaProElite.Views;

[QueryProperty(nameof(EventId), "eventId")]
public partial class EventDetailPage : ContentPage
{
	public EventDetailPage(EventViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	public string EventId { get; set; }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is EventViewModel vm && int.TryParse(EventId, out int id))
		{
			await vm.LoadEventCommand.ExecuteAsync(id);
		}
	}
}

