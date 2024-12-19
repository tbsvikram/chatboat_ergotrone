using ErgotronChatbotApi.Common.Utility;

namespace ErgotronChatbotApi.Common.Constants
{
    public class Urls
    {
        public static string apiUrl = AppSettings.ApiEndPoint;

        public static string azureAIUrl = AppSettings.AzureApiEndPoint;

        public const string requestUrl = "api/StoredProc/";
    }

    public class MediaType
    {
        public const string jsonType = "application/json";
    }

    public class StoreProcedure
    {
        public const string prcDashAssetTracking = "dbo.prcDashAssetTracking";
        public const string prcGetWorkstationUsers = "dbo.PrcGetWorkstationUsers";
        public const string prcDashGetBatteryWarranties = "dbo.prcDashGetBatteryWarranties";
        public const string prcDashDrawerLog = "dbo.prcDashDrawerLog";
        public const string prcDashGetWorkstations = "dbo.prcDashGetWorkstations";
        public const string prcDashGetBattery = "dbo.prcDashGetBattery";
        public const string prcDashGetWorkstationWarranties = "dbo.prcDashGetWorkstationWarranties";
        public const string prcDashAssetsReporting = "dbo.prcDashAssetsReporting";
        public const string prcDashGetUsersBySite = "dbo.prcDashGetUsersBySite";
        public const string prcDashGetEnvoyWorkstations = "dbo.prcDashGetEnvoyWorkstations";
        public const string prcDashGetNotificationsPage = "dbo.prcDashGetNotificationsPage";
        public const string prcGetOnlineWorkstationCountHistory = "dbo.prcGetOnlineWorkstationCountHistory";
        public const string prcDashGetCharger = "dbo.prcDashGetCharger";
        public const string spDashboardBatteryHealthLevels = "dbo.spDashboardBatteryHealthLevels";
        public const string prcDashGetAssetSearchList = "dbo.prcDashGetAssetSearchList";
        public const string roiHighestUsage = "reporting.ROI_HighestUsage";
        public const string prcBeepWorkstation = "dbo.prcBeepWorkstation";
        public const string chatBotOldestAsset = "dbo.ChatBot_OldestAsset";
        public const string prcDashGetWorkstationPacketsFast = "dbo.prcDashGetWorkstationPacketsFast";
        public const string uspGetBatteryCount = "dbo.uspGetBatteryCount";
        public const string roiBusiestDayWeek = "reporting.ROI_BusiestDayWeek";
    }
}
