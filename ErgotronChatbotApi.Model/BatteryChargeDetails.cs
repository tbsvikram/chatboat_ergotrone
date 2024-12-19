using Newtonsoft.Json;

namespace ErgotronChatbotApi.Model
{
    public class BatteryChargeDetails
    {
        public string Serial { get; set; }
        public string Generation { get; set; }
        public DateTime? LastPostDateUTC { get; set; }
        public string Description { get; set; }

        [JsonProperty("Last Used")]
        public string LastUsed { get; set; }
        public double? ChargeLevel { get; set; }
        public double? CapacityHealth { get; set; }

        [JsonProperty("Est Run Time")]
        public double? EstRunTime { get; set; }

        public int? CycleCount { get; set; }

        [JsonProperty("Reporting Floor")]
        public string ReportingFloor { get; set; }
        public double? Amps { get; set; }
        public DateTime? CurrentTimeUTC { get; set; }
        public string RemoveFromField { get; set; }
        public int? NeedsLearnCycle { get; set; }
        public int? MaxCycleCount { get; set; }
        public int? RemainingCycles { get; set; }

        [JsonProperty("Reporting Wing")]
        public string ReportingWing { get; set; }

        [JsonProperty("Reporting Department")]
        public string ReportingDepartment { get; set; }
        public int? IDAsset { get; set; }
        public string WarrantyExp { get; set; }
        public double? WarrantyYears { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
    }
}
