namespace ErgotronChatbotApi.Model
{
    public class AssetDetails
    {
        public string SerialNo { get; set; }
        public string IP { get; set; }
        public string AssetNumber { get; set; }
        public int IDAssetType { get; set; }
        public string DeviceMAC { get; set; }
        public string Notes { get; set; }
        public string Other { get; set; }
        public string CartSerial { get; set; }
    }

    public class OldAssetDetails
    {
        public int SITEID { get; set; }
        public int? DepartmentID { get; set; }
        public string SerialNo { get; set; }
        public string Description { get; set; }
        public string FriendlyDescription { get; set; }
        public string PartNo { get; set; }
        public DateTime? CreatedDateUTC { get; set; }
        public string Wing { get; set; }  // Can be null, so string is used
        public string Floor { get; set; } // Can be null, so string is used
        public string Notes { get; set; }
        public DateTime? LastPostDateUTC { get; set; }
        public string IP { get; set; }
    }
}
