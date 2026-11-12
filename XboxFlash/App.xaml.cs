using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XboxFlash
{
    sealed partial class App : Application
    {
        public App() { this.InitializeComponent(); }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Turn off the Xbox "mouse mode" cursor so the gamepad goes straight to the page
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView()
                .SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                frame = new Frame();
                Window.Current.Content = frame;
            }
            if (frame.Content == null) frame.Navigate(typeof(MainPage));
            Window.Current.Activate();
        }
    }
}
