namespace ErgotronChatbotApi.Model
{
    public class WarrantyDetails
    {
        public string Serial { get; set; }
        public string Description { get; set; }
        public string WarrantyDesc { get; set; }
        public string WarrantyStatus { get; set; }
        public string WarrantyEndDate { get; set; }
        public DateTime? LastCommunicated { get; set; }
    }
}
