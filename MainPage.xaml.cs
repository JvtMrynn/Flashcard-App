using FlashcardApp.ViewModels;

namespace FlashcardApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        private MainViewModel _viewModel;
        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            _viewModel.ShowAlert = async (title, message, cancel) =>
            {
                await DisplayAlert(title, message, cancel);
            };

            BindingContext = _viewModel;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        
        
    }

}
