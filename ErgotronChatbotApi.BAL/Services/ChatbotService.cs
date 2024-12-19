using ErgotronChatbotApi.BAL.Interfaces;
using ErgotronChatbotApi.Common.Constant;
using ErgotronChatbotApi.Common.Constants;
using ErgotronChatbotApi.Common.Utility;
using ErgotronChatbotApi.Model;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Globalization;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ErgotronChatbotApi.BAL.Services
{
    /// <summary>
    /// ChatbotService is responsible for handling chatbot-related operations and dependencies.
    /// </summary>
    /// <remarks>
    /// This class integrates with an external API service to fetch and process data. 
    /// It also defines common constants, such as the <c>sourceArray</c>, used to filter out invalid or unassigned asset data.
    /// </remarks>
    public class ChatbotService : IChatbotService
    {
        // Dependency for API operations
        private readonly IApiService _apiService;

        // Constants to identify unassigned or invalid asset entries
        private static readonly string[] sourceArray = ["NOT ASSIGNED", "N/A"];
        private static readonly string Unassigned = "Unassigned";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatbotService"/> class.
        /// </summary>
        /// <param name="apiService">An instance of <see cref="IApiService"/> to handle external API interactions.</param>
        public ChatbotService(IApiService apiService)
        {
            _apiService = apiService;
        }

        #region Get Responses
        /// <summary>
        /// Asynchronously processes a question and fetches the corresponding response based on the input parameters.
        /// </summary>
        /// <param name="siteId">An optional integer representing the site identifier.</param>
        /// <param name="userId">An optional string representing the user identifier.</param>
        /// <param name="questionAnswerResponse">An instance of <see cref="QuestionAnswerResponse"/> containing the question details and metadata.</param>
        /// <returns>
        /// A <see cref="Task"/> that resolves to a <see cref="ResponseModel{T}"/> containing the response as a string, a status code, and a message.
        /// </returns>
        public async Task<ResponseModel<string>> GetQuestionResponseAsync(int? siteId, string? userId, QuestionAnswerResponse questionAnswerResponse)
        {
            // Initialize the response model
            var responseModel = new ResponseModel<string>();
            var parm = new Dictionary<string, string>();

            // Extract metadata values
            questionAnswerResponse.metaData.TryGetValue(MetaData.is_userid, out var is_userid);
            questionAnswerResponse.metaData.TryGetValue(MetaData.is_siteid, out var is_siteid);

            // Conditionally add parameters based on the metadata values
            if (bool.TryParse(is_userid, out var is_useridExist) && is_useridExist)
            {
                parm.Add("@UserId", userId ?? string.Empty);
            }

            if (bool.TryParse(is_siteid, out var is_siteidExist) && is_siteidExist)
            {
                parm.Add("@SiteId", siteId?.ToString() ?? string.Empty);
            }

            // Call the API service and fetch results
            JArray result = await _apiService.PostAsync(questionAnswerResponse.answer, parm);

            // Build the response model
            responseModel.Response = GetQuestionResponse(questionAnswerResponse, result);
            responseModel.ResponseCode = (int)Enums.StatusCode.OK;
            responseModel.Message = "Success";

            return responseModel;
        }

        /// <summary>
        /// Retrieves the response for a question based on the provided metadata and stored procedures.
        /// </summary>
        /// <param name="questionAnswerResponse">The question-answer response containing query details and metadata.</param>
        /// <param name="result">The result data as a JArray.</param>
        /// <returns>The generated response string, or an empty string if no match is found.</returns>
        private static string GetQuestionResponse(QuestionAnswerResponse questionAnswerResponse, JArray result)
        {
            // Define mapping of metadata keys to corresponding methods
            var metaDataMap = GetFunction(questionAnswerResponse);

            // List of valid stored procedures for matching
            var validProcedures = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                {
                                    StoreProcedure.prcDashAssetTracking,
                                    StoreProcedure.prcGetWorkstationUsers,
                                    StoreProcedure.prcDashGetBatteryWarranties,
                                    StoreProcedure.prcDashGetWorkstations,
                                    StoreProcedure.prcDashGetBattery,
                                    StoreProcedure.prcDashGetWorkstationWarranties,
                                    StoreProcedure.prcDashAssetsReporting,
                                    StoreProcedure.prcDashGetUsersBySite,
                                    StoreProcedure.prcDashGetEnvoyWorkstations,
                                    StoreProcedure.prcGetOnlineWorkstationCountHistory,
                                    StoreProcedure.prcDashGetCharger,
                                    StoreProcedure.spDashboardBatteryHealthLevels,
                                    StoreProcedure.prcDashGetAssetSearchList,
                                    StoreProcedure.roiHighestUsage,
                                    StoreProcedure.chatBotOldestAsset,
                                    StoreProcedure.roiBusiestDayWeek,
                                    StoreProcedure.prcDashDrawerLog
                                };

            // Check if the answer matches any valid stored procedure
            if (validProcedures.Contains(questionAnswerResponse.answer))
            {
                foreach (var itemKey in questionAnswerResponse.metaData.Keys)
                {
                    if (metaDataMap.TryGetValue(itemKey, out var function))
                    {
                        return function(result);
                    }
                }
            }

            return string.Empty; // Return an empty string if no match is found
        }

        /// <summary>
        /// Creates a mapping of query types to their respective processing functions.
        /// </summary>
        /// <param name="questionAnswerResponse">
        /// An object containing query details, metadata, and other information required for processing responses.
        /// </param>
        /// <returns>
        /// A dictionary where the key represents a query type, and the value is a function 
        /// that takes a <see cref="JArray"/> and returns a formatted response string.
        /// </returns>
        private static Dictionary<string, Func<JArray, string>> GetFunction(QuestionAnswerResponse questionAnswerResponse)
        {
            return new Dictionary<string, Func<JArray, string>>
                {
                    { MetaData.is_ws_loc, res => GetWorkstationLocationDetails(questionAnswerResponse.query, res) },
                    { MetaData.is_asset_loc, res => GetAssetLocationDetails(res) },
                    { MetaData.is_device_loc, res => GetDeviceLocationDetails(questionAnswerResponse.query, res) },
                    { MetaData.is_all_asset_loc, res => GetAllAssetsLocationDetails(res) },
                    { MetaData.is_count_user, res => GetWorkstationUserDetails(questionAnswerResponse.query, res, questionAnswerResponse.metaData) },
                    { MetaData.subject, res => GetWarrantyDetails(res) },
                    { MetaData.is_last_contact_ws, res => GetWorkStationDetails(questionAnswerResponse.metaData, res, questionAnswerResponse.query) },
                    { MetaData.is_most_dep_ws, res => GetWorkStationDetails(questionAnswerResponse.metaData, res, questionAnswerResponse.query) },
                    { MetaData.is_latestip_ws, res => GetWorkStationDetails(questionAnswerResponse.metaData, res, questionAnswerResponse.query) },
                    { MetaData.is_software_up_date_ws, res => GetWorkStationDetails(questionAnswerResponse.metaData, res, questionAnswerResponse.query) },
                    { MetaData.is_unoccupied_ws, res => GetWorkStationDetails(questionAnswerResponse.metaData, res, questionAnswerResponse.query) },
                    { MetaData.btry_bng_usd, res => GetBattaryUsedDetails(res) },
                    { MetaData.is_expire, res => GetWorkstationWarranties(res, questionAnswerResponse.query, questionAnswerResponse.metaData) },
                    { MetaData.is_online_offline, res => GetAssetReporting(res, questionAnswerResponse.metaData) },
                    { MetaData.frgt_pass, res => AccountForgotPassword() },
                    { MetaData.envy_ws_dtl, res => GetEnvoyWorkstationDetails(res) },
                    { MetaData.workstation_online, res => GetWorkstationHistory(res, questionAnswerResponse.metaData) },
                    { MetaData.chrg_dtl, res => GetChargerDetails(res) },
                    { MetaData.btry_hlth, res => GetBattaryHealth(res) },
                    { MetaData.decommissioned_dtl, res => GetAssetDetails(res) },
                    { MetaData.hgst_usg, res => GetHighestUsage(res) },
                    { MetaData.old_ast, res => GetOldestAsset(res) },
                    { MetaData.btry_chrg_dtl, res => GetBattaryChargeDetails(res, questionAnswerResponse.query) },
                    { MetaData.busist_day_week, res => GetBusiestDayWeek(res) },
                    { MetaData.ws_log, res => GetWorkstationlog(res,questionAnswerResponse.query) },
                    { MetaData.ws_pwr_off, res => GetWorkstationPower(res,questionAnswerResponse.query) },
                };
        }


        #endregion

        #region WarrantyDetails
        /// <summary>
        /// Generates an HTML string listing batteries that need replacement based on their warranty status and end date.
        /// Filters out batteries that are expired or have a warranty end date in the past.
        /// Checks if there are any batteries with a warranty end date within the next 90 days.
        /// </summary>
        /// <param name="data">A JArray containing warranty details of batteries to process.</param>
        /// <returns>
        /// An HTML string listing the batteries that need replacement within the next 90 days, or a message indicating that no batteries require replacement.
        /// If no active batteries are found, a default message about expired batteries is returned.
        /// </returns
        private static string GetWarrantyDetails(JArray data)
        {
            // Convert JArray to a list of WarrantyDetails objects
            var warrantyDetails = data.ToObject<List<WarrantyDetails>>();

            // Return a default message if the data is null or empty
            if (warrantyDetails == null || !warrantyDetails.Any())
            {
                return GenerateHtmlMessage(
                    "All batteries listed have a `WarrantyStatus` of 'Expired' and their `WarrantyEndDate` is in the past. " +
                    "There are no batteries with a `WarrantyEndDate` within the next 90 days.");
            }

            // Calculate the date range: today and 90 days from now
            DateTime today = DateTime.Now;
            DateTime ninetyDaysFromNow = today.AddDays(90);

            // Filter batteries needing replacement within the next 90 days
            var batteriesToReplace = warrantyDetails
                .Where(item => item.WarrantyStatus == "Active" &&
                               !string.IsNullOrEmpty(item.Serial) &&
                               DateTime.TryParseExact(item.WarrantyEndDate.ToString(),
                                                      "M-d-yy",
                                                      CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None,
                                                      out DateTime parsedDate) &&
                               today <= parsedDate && parsedDate <= ninetyDaysFromNow)
                .Select(item => item.Serial)
                .ToList();

            // Generate the appropriate HTML message
            if (batteriesToReplace.Any())
            {
                return GenerateHtmlMessage(
                    "Based on the provided data, the following batteries need replacement in the next 90 days:",
                    batteriesToReplace);
            }

            return GenerateHtmlMessage(
                "All batteries listed have a `WarrantyStatus` of 'Expired' and their `WarrantyEndDate` is in the past. " +
                "There are no batteries with a `WarrantyEndDate` within the next 90 days.");
        }

        // Helper method to generate an HTML message
        private static string GenerateHtmlMessage(string message, List<string> items = null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<ul>");
            stringBuilder.AppendFormat("<li>{0}</li>", message);

            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    stringBuilder.AppendFormat("<li>{0}</li>", item);
                }
            }

            stringBuilder.Append("</ul>");
            return stringBuilder.ToString();
        }
        #endregion

        #region Get Location of workstations, assets
        /// <summary>
        /// Generates an HTML string listing all asset locations from the given data.
        /// Filters out entries with asset numbers "NOT ASSIGNED" or "N/A" and excludes entries with empty or "N/A" locations.
        /// </summary>
        /// <param name="data">A JArray containing asset tracking data to process.</param>
        /// <returns>
        /// An HTML string representing a list of assets and their locations. Returns an empty string if no valid data is provided.
        /// </returns>
        private static string GetAllAssetsLocationDetails(JArray data)
        {
            StringBuilder responseString = new();

            if (data != null && data.Any())
            {
                // Convert JArray to a list of DashAssetTracking objects
                var dashAssetTrackings = data.ToObject<List<DashAssetTracking>>();
                responseString.Append("Kindly check given below details of all Assets and Current Location : <ul>");
                foreach (var entry in dashAssetTrackings)
                {
                    // Exclude entries with "NOT ASSIGNED" or "N/A" asset numbers
                    if (!sourceArray.Contains(entry.AssetNumber, StringComparer.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(entry.Location) && !entry.Location.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                    {
                        responseString.AppendFormat("<li>Asset: {0}, Location: {1}</li>", entry.AssetNumber, entry.Location);
                    }
                }
                responseString.Append("</ul>");
            }
            return responseString.ToString();
        }

        /// <summary>
        /// Analyzes asset location data and generates a detailed HTML response.
        /// The response provides information about the most frequently used assets in each floor and wing.
        /// </summary>
        /// <param name="data">A JArray containing asset tracking data to process.</param>
        /// <returns>
        /// An HTML string summarizing the most used assets by floor and wing.
        /// If the data is null or empty, an empty string is returned.
        /// </returns>
        public static string GetAssetLocationDetails(JArray data)
        {
            StringBuilder responseString = new();

            if (data != null && data.Any())
            {
                // Convert JArray to a list of DashAssetTracking objects
                var dashAssetTrackings = data.ToObject<List<DashAssetTracking>>();

                // Dictionary to hold asset counts for each floor and wing
                var floorWingAssetCounter = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

                // Count asset occurrences by floor and wing
                foreach (var entry in dashAssetTrackings)
                {
                    string floor = entry.Floor;
                    string wing = entry.Wing;
                    string asset = entry.AssetNumber;

                    // Exclude entries with "NOT ASSIGNED" or "N/A" asset numbers
                    if (!sourceArray.Contains(asset, StringComparer.OrdinalIgnoreCase))
                    {
                        // Initialize floor in the dictionary if not present
                        if (!floor.Equals(Unassigned, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!floorWingAssetCounter.ContainsKey(floor))
                            {
                                floorWingAssetCounter[floor] = [];
                            }

                            // Initialize wing in the floor dictionary if not present
                            if (!wing.Equals(Unassigned, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!floorWingAssetCounter[floor].ContainsKey(wing))
                                {
                                    floorWingAssetCounter[floor][wing] = [];
                                }

                                // Increment the asset count for the floor and wing
                                if (!floorWingAssetCounter[floor][wing].ContainsKey(asset))
                                {
                                    floorWingAssetCounter[floor][wing][asset] = 0;
                                }
                                floorWingAssetCounter[floor][wing][asset]++;
                            }
                        }
                    }
                }

                // Generate the response string for most used assets
                foreach (var (floor, wings) in floorWingAssetCounter)
                {
                    responseString.Append($"Floor {floor}:<br>");
                    foreach (var (wing, assetCounter) in wings)
                    {
                        if (assetCounter.Any())
                        {
                            // Find the most used asset for the wing
                            var mostUsedAsset = assetCounter.OrderByDescending(x => x.Value).FirstOrDefault();
                            responseString.Append($"<b>Wing {wing}:</b> The most used asset is <b>'{mostUsedAsset.Key}'</b> with <b>{mostUsedAsset.Value}</b> occurrences.<br>");
                        }
                        else
                        {
                            responseString.Append($"<b>Wing {wing}:</b> No assigned assets found.<br>");
                        }
                    }
                }
            }

            return responseString.ToString();
        }

        /// <summary>
        /// Retrieves and formats the location details for a specific device based on its serial number.
        /// Extracts the serial number from the query and matches it against the provided data.
        /// </summary>
        /// <param name="query">The query string containing the device's serial number.</param>
        /// <param name="jArray">A JArray containing asset tracking data to process.</param>
        /// <returns>
        /// A formatted HTML string providing the device's location details, including floor, wing, department, and specific location if available. 
        /// Returns an empty string if no matching device is found.
        /// </returns>
        public static string GetDeviceLocationDetails(string query, JArray jArray)
        {
            StringBuilder responseString = new();

            // Extract SerialNo from the query
            var serialNo = Helper.ExtractNo(query);

            // Convert JArray to a list of DashAssetTracking objects
            var dashAssetTrackings = jArray.ToObject<List<DashAssetTracking>>();

            if (dashAssetTrackings != null && dashAssetTrackings.Any())
            {
                // Filter the list to find items matching the extracted serial number
                var filteredItems = dashAssetTrackings.Where(p => p.SerialNo == serialNo).ToList();

                // If matching items are found
                if (filteredItems.Any())
                {
                    var finalData = filteredItems.FirstOrDefault();

                    // Build the response string with the location details
                    responseString.Append($"The device <b>{serialNo}</b> is located on Floor <b>{finalData?.Floor}</b>, ");
                    responseString.Append($"<b>{finalData?.Wing}</b> Wing, in the <b>{finalData?.Department}</b> Department.");

                    // Add location if it is not "N/A"
                    if (!string.IsNullOrWhiteSpace(finalData?.Location) && !finalData.Location.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                    {
                        responseString.Append($" Location <b>{finalData.Location}</b>.");
                    }
                }

            }
            return responseString.ToString();
        }

        /// <summary>
        /// Retrieves and formats the location details for a specific workstation based on its serial number.
        /// Extracts the serial number from the query and searches the provided data for a matching asset.
        /// </summary>
        /// <param name="query">The query string containing the workstation's serial number.</param>
        /// <param name="jArray">A JArray containing asset tracking data to process.</param>
        /// <returns>
        /// A formatted HTML string providing the workstation's location details if found, or a message indicating the device is not located.
        /// </returns>
        private static string GetWorkstationLocationDetails(string query, JArray jArray)
        {
            // Extract SerialNo from the query
            var serialNo = Helper.ExtractNo(query);

            // Convert JArray to a list of DashAssetTracking objects
            var dashAssetTrackings = jArray.ToObject<List<DashAssetTracking>>();

            // Find the asset with matching SerialNo
            var location = dashAssetTrackings?.FirstOrDefault(p => p.SerialNo == serialNo);

            // Build location details string
            return location != null
                ? BuildLocationString(location)
                : $"The device <b>{serialNo}</b> is not located.";
        }

        /// <summary>
        /// Constructs a formatted HTML string detailing the location of a device.
        /// Includes information about the floor, wing, department, and specific location if available.
        /// </summary>
        /// <param name="location">An instance of the <see cref="DashAssetTracking"/> class containing the location details of the device.</param>
        /// <returns>
        /// A formatted HTML string describing the device's location, including specific location details if provided and valid.
        /// </returns>
        private static string BuildLocationString(DashAssetTracking location)
        {
            StringBuilder locationDetails = new();

            locationDetails.Append($"Workstation <b>{location.SerialNo}</b> is located in <b>{location.Location}</b>, <b>{location.Department}</b>, on <b>{location.Floor}</b>, <b>{location.Wing}</b>");

            return locationDetails.ToString();
        }
        #endregion

        #region Get User Workstation details
        /// <summary>
        /// Processes the provided query and JArray to retrieve and format user details associated with a specific workstation.
        /// The method extracts the workstation serial number from the query, filters the data to find users linked to that workstation ID,
        /// and formats the response into an HTML string.
        /// </summary>
        /// <param name="query">The query containing the workstation serial number.</param>
        /// <param name="data">The JArray containing workstation user details.</param>
        /// <param name="metaDataValue">The metaDataValue containing meta value details.</param>
        /// <returns>A string containing an HTML formatted list of users or a message about the workstation.</returns>
        private static string GetWorkstationUserDetails(string query, JArray data, Dictionary<string, string> metaDataValue)
        {
            var responseString = new StringBuilder();

            if (data == null || !data.Any())
            {
                responseString.Append("No data available.");
            }

            string serialNo = Helper.ExtractNo(query);
            if (string.IsNullOrEmpty(serialNo))
            {
                responseString.Append("Kindly provide a workstation id.");
            }

            var dashAssetTrackings = data.ToObject<List<WorkstationUserDetails>>();
            var filteredResult = dashAssetTrackings?.Where(item => item.WorkstationUserID == Convert.ToInt64(serialNo)).ToList();

            if (filteredResult == null || !filteredResult.Any())
            {
                return "We have no data regarding the provided workstation.";
            }

            metaDataValue.TryGetValue("is_count_user", out var isCountUser);

            var resultToDisplay = Convert.ToBoolean(isCountUser)
                ? filteredResult.OrderByDescending(p => p.UniqueID).Take(10)
                : filteredResult;

            responseString.Append("The following user(s) have previously used this workstation:<ul>");

            foreach (var item in resultToDisplay)
            {
                string username = string.IsNullOrEmpty(item.Username) ? "(empty username)" : item.Username;
                responseString.AppendFormat("<li>{0}</li>", username);
            }

            responseString.Append("</ul>");
            return responseString.ToString();
        }
        #endregion

        #region Get Workstation details
        /// <summary>
        /// Retrieves and formats workstation details based on provided metadata and query parameters.
        /// </summary>
        /// <param name="metaData">A dictionary containing meta-data that determines which details to return.</param>
        /// <param name="data">A JArray containing workstation details.</param>
        /// <param name="query">The query string used to filter specific workstation details.</param>
        /// <returns>A formatted HTML string containing workstation details.</returns>
        private static string GetWorkStationDetails(Dictionary<string, string> metaData, JArray data, string query)
        {
            var response = new StringBuilder();

            var workstationDetails = data.ToObject<List<WorkstationDetails>>();

            // Try to retrieve meta-data flags
            metaData.TryGetValue(MetaData.is_last_contact_ws, out var isLastContactWs);
            metaData.TryGetValue(MetaData.is_software_up_date_ws, out var isSoftwareUpDateWs);
            metaData.TryGetValue(MetaData.is_most_dep_ws, out var isMostDepWs);
            metaData.TryGetValue(MetaData.is_latestip_ws, out var isLatestIpWs);

            // Check for 'is_last_contact_ws' flag and process the last contact information
            if (bool.TryParse(isLastContactWs, out var isLastContact) && isLastContact)
            {
                ProcessLastContact(workstationDetails, response);
            }
            // Check for 'is_software_up_date_ws' flag and process the software update information
            else if (bool.TryParse(isSoftwareUpDateWs, out var isSoftwareUpDate) && isSoftwareUpDate)
            {
                ProcessSoftwareUpdate(workstationDetails, query, response);
            }
            // Check for 'is_most_dep_ws' flag and process department-wise workstation count
            else if (bool.TryParse(isMostDepWs, out var isMostDep) && isMostDep)
            {
                ProcessMostDep(workstationDetails, response);
            }
            // Check for 'is_latestip_ws' flag and process the latest IP and MAC address
            else if (bool.TryParse(isLatestIpWs, out var isLatestIp) && isLatestIp)
            {
                ProcessLatestIp(workstationDetails, query, response);
            }
            else
            {
                ProcessUnoccupiedWorkstations(workstationDetails, response);
            }

            return response.ToString();
        }

        // Helper method to process last contact details
        private static void ProcessLastContact(List<WorkstationDetails> workstationDetails, StringBuilder response)
        {
            var latestContact = workstationDetails
                .Where(item => !string.IsNullOrEmpty(Convert.ToString(item.LastPostDateUTC)))
                .Select(item => DateTime.Parse(Convert.ToString(item.LastPostDateUTC)))
                .OrderByDescending(date => date)
                .FirstOrDefault();

            if (latestContact != DateTime.MinValue)
            {
                string formattedDate = latestContact.ToString("hh:mm tt 'on' MMMM dd, yyyy");
                response.Append($"The most recent contact with a workstation was at <b>{formattedDate}</b>.");
            }
            else
            {
                response.Append("Based on the provided data, there is no workstation with recent contact.");
            }
        }

        // Helper method to process software update information
        private static void ProcessSoftwareUpdate(List<WorkstationDetails> workstationDetails, string query, StringBuilder response)
        {
            var workstationName = Helper.ExtractNo(query);
            var workstation = workstationDetails.FirstOrDefault(p => p.Workstation == workstationName);

            if (workstation != null)
            {
                response.Append($"The provided data does not contain information about software updates for any workstations. Therefore, I cannot determine if workstation <b>{workstationName}</b>'s software is up to date.");
            }
            else
            {
                response.Append("The provided data does not contain information about software updates for any workstations.");
            }
        }

        // Helper method to process the most department-wise workstation counts
        private static void ProcessMostDep(List<WorkstationDetails> workstationDetails, StringBuilder response)
        {
            var departmentCount = workstationDetails
                .GroupBy(item => item.DepartmentAssigned ?? Unassigned)
                .ToDictionary(g => g.Key, g => g.Count());

            if (departmentCount.Any())
            {
                var formattedOutput = new StringBuilder("<ul>");
                foreach (var (department, count) in departmentCount)
                {
                    if (!department.Equals(Unassigned, StringComparison.OrdinalIgnoreCase))
                    {
                        formattedOutput.Append($"<li>{department}: {count} workstation{(count > 1 ? "s" : string.Empty)}</li>");
                    }
                }
                formattedOutput.Append("</ul>");

                response.Append($"Based on the provided data, the departments with the most workstations are:\n\n{formattedOutput}\n\nNote that many workstations are assigned to 'Unassigned' department.");
            }
            else
            {
                response.Append("Based on the provided data, there are no departments with workstation data.");
            }
        }

        // Helper method to process latest IP and MAC address information
        private static void ProcessLatestIp(List<WorkstationDetails> workstationDetails, string query, StringBuilder response)
        {
            var workstationName = Helper.ExtractNo(query);
            var workstation = workstationDetails.FirstOrDefault(p => p.Workstation == workstationName);

            if (workstation != null)
            {
                response.Append($"Info for workstation <b>{workstationName}</b>:<br>IP Address: {workstation.IP ?? "N/A"}<br>MAC Address: {workstation.DeviceMAC ?? "N/A"}");
            }
            else
            {
                response.Append($"The latest IP address for workstation <b>{workstationName}</b> is not available in the provided data.");
            }
        }

        // Helper method to process unoccupied workstations
        private static void ProcessUnoccupiedWorkstations(List<WorkstationDetails> workstationDetails, StringBuilder response)
        {
            var unassignedWorkstations = workstationDetails
                .Where(item => item.DepartmentAssigned == Unassigned && item.Location == Unassigned)
                .ToList();

            if (unassignedWorkstations.Any())
            {
                response.Append($"Yes, based on the provided data, there are {unassignedWorkstations.Count} unoccupied workstations.");
                response.Append("<ul>");
                foreach (var item in unassignedWorkstations)
                {
                    response.AppendFormat("<li>{0}</li>", item.Workstation);
                }
                response.Append("</ul>");
            }
            else
            {
                response.Append("No, based on the provided data, there are no unoccupied workstations.");
            }
        }

        public static string GetOrdinalSuffix(int day)
        {
            if (day >= 11 && day <= 13) // Special case for 11th, 12th, 13th
            {
                return "th";
            }

            return (day % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };
        }
        #endregion

        #region Battary Details
        /// <summary>
        /// Retrieves and formats battery details for a specific workstation based on its serial number.
        /// </summary>
        /// <param name="data">A JArray containing battery details for workstations.</param>
        /// <param name="query">The query string containing the workstation's serial number to filter the data.</param>
        /// <returns>A formatted HTML string with battery details for the specified workstation, including the last usage date and charge level.</returns>
        /// need columns for to specify battary type
        private static string GetBattaryUsedDetails(JArray data)
        {

            // Initialize StringBuilder to construct the response
            var stringBuilder = new StringBuilder();

            // Deserialize JArray to a list of battery details
            var batteryDetails = data.ToObject<List<WarrantyDetails>>();

            // Check if battery details are available
            if (batteryDetails.Any())
            {
                // Append general battery usage information using WarrantyDesc column
                var mobiusWarranty = batteryDetails.Where(b => b.WarrantyDesc.Contains("MOBIUS")).Select(b => b.WarrantyDesc).Distinct();
                var powerSonicWarranty = batteryDetails.Where(b => b.WarrantyDesc.Contains("PowerSonic", StringComparison.OrdinalIgnoreCase)).Select(b => b.WarrantyDesc).Distinct();

                stringBuilder.Append("<br>The batteries in use are a mix of MobiusPower and PowerSonic batteries. ");
                stringBuilder.Append($"MobiusPower batteries include models with the following warranties: {string.Join(", ", mobiusWarranty)}. ");
                stringBuilder.Append($"PowerSonic batteries include models with the following warranties: {string.Join(", ", powerSonicWarranty)}.");

                return stringBuilder.ToString();
            }
            else
            {
                // Append a message if no battery details are available
                stringBuilder.Append("No battery details found. Try another prompt.");
            }

            return stringBuilder.ToString();
        }


        /// <summary>
        /// Retrieves and formats battery charge details for a specific workstation based on its serial number.
        /// </summary>
        /// <param name="data">A JArray containing battery details for workstations.</param>
        /// <param name="query">The query string containing the workstation's serial number to filter the data.</param>
        /// <returns>A formatted HTML string with battery details for the specified workstation, including the last usage date and charge level.</returns>
        private static string GetBattaryChargeDetails(JArray data, string query)
        {
            // Initialize StringBuilder to construct the response
            var stringBuilder = new StringBuilder();

            // Deserialize JArray to list of battery details
            var batteryDetails = data.ToObject<List<BatteryChargeDetails>>();

            // Check if any battery details are available
            if (batteryDetails.Any())
            {
                // Extract serial number from the query
                var serialNo = Helper.ExtractNo(query);

                if (!string.IsNullOrEmpty(serialNo))
                {
                    // Get the battery details matching the serial number
                    var batteryDetail = batteryDetails.FirstOrDefault(p => p.Serial == serialNo);

                    if (batteryDetail != null)
                    {
                        // Append the battery usage details if a match is found
                        stringBuilder.Append($"The battery <b>{batteryDetail.Description}</b> has a charge level of <b>{batteryDetail.ChargeLevel}</b> and was last used on <b>{batteryDetail.LastUsed}</b>");
                    }
                    else
                    {
                        // Append a message if no match is found
                        stringBuilder.Append($"The workstation <b>{serialNo}</b> was not last used.");
                    }
                }
                else
                {
                    // Append a message if the serial number is invalid or empty
                    stringBuilder.Append("The workstation serial number is invalid or not provided.");
                }
            }
            else
            {
                // Append a message if no battery details are available
                stringBuilder.Append("No battery details found. Try another prompt.");
            }

            return stringBuilder.ToString();
        }
        #endregion

        #region Workstation Warranties
        /// <summary>
        /// Retrieves and formats warranty information for workstations based on metadata and query input.
        /// </summary>
        /// <param name="res">A JArray containing warranty details for workstations.</param>
        /// <param name="query">A query string containing the workstation's serial number to filter data.</param>
        /// <param name="metaDataValue">A dictionary containing metadata values for additional filters (e.g., whether to check for expired warranties).</param>
        /// <returns>A formatted HTML string with warranty details for the specified workstation, including expiration status and closest expiration dates if applicable.</returns>
        private static string GetWorkstationWarranties(JArray res, string query, Dictionary<string, string> metaDataValue)
        {
            // Initialize StringBuilder to construct the response
            var stringBuilder = new StringBuilder();

            // Retrieve the "is_expire" metadata value (if present)
            metaDataValue.TryGetValue(MetaData.is_expire, out var isExpireValue);

            // Extract the serial number from the query string
            var serialNo = Helper.ExtractNo(query);

            // Deserialize the JArray to a list of warranty details
            var warrantyDetails = res.ToObject<List<WarrantyDetails>>();

            // If no warranty details are found, return a message stating that the workstation does not exist
            if (!warrantyDetails.Any())
            {
                stringBuilder.Append($"Based on the provided data, the {serialNo} workstation does not exist in the system.");
                return stringBuilder.ToString();
            }

            // If isExpire is true, filter and display warranties that are expired or about to expire
            if (bool.TryParse(isExpireValue, out var isExpire) && isExpire)
            {
                DateTime today = DateTime.Now;

                // Filter warranty details based on expiration date
                var expiredItems = warrantyDetails
                    .Where(item => item.WarrantyEndDate != "N/A" && DateTime.TryParseExact(item.WarrantyEndDate, "MM-dd-yy", null, DateTimeStyles.None, out DateTime warrantyEndDate)
                        && (today > warrantyEndDate || (today.Year == warrantyEndDate.Year && today.Month == warrantyEndDate.Month)))
                    .OrderBy(item => DateTime.ParseExact(item.WarrantyEndDate, "MM-dd-yy", null))
                    .ToList();

                if (expiredItems.Any())
                {
                    // Append the details of expired warranties
                    stringBuilder.Append("Based on the provided data, the closest assets to expire out of warranty are:");
                    stringBuilder.Append("<ul>");
                    foreach (var item in expiredItems)
                    {
                        stringBuilder.AppendFormat("<li>Serial <b>{0}</b> - <b>({1})</b> : Warranty expires <b>{2}</b>{3}</li>",
                            item.Serial,
                            item.Description,
                            item.WarrantyEndDate,
                            today > DateTime.ParseExact(item.WarrantyEndDate, "MM-dd-yy", null) ? " - This warranty is already expired." : string.Empty);
                    }
                    stringBuilder.Append("</ul>");
                    stringBuilder.Append($"Many assets have warranties expiring in <b>{today.ToString("MMMM yyyy")}</b>. Please note that some assets show expired warranties in the data. Contact technical services for clarification on warranty information marked as \"N/A\".");
                }
            }
            else
            {
                // If not checking for expiration, return warranty details for the specified serial number
                var filteredItem = warrantyDetails.FirstOrDefault(item => item.Serial == serialNo);

                if (filteredItem != null)
                {
                    stringBuilder.Append($"Kindly check the given details of the asset's warranty: <ul> " +
                                          $"<li>SerialNo: <b>{filteredItem.Serial}</b></li> " +
                                          $"<li>Warranty: <b>{filteredItem.WarrantyDesc}</b></li> " +
                                          $"<li>WarrantyEndDate: <b>{filteredItem.WarrantyEndDate}</b></li></ul>");
                }
            }

            return stringBuilder.ToString();
        }
        #endregion

        #region Asset Reporting
        /// <summary>
        /// Generates a response string based on asset reporting details, calculating online/offline assets or providing overall health.
        /// </summary>
        /// <param name="res">A JArray containing asset reporting details.</param>
        /// <param name="metaData">A dictionary containing metadata that determines the type of reporting (e.g., online/offline status).</param>
        /// <returns>A formatted string with either the online/offline asset count or overall health percentage based on metadata.</returns>
        private static string GetAssetReporting(JArray res, Dictionary<string, string> metaData)
        {
            // Initialize StringBuilder to construct the response
            var responseString = new StringBuilder();

            // Convert JArray to List<AssetsReportingDetails>
            var assetReporting = res.ToObject<List<AssetsReportingDetails>>();

            // Retrieve the first asset reporting entry (assuming there's at least one entry)
            var asset = assetReporting?.FirstOrDefault();

            // If no asset data is available, return a message indicating no data
            if (asset == null)
            {
                responseString.Append("No asset reporting data available.");
                return responseString.ToString();
            }

            // Conditionally add parameters based on the metadata values
            metaData.TryGetValue(MetaData.is_online_offline, out var isOnlineOffline);

            if (bool.TryParse(isOnlineOffline, out var is_online_offline) && is_online_offline)
            {
                responseString.Append($"<b>{asset.AssetsReporting}</b> assets are online out of a total of <b>{asset.AssetsTotal}</b> assets, with the remaining being offline.");
            }
            else
            {
                responseString.Append($"You have <b>{asset.AssetsTotal} </b> assets of which <b>{asset.AssetsReportingPct}%</b> are currently in use.");
            }
            return responseString.ToString();
        }
        #endregion

        #region Asset Reporting
        /// <summary>
        /// Generates a response string with a link to the forgot password page and instructions for contacting ModCart support.
        /// </summary>
        /// <returns>A formatted HTML string with a "Forgot Password" link and contact instructions.</returns>
        private static string AccountForgotPassword()
        {
            // Initialize StringBuilder for constructing the response
            var responseString = new StringBuilder();

            // Append the instruction message with a hyperlink to the forgot password page
            responseString.Append(
                "Please look for a <a style='text-decoration: underline;' " +
                "href='https://shop.ergotron.com/ccrz__CCForgotPassword?cartID=&portalUser=&store=&cclcl=en_US' " +
                "target='_blank'>Forgot Password</a> link on the ModCart login page or contact ModCart support directly."
            );

            // Return the constructed response string
            return responseString.ToString();
        }
        #endregion

        #region Envoy Workstation
        /// <summary>
        /// Processes a given JArray containing Envoy workstation details and returns a formatted string
        /// listing workstations with Medbin enabled, including their workstation name, description, and lock timeout.
        /// </summary>
        /// <param name="jArray">A JArray containing Envoy workstation details.</param>
        /// <returns>A formatted string containing a list of workstations with Medbin enabled and their corresponding details.</returns>
        private static string GetEnvoyWorkstationDetails(JArray jArray)
        {
            // Initialize StringBuilder for constructing the response
            var responseString = new StringBuilder();

            // Convert JArray to List<EnvoyWorkstationDetails>
            var data = jArray.ToObject<List<EnvoyWorkstationDetails>>();

            // If data is available, proceed with filtering and formatting the output
            if (data != null && data.Any())
            {
                // Filter workstations where MedbinEnabled is 1
                var filteredItems = data.Where(item => item.MedbinEnabled == 1).ToList();

                if (filteredItems.Any())
                {
                    // Start the unordered list
                    responseString.Append("<ul>");

                    // Iterate over the filtered workstations and append details to the response
                    foreach (var item in filteredItems)
                    {
                        // Provide default values if any property is null or empty
                        string workstation = string.IsNullOrEmpty(item.Workstation) ? "N/A" : item.Workstation;
                        string description = string.IsNullOrEmpty(item.Description) ? "N/A" : item.Description;
                        string? lockTimeout = item.MedbinLockTimeout > 0 ? item.MedbinLockTimeout.ToString() : "N/A";

                        // Append formatted details to the response string
                        responseString.AppendFormat(
                            "<li><b>{0} ({1})</b> : Med Bin Enabled = YES, Lock Timeout = <b>{2}</b></li>",
                            workstation,
                            description,
                            lockTimeout
                        );
                    }

                    // End the unordered list
                    responseString.Append("</ul>");
                }
                else
                {
                    // If no workstations with Med Bin enabled, provide a message
                    responseString.Append("There is no workstation with Med Bin enabled.");
                }
            }
            else
            {
                // If no data available, return a message indicating no data
                responseString.Append("No workstation details available.");
            }

            // Return the constructed response string
            return responseString.ToString();
        }
        #endregion

        #region Workstation History
        /// <summary>
        /// Processes a given JArray containing workstation history details and a metadata dictionary,
        /// and returns a formatted string with the number of online or available workstations based on the metadata.
        /// </summary>
        /// <param name="res">A JArray containing workstation history details.</param>
        /// <param name="metaData">A dictionary containing metadata that influences the output (e.g., workstation online status).</param>
        /// <returns>A formatted string indicating the number of workstations based on their online status or total availability.</returns>
        private static string GetWorkstationHistory(JArray res, Dictionary<string, string> metaData)
        {
            // Early return if no data is available
            if (res == null || !res.Any()) return "No workstation history available.";

            // Convert JArray to List<WorkstationHistory>
            var data = res.ToObject<List<WorkstationHistory>>();

            // Get the first workstation history entry
            var workStationHistory = data?.FirstOrDefault();

            // If no workstation history is available, return an appropriate message
            if (workStationHistory == null) return "No workstation history found.";

            // Initialize StringBuilder for the response string
            var responseString = new StringBuilder();

            // Check the metadata for the 'workstation_online' key to determine the status
            metaData.TryGetValue(MetaData.workstation_online, out var workstationOnline);

            // Conditionally add parameters based on the metadata values
            if (bool.TryParse(workstationOnline, out var workstationOnline_false) && !workstationOnline_false)
            {
                responseString.Append($"We have <b>{workStationHistory.TotalAvailable}</b> workstations.");
            }
            else
            {
                responseString.Append($"Currently, <b>{workStationHistory.NumOnline}</b> workstations are online.");
            }

            return responseString.ToString();
        }

        #endregion

        #region Charger Details
        /// <summary>
        /// Processes a given JArray containing charger details and returns a formatted string with the count of chargers.
        /// The number of chargers is extracted from the provided data and displayed in a simple HTML format.
        /// </summary>
        /// <param name="res">A JArray containing charger details.</param>
        /// <returns>A formatted string containing the count of chargers in HTML format.</returns>
        private static string GetChargerDetails(JArray res)
        {
            // If no data is available, return a message indicating no data
            if (res == null || !res.Any()) return "No data available.";

            // Convert JArray to List<WorkstationHistory>
            var data = res.ToObject<List<WorkstationHistory>>();

            // Return formatted string with the count of chargers
            return $"We have <b>{data?.Count}</b> Chargers.";
        }
        #endregion

        #region Get Battary Health
        /// <summary>
        /// Processes a given JArray containing battery health details and returns a formatted string with the count of batteries.
        /// The number of batteries is extracted from the provided data and displayed in a simple HTML format.
        /// </summary>
        /// <param name="res">A JArray containing battery health details.</param>
        /// <returns>A formatted string containing the count of batteries in HTML format.</returns>
        private static string GetBattaryHealth(JArray res)
        {
            // If no data is available, return a message indicating no data
            if (res == null || !res.Any()) return "No data available.";

            // Convert JArray to List<BatteryHealthDetails>
            var data = res.ToObject<List<BatteryHealthDetails>>();

            // Return formatted string with the count of batteries
            return $"We have <b>{data?.Count}</b> batteries.";
        }
        #endregion

        #region Get Asset Details
        /// <summary>
        /// Processes a given JArray containing asset details and returns a formatted string listing valid assets.
        /// The assets are filtered based on their `CartSerial` containing digits and a valid `SerialNo`. 
        /// If valid assets are found, their serial numbers and IP addresses are included in the returned HTML list.
        /// </summary>
        /// <param name="res">A JArray containing asset details.</param>
        /// <returns>A formatted string containing a list of valid assets or a message indicating no data was found.</returns>
        private static string GetAssetDetails(JArray res)
        {
            // Return early if there is no data
            if (res == null || !res.Any()) return "No data available.";

            // Convert JArray to List<AssetDetails>
            var data = res.ToObject<List<AssetDetails>>();

            // Initialize StringBuilder for building response
            var responseString = new StringBuilder();
            responseString.Append("Here is a list of decommissioned carts ready to be removed from Rhythm:");

            // Filter and construct list of valid items
            var filteredItems = data
                .Where(item => string.IsNullOrEmpty(item.IP))
                .Select(item => $"<li>Serial Number: {item.SerialNo}</li>")
                .ToList();

            // If there are valid items, create the unordered list
            if (filteredItems.Any())
            {
                responseString.Append("<ul>");
                responseString.Append(string.Join("", filteredItems));  // Join the filtered items into a single string
                responseString.Append("</ul>");
            }

            return responseString.ToString();
        }
        #endregion

        #region Get Highest Usage
        /// <summary>
        /// Retrieves the highest usage value from the provided JArray.
        /// If the usage data is available and the 'highestUsed' value is greater than 0, it returns the peak usage formatted in HTML.
        /// Otherwise, it returns an appropriate message based on the data content.
        /// </summary>
        /// <param name="jArray">A JArray containing the highest usage details.</param>
        /// <returns>A formatted string containing the peak usage value or a message indicating missing data.</returns>
        private static string GetHighestUsage(JArray jArray)
        {
            // Initialize StringBuilder to build the response string
            var stringBuilder = new StringBuilder();

            // Convert JArray to a list of HighestUsageDetails objects
            var usageDetails = jArray.ToObject<List<HighestUsageDetails>>();

            // Check if there is any data available
            if (usageDetails?.Any() == true)
            {
                // Access the first item in the list (assuming there's only one relevant item)
                var finalData = usageDetails.FirstOrDefault();

                // Check if 'highestUsed' value is greater than 0
                if (finalData?.highestUsed > 0)
                {
                    stringBuilder.Append($"Your peak usage is <b>{finalData.highestUsed}</b>.");
                }
                else
                {
                    stringBuilder.Append("No valid 'highestUsed' value found in the data.");
                }
            }
            else
            {
                stringBuilder.Append("No answer found. Try another prompt.");
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region Get Oldest Asset
        /// <summary>
        /// Retrieves and formats information about the oldest asset still in production from a given JSON array of asset data.
        /// The method identifies the first asset in the list and extracts its serial number and manufacturing date, 
        /// which is then formatted into an HTML string.
        /// </summary>
        /// <param name="jArray">A JArray containing asset details to process.</param>
        /// <returns>
        /// A string containing HTML formatted information about the oldest asset still in production, 
        /// or a message stating that no data was found if the input is empty or null.
        /// </returns>
        public static string GetOldestAsset(JArray jArray)
        {
            StringBuilder responseString = new();

            var data = jArray.ToObject<List<OldAssetDetails>>();

            if (data != null && data.Count > 0)
            {
                var finalData = data.FirstOrDefault();

                if (finalData != null)
                {
                    responseString.Append($"The oldest asset still in production is <b>{finalData.Description}</b> serial <b>{finalData.SerialNo}</b> last posted date was <b>{finalData.LastPostDateUTC}</b>");
                }
                else
                {
                    responseString.Append("No answer found. Try another prompt.");
                }
            }
            else
            {
                responseString.Append("No answer found. Try another prompt.");
            }
            return responseString.ToString();
        }
        #endregion

        #region Get Busiest DayWeek
        /// <summary>
        /// Generates a summary of the busiest day of the week based on the provided data.
        /// </summary>
        /// <param name="jArray">A JSON array containing data about the busiest day of the week.</param>
        /// <returns>
        /// A formatted string describing the busiest day, peak carts, average carts, and maximum carts.
        /// If no data is available, returns a message indicating no answer is found.
        /// </returns>
        public static string GetBusiestDayWeek(JArray jArray)
        {
            StringBuilder responseString = new();

            // Convert JSON array to a list of BusiestDayWeekDetails
            var data = jArray.ToObject<List<BusiestDayWeekDetails>>();

            if (data != null && data.Count > 0)
            {
                var finalData = data.FirstOrDefault();

                if (finalData != null)
                {
                    responseString.Append($"<b>{finalData.PeakDayName}</b> is busiest day of the week.");
                }
                else
                {
                    responseString.Append("No answer found. Try another prompt.");
                }
            }
            else
            {
                responseString.Append("No answer found. Try another prompt.");
            }
            return responseString.ToString();
        }
        #endregion

        #region Get Workstation log
        /// <summary>
        /// Retrieves a list of workstation logs that match the given serial number and time extracted from the query string.
        /// The method parses the provided query to extract the serial number and time, searches through the provided log data,
        /// and returns an HTML-formatted string containing the details of the matching logs, or a message if no matches are found.
        /// </summary>
        /// <param name="jArray">A JSON array containing a list of workstation log entries.</param>
        /// <param name="query">A string query containing a serial number and a time (in MM/dd/yyyy HH:mm:ss format) to search for in the logs.</param>
        /// <returns>
        /// An HTML-formatted string with a list of logs that match the given serial number and time.
        /// If no matching logs are found, a message indicating no data is available is returned.
        /// </returns>
        public static string GetWorkstationlog(JArray jArray, string query)
        {
            // Initialize response builder and result list
            var responseString = new StringBuilder();

            // Extract serial number and time from the query
            var serialNo = Helper.ExtractNo(query);

            // Extract serial number and time from the query
            var date = Helper.ExtractDate(query);

            // Convert the JSON array to a list of workstation logs
            var data = jArray.ToObject<List<WorkstationLog>>();
            if (data == null || data.Count == 0)
            {
                responseString.Append("No data available to process the query.");
            }

            // Filter logs that match the serial number and time
            var result = data?.Where(log => log.SerialNo == serialNo && Convert.ToDateTime(log.LocalTime).Date == Convert.ToDateTime(date).Date)
                        .FirstOrDefault();


            // Construct response string
            if (result != null)
            {
                responseString.Append($"The MedBins of <b>{serialNo}</b> were accessed on <b>{date}</b> by <b>{result.FirstName}</b> <b>{result.LastName}</b> at <b>{result.LocalTime}</b>. The following drawers were accessed: <b>{result.Drawer}</b>");
            }
            else
            {
                responseString.Append("No battery details found. Try another prompt.");
            }

            return responseString.ToString();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private static string GetWorkstationPower(JArray res, string query)
        {
            // Initialize response builder and result list
            var responseString = new StringBuilder();

            // Extract serial number and time from the query
            var serialNo = Helper.ExtractNo(query);
            if (string.IsNullOrEmpty(serialNo))
            {
                responseString.Append("Kindly provide a workstation id.");
            }

            // Convert the JSON array to a list of workstation logs
            var data = res.ToObject<List<DashAssetTracking>>();
            if (data == null || data.Count == 0)
            {
                responseString.Append("No data available to process the query.");
            }

            var result = data?.FirstOrDefault(p => p.SerialNo == serialNo);

            if (result != null)
            {
                responseString.AppendFormat("The last known location of workstation <b>{0}</b> was &lt;<b>{1}</b>, <b>{2}</b>, <b>{3}</b>, <b>{4}</b>&gt; LastReportDate of <b>{5}</b>", serialNo, result.Department, result.Floor, result.Wing, result.Location, result.LastReported);
            }
            else
            {
                responseString.AppendFormat("The last known location of workstation {0} is not available", serialNo);
            }

            return responseString.ToString();
        }
    }
}
