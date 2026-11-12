using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace XboxFlash
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Web.Loaded += (s, e) =>
            {
                Web.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                // Serve app files at ms-local-stream:// so fetch()/WASM works (ms-appx-web blocks it)
                Web.NavigateToLocalStreamUri(
                    new Uri("ms-local-stream://" + Windows.ApplicationModel.Package.Current.Id.Name + "_www/index.html"),
                    new StreamResolver());
            };
            Web.NavigationFailed += (s, e) => ShowErr("Navigation failed: " + e.WebErrorStatus);
        }

        private async void ShowErr(string msg)
        {
            var d = new Windows.UI.Popups.MessageDialog(msg, "XboxFlash");
            await d.ShowAsync();
        }
    }

    // Maps ms-local-stream://<pkg>_www/<path> to <install folder>\www\<path>
    public sealed class StreamResolver : Windows.Web.IUriToStreamResolver
    {
        public Windows.Foundation.IAsyncOperation<Windows.Storage.Streams.IInputStream> UriToStreamAsync(Uri uri)
        {
            return GetStream(uri).AsAsyncOperation();
        }

        private async System.Threading.Tasks.Task<Windows.Storage.Streams.IInputStream> GetStream(Uri uri)
        {
            try
            {
                var path = Uri.UnescapeDataString(uri.AbsolutePath).TrimStart('/').Replace('/', '\\');
                var root = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("www");
                var file = await root.GetFileAsync(path);
                var stream = await file.OpenReadAsync();
                return stream.GetInputStreamAt(0);
            }
            catch { return null; }
        }
    }
}
