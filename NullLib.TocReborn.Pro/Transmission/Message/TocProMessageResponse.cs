namespace NullLib.TocReborn.Pro
{
    public class TocProMessageResponse
    {
        public TocProMessageResponse(int errorCode, string? message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public TocProMessageResponse()
        {
        }

        public int ErrorCode { get; set; }
        public string? Message { get; set; }

        public static TocProMessageResponse OK => new TocProMessageResponse(0, null);
    }
}
