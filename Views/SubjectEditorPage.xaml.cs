using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class SubjectEditorPage : ContentPage
{
	public SubjectEditorPage()
	{
		InitializeComponent();

        var vm = new SubjectEditorPageViewModel();
        vm.ShowAlert = async (title, message, cancel) =>
        {
            await DisplayAlert(title, message, cancel);
        };

        BindingContext = vm;
	}
}