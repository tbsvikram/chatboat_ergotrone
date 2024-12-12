namespace ErgotronChatbotApi.Model
{
    public class DashAssetTracking
    {
        public int IDSite { get; set; }
        public string SerialNo { get; set; }
        public string AssetNumber { get; set; }
        public string CartSerial { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string TypeIcon { get; set; }
        public string TypeColor { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Floor { get; set; }
        public string Wing { get; set; }
        public string EmTagBattLvl { get; set; }
        public int? IDDepartment { get; set; } // Nullable int
        public int? BeaconMinor { get; set; } // Nullable int
        public string LastReported { get; set; }
        public string EmTagOnlineSince { get; set; }
        public string AvgAmbientTemp { get; set; }
        public string ATDetail1Label { get; set; }
        public string ATDetail1Value { get; set; }
        public string ATDetail2Label { get; set; }
        public string ATDetail2Value { get; set; }
        public int? EmTagID { get; set; }
        public int? MinorID { get; set; } // Nullable int
        public int? IDArea { get; set; } // Nullable int
        public double? XPos { get; set; } // Nullable double
        public double? YPos { get; set; } // Nullable double
    }
}
