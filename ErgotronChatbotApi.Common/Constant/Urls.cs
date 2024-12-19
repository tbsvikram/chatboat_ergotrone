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
        public const string roiBusiestDayWeek = "reporting.ROI_BusiestDayWeek";
    }
    public class MetaData
    {
        public const string is_ws_loc = "is_ws_loc";
        public const string is_asset_loc = "is_asset_loc";
        public const string is_device_loc = "is_device_loc";
        public const string is_all_asset_loc = "is_all_asset_loc";
        public const string is_count_user = "is_count_user";
        public const string subject = "subject";
        public const string is_last_contact_ws = "is_last_contact_ws";
        public const string is_most_dep_ws = "is_most_dep_ws";
        public const string is_latestip_ws = "is_latestip_ws";
        public const string is_software_up_date_ws = "is_software_up_date_ws";
        public const string is_unoccupied_ws = "is_unoccupied_ws";
        public const string btry_bng_usd = "btry_bng_usd";
        public const string is_expire = "is_expire";
        public const string is_online_offline = "is_online_offline";
        public const string frgt_pass = "frgt_pass";
        public const string envy_ws_dtl = "envy_ws_dtl";
        public const string workstation_online = "workstation_online";
        public const string chrg_dtl = "chrg_dtl";
        public const string btry_hlth = "btry_hlth";
        public const string decommissioned_dtl = "decommissioned_dtl";
        public const string hgst_usg = "hgst_usg";
        public const string old_ast = "old_ast";
        public const string btry_chrg_dtl = "btry_chrg_dtl";
        public const string busist_day_week = "busist_day_week";
        public const string is_userid = "is_userid";
        public const string is_siteid = "is_siteid";
        public const string ws_log = "ws_log";
        public const string ws_pwr_off = "ws_pwr_off";
    }
}
