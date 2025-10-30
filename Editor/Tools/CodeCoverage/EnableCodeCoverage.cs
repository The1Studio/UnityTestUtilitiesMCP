using System;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage
{
    /// <summary>
    /// MCP tool for enabling or disabling Unity Code Coverage.
    /// Allows control over coverage recording and automatic report generation.
    /// </summary>
    [McpForUnityTool("enable_code_coverage")]
    public static class EnableCodeCoverage
    {
        private static readonly ICoverageService _coverageService = new CoverageService();

        /// <summary>
        /// Handles the enable_code_coverage command.
        /// </summary>
        /// <param name="params">
        /// JSON parameters:
        /// - enabled (bool, required): True to enable coverage, false to disable
        /// - autoGenerateReport (bool, optional): If true, automatically generate HTML report after recording stops. Default: false
        /// </param>
        /// <returns>Success response with status, or error response if parameters are invalid or operation fails.</returns>
        public static object HandleCommand(JObject @params)
        {
            try
            {
                // Parse 'enabled' parameter (required)
                var enabledToken = @params["enabled"];
                if (enabledToken == null)
                {
                    return Response.Error("Required parameter 'enabled' (bool) is missing");
                }

                if (enabledToken.Type != JTokenType.Boolean)
                {
                    return Response.Error($"Parameter 'enabled' must be a boolean, got: {enabledToken.Type}");
                }

                bool enabled = enabledToken.Value<bool>();

                // Parse 'autoGenerateReport' parameter (optional, default: false)
                bool autoGenerateReport = false;
                var autoReportToken = @params["autoGenerateReport"];
                if (autoReportToken != null)
                {
                    if (autoReportToken.Type != JTokenType.Boolean)
                    {
                        return Response.Error($"Parameter 'autoGenerateReport' must be a boolean, got: {autoReportToken.Type}");
                    }
                    autoGenerateReport = autoReportToken.Value<bool>();
                }

                // Enable/disable coverage
                _coverageService.EnableCoverage(enabled, autoGenerateReport);

                var data = new
                {
                    enabled = enabled,
                    autoGenerateReport = autoGenerateReport,
                    coverageResultsPath = _coverageService.GetCoverageResultsPath()
                };

                string message = enabled
                    ? $"Code coverage enabled. Auto-generate report: {autoGenerateReport}"
                    : "Code coverage disabled";

                return Response.Success(message, data);
            }
            catch (NotSupportedException ex)
            {
                return Response.Error($"Code Coverage not supported: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to enable/disable code coverage: {ex.Message}");
            }
        }
    }
}
