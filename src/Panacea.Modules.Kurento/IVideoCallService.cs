using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.WebBrowsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using WampSharp.V2.Rpc;

namespace Panacea.Modules.Kurento
{
    public interface IVideoCallService
    {
        [WampProcedure("panacea.plugins.kurento.start")]
        void StartVideoCall(string url);
        [WampProcedure("panacea.plugins.kurento.surveillance")]
        void StartVideoSurveillance(string url);
    }



    public class VideoCallService : IVideoCallService
    {
        private readonly PanaceaServices _core;

        public VideoCallService(PanaceaServices core)
        {
            _core = core;
        }

        public void StartVideoCall(string url)
        {
            _core.Logger.Info(this, "StartVideoCall");
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_core.TryGetUiManager(out IUiManager ui)
                && _core.TryGetWebBrowser(out IWebBrowserPlugin web))
                {
                    ui.Toast("A video call is being established");
                    web.OpenUnmanaged(url);
                }
            });
        }

        public void StartVideoSurveillance(string url)
        {
            _core.Logger.Info(this, "StartVideoSurveillance");
            System.Windows.Application.Current.Dispatcher.Invoke(async () =>
            {
                var manager = await _core.PluginLoader.GetPlugins<IWebViewPlugin>().FirstOrDefault()?.GetWebViewManagerAsync();
                if (manager == null) return;
                Debug.WriteLine(url);
        
                var tab = manager.CreateTab(url);
                Window newwindow = new Window()
                {
                    ShowInTaskbar = false,
                    WindowState = WindowState.Minimized,
                    Visibility = Visibility.Hidden
                };
                newwindow.Content = tab;
                newwindow.Show();
                tab.Close += (oo, ee) =>
                {
                    newwindow.Close();
                    tab.Dispose();
                };

                //todo _comm.OpenUri("chromiumhidden:?url=" + HttpUtility.UrlEncode(url));
            });
        }
    }
}
