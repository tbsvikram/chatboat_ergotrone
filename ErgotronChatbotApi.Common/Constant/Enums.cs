namespace ErgotronChatbotApi.Common.Constant
{
    public class Enums
    {
        public enum StatusCode
        {
            OK = 200,
            Created,
            Accepted,
            BadRequest = 400,
            Unauthorized,
            PaymentRequired,
            Forbidden,
            NotFound,
            InternalError = 500,
            RecordNotInserted
        }
    }
}
