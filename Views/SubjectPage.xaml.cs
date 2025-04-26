using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class SubjectPage : ContentPage
{
	public SubjectPage()
	{
		InitializeComponent();

        BindingContext = new SubjectPageViewModel();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is SubjectPageViewModel vm)
        {
            vm.ReloadSubjects();
        }
    }

}