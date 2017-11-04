using System;
using System.Threading.Tasks;
using DSLink.SDK.Network;
using DSLink.SDK.Network.WebSocket;

namespace DSLink.SDK.Examples.RNG
{
    class RNGDSLink
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var ws = new WebSocketConnection(null);

                ws.OnMessage += msgEvent =>
                {
                    switch (msgEvent.MessageType)
                    {
                        case MessageEventType.Text:
                            Console.WriteLine(msgEvent.Message);
                            break;
                        case MessageEventType.Binary:
                            Console.WriteLine(BitConverter.ToString(msgEvent.Message));
                            break;
                    }
                };

                await ws.Connect();
                await ws.Write("Test");
                await ws.Write(new byte[] {
                    0x01, 0x02, 0x03, 0x04, 0x05
                });
                await Task.Delay(2000);
                await ws.Disconnect();
            });

            Console.ReadLine();
        }
    }
}