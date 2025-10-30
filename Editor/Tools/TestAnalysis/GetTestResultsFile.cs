using System;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Resources;
using Newtonsoft.Json.Linq;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.TestAnalysis
{
    /// <summary>
    /// MCP resource for retrieving Unity test results file location and content.
    /// Locates TestResults.xml in platform-specific Unity directories.
    /// </summary>
    [McpForUnityResource("get_test_results_file")]
    public static class GetTestResultsFile
    {
        private static readonly ITestResultsService _testResultsService = new TestResultsService();

        /// <summary>
        /// Handles the get_test_results_file resource query.
        /// Returns the location and optionally the content of the Unity test results file.
        /// </summary>
        /// <param name="params">
        /// JSON parameters:
        /// - includeContent (bool, optional): If true, includes the full XML content of the test results file. Default: false
        /// </param>
        /// <returns>Success response with file path and optionally content, or error response if file cannot be located.</returns>
        public static object HandleCommand(JObject @params)
        {
            try
            {
                // Parse 'includeContent' parameter (optional, default: false)
                bool includeContent = false;
                var includeContentToken = @params["includeContent"];
                if (includeContentToken != null)
                {
                    if (includeContentToken.Type != JTokenType.Boolean)
                    {
                        return Response.Error($"Parameter 'includeContent' must be a boolean, got: {includeContentToken.Type}");
                    }
                    includeContent = includeContentToken.Value<bool>();
                }

                // Locate test results file
                string filePath = _testResultsService.LocateTestResultsFile();

                if (string.IsNullOrEmpty(filePath))
                {
                    return Response.Error(
                        "Unity test results file not found. Ensure tests have been run via Test Runner (Window → General → Test Runner) " +
                        "and that Company Name and Product Name are set in Project Settings → Player.");
                }

                // Build response data
                var data = new
                {
                    filePath = filePath,
                    fileExists = System.IO.File.Exists(filePath),
                    lastModified = System.IO.File.GetLastWriteTime(filePath).ToString("yyyy-MM-dd HH:mm:ss"),
                    fileSizeBytes = new System.IO.FileInfo(filePath).Length,
                    content = includeContent ? _testResultsService.ReadTestResultsFile() : null
                };

                string message = includeContent
                    ? $"Test results file located and read: {filePath}"
                    : $"Test results file located: {filePath}";

                return Response.Success(message, data);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                return Response.Error($"Test results file not found: {ex.Message}");
            }
            catch (System.IO.IOException ex)
            {
                return Response.Error($"Failed to read test results file: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to get test results file: {ex.Message}");
            }
        }
    }
}
