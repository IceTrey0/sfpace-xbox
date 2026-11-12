using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XboxFlash
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += (s, e) => { e.Handled = true; };
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            try { this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested; } catch { }
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
