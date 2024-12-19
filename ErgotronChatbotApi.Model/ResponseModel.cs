namespace ErgotronChatbotApi.Model
{
    public class ResponseModel<T>
    {
        public T Response { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
    }
}
