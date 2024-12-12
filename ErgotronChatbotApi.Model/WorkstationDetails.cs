using Newtonsoft.Json;

namespace ErgotronChatbotApi.Model
{
    public class WorkstationDetails
    {
        [JsonProperty("Workstation")]
        public string Workstation { get; set; }

        public int SiteId { get; set; }

        [JsonProperty("AssetNumber")]
        public string AssetNumber { get; set; }

        public string Description { get; set; }

        [JsonProperty("Department Assigned")]
        public string DepartmentAssigned { get; set; }

        [JsonProperty("Department Reporting")]
        public string DepartmentReporting { get; set; }

        [JsonProperty("Floor Assigned")]
        public string FloorAssigned { get; set; }

        [JsonProperty("Floor Reporting")]
        public string FloorReporting { get; set; }

        [JsonProperty("Wing Assigned")]
        public string WingAssigned { get; set; }

        [JsonProperty("Wing Reporting")]
        public string WingReporting { get; set; }

        public string Location { get; set; }

        public string CartID { get; set; }

        [JsonProperty("PC Serial")]
        public string PCSerial { get; set; }

        [JsonProperty("Cart Serial")]
        public string CartSerial { get; set; }

        [JsonProperty("Printer Serial")]
        public string PrinterSerial { get; set; }

        [JsonProperty("Monitor Serial")]
        public string MonitorSerial { get; set; }

        [JsonProperty("Last Reported")]
        public string LastReported { get; set; }

        public string Notes { get; set; }
        public string Other { get; set; }
        public bool Move { get; set; }
        public string IP { get; set; }

        [JsonProperty("DeviceMAC")]
        public string DeviceMAC { get; set; }

        [JsonProperty("warrantyexp")]
        public string WarrantyExp { get; set; }

        public int? WarrantyYears { get; set; }

        public DateTime? LastPostDateUTC { get; set; }
        public DateTime? CurrentTimeUTC { get; set; }
        public DateTime? MFGDate { get; set; }
    }
}
