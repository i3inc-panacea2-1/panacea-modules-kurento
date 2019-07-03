using Panacea.Core;
using Panacea.Interop;
using Panacea.Modularity;
using Panacea.Modularity.UiManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Kurento
{
    class KurentoPlugin
    {
    }
    public class KurentoVideoCallPlugin : IPlugin
    {

        public KurentoVideoCallPlugin(
            PanaceaServices core)
        {
            _core = core;
        }
        Process wampProxy;
        private WampClient _wampClient;
        private readonly PanaceaServices _core;

        private void startWampProxy()
        {
            wampProxy = new Process();
            wampProxy.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wampProxy", "ServiceHost.exe");
            wampProxy.StartInfo.UseShellExecute = false;
            wampProxy.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            wampProxy.StartInfo.CreateNoWindow = true;
            wampProxy.Start();
            wampProxy.BindToCurrentProcess();
        }


        public void Dispose()
        {
            wampProxy.Close();
        }

        public Task BeginInit()
        {
            startWampProxy();
            _wampClient = new WampClient(new VideoCallService(_core), _core.Logger);
            _wampClient.SetupRemoteConnection();
            return Task.CompletedTask;
        }

        public Task EndInit()
        {
            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
