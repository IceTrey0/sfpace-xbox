using System;
using Windows.UI.Xaml.Controls;

namespace XboxFlash
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += async (s, e) =>
            {
                Web.NavigationFailed += async (ws, we) => await Show("Navigation failed: " + we.WebErrorStatus + "\n" + we.Uri);

                // Try the stream-resolver route first (lets fetch()/WASM work), then plain ms-appx-web.
                string[] streamUris =
                {
                    "ms-local-stream://xboxflash_0a1b2c3d4e5f/www/index.html",
                    "ms-local-stream://xboxflash_www/www/index.html",
                };
                foreach (var u in streamUris)
                {
                    try { Web.NavigateToLocalStreamUri(new Uri(u), new StreamResolver()); return; }
                    catch (Exception) { }
                }
                try { Web.Navigate(new Uri("ms-appx-web:///www/index.html")); }
                catch (Exception ex) { await Show("Startup error: " + ex.GetType().Name + "\n" + ex.Message); }
            };
        }

        private async System.Threading.Tasks.Task Show(string msg)
        {
            try { await new Windows.UI.Popups.MessageDialog(msg, "XboxFlash").ShowAsync(); } catch { }
        }
    }

    // Maps ms-local-stream://<host>/<path> to <install folder>\<path>
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
                var rel = Uri.UnescapeDataString(uri.AbsolutePath).TrimStart('/').Replace('/', '\\');
                var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(rel);
                var stream = await file.OpenReadAsync();
                return stream.GetInputStreamAt(0);
            }
            catch { return null; }
        }
    }
}
