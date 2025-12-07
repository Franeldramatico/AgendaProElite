using AgendaProElite.ViewModels;

namespace AgendaProElite.Views;

public partial class TaskPage : ContentPage
{
	public TaskPage(TaskViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is TaskViewModel vm)
		{
			vm.LoadTasksCommand.Execute(null);
		}
	}
}

