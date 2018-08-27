using AppKit;
using Foundation;
using UnityTabFromXamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace UnityTab.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var rect = new CoreGraphics.CGRect(200, 200, 800, 600);
            this._window = new NSWindow(rect, style, NSBackingStore.Buffered, false)
            {
                Title = "Hello, Xamarin Forms!"
            };
        }

        private NSWindow _window;

        public override NSWindow MainWindow => this._window;

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
