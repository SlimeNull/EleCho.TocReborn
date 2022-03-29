namespace NullLib.TocReborn
{
    public class MessageEventArgs
    {
        public TocMessage? Message { get; }

        public MessageEventArgs(TocMessage? message) => Message = message;
    }
}