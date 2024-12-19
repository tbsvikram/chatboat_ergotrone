using Newtonsoft.Json;

namespace ErgotronChatbotApi.Model
{
    public class WorkstationUserDetails
    {
        public string SiteName { get; set; }
        public int? WorkstationUserID { get; set; }
        public string Username { get; set; }

        [JsonProperty("First Name")]
        public string FirstName { get; set; }

        [JsonProperty("Last Name")]
        public string LastName { get; set; }

        [JsonProperty("Sit Position")]
        public int? SitPosition { get; set; }

        [JsonProperty("Stand Position")]
        public int? StandPosition { get; set; }

        [JsonProperty("KeyBoard Light")]
        public int? KeyBoardLight { get; set; }

        [JsonProperty("Display Brightness")]
        public int? DisplayBrightness { get; set; }

        [JsonProperty("Unique ID")]
        public string UniqueID { get; set; }

        [JsonProperty("Medbin Code")]
        public string MedbinCode { get; set; }
    }
}
