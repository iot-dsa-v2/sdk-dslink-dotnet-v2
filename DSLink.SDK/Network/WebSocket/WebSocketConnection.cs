using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DSLink.SDK.Network.WebSocket
{
    public class WebSocketConnection : BaseConnection
    {
        private readonly ClientWebSocket _ws;
        private readonly CancellationTokenSource _tokenSource;
        
        public WebSocketConnection(LinkConfig config) : base(config)
        {
            _ws = new ClientWebSocket();
            _tokenSource = new CancellationTokenSource();
        }

        public override async Task Connect()
        {
            await _ws.ConnectAsync(new Uri("wss://echo.websocket.org"), CancellationToken.None);
            _startWatchTask();
            EmitOpen();
        }

        public override async Task Disconnect()
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server Close", _tokenSource.Token);
            _tokenSource.Cancel();
            EmitClose();
        }

        public override Task Write(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _tokenSource.Token);
        }

        public override Task Write(byte[] bytes)
        {
            return _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, _tokenSource.Token);
        }

        private void _startWatchTask()
        {
            Task.Run(async () =>
            {
                var token = _tokenSource.Token;

                while (_ws.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024];
                    var bufferUsed = 0;
                    var bytes = new List<byte>();
                    var str = "";
                    
                    RECV:
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    
                    Console.WriteLine("loop");

                    if (result == null)
                    {
                        goto RECV;
                    }

                    bufferUsed = result.Count;
                    
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            await Disconnect();
                            break;
                        case WebSocketMessageType.Text:
                            {
                                str += Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                                if (!result.EndOfMessage)
                                    goto RECV;
                                EmitText(str);
                            }
                            break;
                        case WebSocketMessageType.Binary:
                            {
                                var newBytes = new byte[bufferUsed];
                                Array.Copy(buffer, newBytes, bufferUsed);
                                bytes.AddRange(newBytes);
                                if (!result.EndOfMessage)
                                    goto RECV;
                                EmitBinary(bytes.ToArray());
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }

                return Task.CompletedTask;
            });
        }
    }
}