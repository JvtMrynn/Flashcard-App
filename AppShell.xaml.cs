namespace FlashcardApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register simplified routes for easier navigation
            Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(Views.RegisterPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("SubjectPage", typeof(Views.SubjectPage));
            Routing.RegisterRoute("SubjectEditorPage", typeof(Views.SubjectEditorPage));
            Routing.RegisterRoute("FlashcardPage", typeof(Views.FlashcardPage));
            Routing.RegisterRoute("FlashcardEditorPage", typeof(Views.FlashcardEditorPage));

            HideMainContent();

            // Check login state on app start
            CheckLoggedInStatus();
        }

        private async void CheckLoggedInStatus()
        {
            var token = await SecureStorage.GetAsync("firebase_token");

            if (!string.IsNullOrEmpty(token))
            {
                // ✅ User is logged in
                ShowMainContent();
                HideAuthPages();
                await GoToAsync("//MainPage"); // Optional: go to main page
            }
            else
            {
                // ❌ Not logged in, send to LoginPage
                await GoToAsync("//LoginPage");
            }
        }

        public void HideMainContent()
        {
            // Hide Dashboard
            if (MainPageFlyoutItem != null && Items.Contains(MainPageFlyoutItem))
                Items.Remove(MainPageFlyoutItem);
                
            // Hide Subject
            if (SubjectFlyoutItem != null && Items.Contains(SubjectFlyoutItem))
                Items.Remove(SubjectFlyoutItem);
                
            // Hide Flashcard
            if (FlashcardFlyoutItem != null && Items.Contains(FlashcardFlyoutItem))
                Items.Remove(FlashcardFlyoutItem);
                
            // Make sure the editors are not visible in the flyout
            if (SubjectEditorShellContent != null)
                SubjectEditorShellContent.FlyoutItemIsVisible = false;
                
            if (FlashcardEditorShellContent != null)
                FlashcardEditorShellContent.FlyoutItemIsVisible = false;
        }

        public void ShowMainContent()
        {
            // Show Dashboard
            if (MainPageFlyoutItem != null && !Items.Contains(MainPageFlyoutItem))
                Items.Add(MainPageFlyoutItem);
                
            // Do not add Subject and Flashcard to flyout - we'll only show Dashboard
            // Users will navigate to these pages through the Dashboard UI
            
            // Ensure editors are not visible in the flyout
            if (SubjectEditorShellContent != null)
                SubjectEditorShellContent.FlyoutItemIsVisible = false;
                
            if (FlashcardEditorShellContent != null)
                FlashcardEditorShellContent.FlyoutItemIsVisible = false;
        }

        public void HideAuthPages()
        {
            if (LoginFlyoutItem != null && Items.Contains(LoginFlyoutItem))
                Items.Remove(LoginFlyoutItem);

            if (RegisterFlyoutItem != null && Items.Contains(RegisterFlyoutItem))
                Items.Remove(RegisterFlyoutItem);
        }

        public void ShowAuthPages()
        {
            if (LoginFlyoutItem != null && !Items.Contains(LoginFlyoutItem))
                Items.Insert(0, LoginFlyoutItem);

            if (RegisterFlyoutItem != null && !Items.Contains(RegisterFlyoutItem))
                Items.Insert(1, RegisterFlyoutItem);
        }
    }
}
