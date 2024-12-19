namespace ErgotronChatbotApi.Model
{
    public class EnvoyWorkstationDetails
    {
        public string Workstation { get; set; }
        public string Description { get; set; }
        public string AssetNumber { get; set; }
        public string LastReported { get; set; }
        public string DepartmentReporting { get; set; }
        public string FloorReporting { get; set; }
        public string StartUpdateAllow { get; set; }
        public string EndUpdateAllow { get; set; }
        public string LoggedInUser { get; set; }
        public int? AllowAutoUpdate { get; set; }
        public int? Mon { get; set; }
        public int? Tue { get; set; }
        public int? Wed { get; set; }
        public int? Thur { get; set; }
        public int? Fri { get; set; }
        public int? Sat { get; set; }
        public int? Sun { get; set; }
        public int? MedbinEnabled { get; set; }
        public int? MedbinLockTimeout { get; set; }
        public int? RhythmEnabled { get; set; }
        public int? RestartPCEnabled { get; set; }
        public int? DCCh1VoltageSetting { get; set; }
        public int? DCCh2VoltageSetting { get; set; }
        public int? DCCh3VoltageSetting { get; set; }
        public string MedbinPartnumber { get; set; }
        public string SystemVersion { get; set; }
        public string AdminOnlyReset { get; set; }
        public string AdminOnlyCalibrate { get; set; }
        public string Ssid { get; set; }
        public string LastPostDateUTC { get; set; }
    }
}
