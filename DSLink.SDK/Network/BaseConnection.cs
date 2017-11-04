using System;
using System.Threading.Tasks;

namespace DSLink.SDK.Network
{
    public abstract class BaseConnection
    {
        public readonly LinkConfig Config;

        public event Action OnOpen;
        public event Action OnClose;
        public event Action<MessageEvent> OnMessage;

        protected BaseConnection(LinkConfig config)
        {
            Config = config;
        }
        
        public abstract Task Connect();
        public abstract Task Disconnect();
        public abstract Task Write(string str);
        public abstract Task Write(byte[] bytes);

        protected virtual void EmitOpen()
        {
            OnOpen?.Invoke();
        }

        protected virtual void EmitClose()
        {
            OnClose?.Invoke();
        }

        protected virtual void EmitText(string str)
        {
            OnMessage?.Invoke(new MessageEvent(MessageEventType.Text, str));
        }

        protected virtual void EmitBinary(byte[] bytes)
        {
            OnMessage?.Invoke(new MessageEvent(MessageEventType.Binary, bytes));
        }
    }
}