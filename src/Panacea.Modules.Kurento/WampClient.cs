using Panacea.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace Panacea.Modules.Kurento
{
    public class WampClient
    {
        IWampChannel _remoteChannel;
        const string location = "ws://127.0.0.1:64999/";
        const string realm = "Export";
        WampChannelReconnector _reconnector;

        IVideoCallService _service;
        private readonly ILogger _logger;

        public WampClient(IVideoCallService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public void SetupRemoteConnection()
        {
            var factory = new DefaultWampChannelFactory();

            _remoteChannel =
                factory.CreateJsonChannel(location, realm);

            Task connect()
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        _logger.Info(this, "Trying to connect");
                        _remoteChannel.Open().Wait(5000);
                        IWampRealmProxy realm = _remoteChannel.RealmProxy;
                        _logger.Info(this, "registering RPC");
                        await realm.Services.RegisterCallee(_service);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(this, ex.Message);
                        var exe = ex;
                        while(exe.InnerException != null)
                        {
                            exe = exe.InnerException;
                            _logger.Error(this, exe.Message);
                        }
                        throw;
                    }
                });

            }

            _reconnector = new WampChannelReconnector(_remoteChannel, connect);

            _reconnector.Start();
        }
    }
}
