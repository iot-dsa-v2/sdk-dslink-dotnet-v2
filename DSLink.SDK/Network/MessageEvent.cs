namespace DSLink.SDK.Network
{
    public class MessageEvent
    {
        public readonly MessageEventType MessageType;
        public readonly dynamic Message;

        public MessageEvent(MessageEventType type, dynamic message)
        {
            MessageType = type;
            Message = message;
        }
    }

    public enum MessageEventType
    {
        Text,
        Binary
    }
}