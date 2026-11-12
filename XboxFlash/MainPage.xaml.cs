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
                try
                {
                    Web.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    Web.NavigationFailed += async (ws, we) => await Show("Navigation failed: " + we.WebErrorStatus);
                    Web.NavigateToLocalStreamUri(new Uri("ms-local-stream://xboxflash_www/index.html"), new StreamResolver());
                }
                catch (Exception ex)
                {
                    await Show("Startup error: " + ex.GetType().Name + "\n" + ex.Message);
                }
            };
        }

        private async System.Threading.Tasks.Task Show(string msg)
        {
            try { await new Windows.UI.Popups.MessageDialog(msg, "XboxFlash").ShowAsync(); } catch { }
        }
    }

    // Maps ms-local-stream://xboxflash_www/<path> to <install folder>\www\<path>
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
                var rel = Uri.UnescapeDataString(uri.AbsolutePath).TrimStart('/');
                var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("www");
                var file = await folder.GetFileAsync(rel.Replace('/', '\\'));
                var stream = await file.OpenReadAsync();
                return stream.GetInputStreamAt(0);
            }
            catch { return null; }
        }
    }
}
