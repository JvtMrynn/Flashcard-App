namespace FlashcardApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                var navigationPage = new NavigationPage(new AppShell());
                return new Window(navigationPage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating window: {ex.Message}");
                // Fallback to a basic window if navigation fails
                return new Window(new ContentPage { Content = new Label { Text = "Error loading app. Please restart." } });
            }
        }

        protected override async void OnStart()
        {
            try
            {
                var token = await SecureStorage.GetAsync("firebase_token");
                if (!string.IsNullOrEmpty(token))
                {
                    if (MainPage is AppShell appShell)
                    {
                        appShell.ShowMainContent();
                        appShell.HideAuthPages();
                        await Shell.Current.GoToAsync("//MainPage");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnStart: {ex.Message}");
                // Handle the error gracefully
                MainPage = new ContentPage
                {
                    Content = new VerticalStackLayout
                    {
                        Children =
                        {
                            new Label { Text = "Unable to initialize app. Please check your connection and try again." }
                        }
                    }
                };
            }
        }
    }
}