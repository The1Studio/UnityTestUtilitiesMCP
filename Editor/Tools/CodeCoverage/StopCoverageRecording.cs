using System;
using System.Threading.Tasks;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage
{
    /// <summary>
    /// MCP tool for stopping code coverage recording.
    /// Optionally generates an HTML coverage report after stopping.
    /// </summary>
    [McpForUnityTool("stop_coverage_recording")]
    public static class StopCoverageRecording
    {
        private static readonly ICoverageService _coverageService = new CoverageService();

        /// <summary>
        /// Handles the stop_coverage_recording command.
        /// Stops recording code coverage data and optionally generates a report.
        /// </summary>
        /// <param name="params">
        /// JSON parameters:
        /// - generateReport (bool, optional): If true, generate HTML report after stopping. Default: false
        /// </param>
        /// <returns>Success response with recording status and optional report path, or error response if operation fails.</returns>
        public static async Task<object> HandleCommand(JObject @params)
        {
            try
            {
                // Parse 'generateReport' parameter (optional, default: false)
                bool generateReport = false;
                var generateReportToken = @params["generateReport"];
                if (generateReportToken != null)
                {
                    if (generateReportToken.Type != JTokenType.Boolean)
                    {
                        return Response.Error($"Parameter 'generateReport' must be a boolean, got: {generateReportToken.Type}");
                    }
                    generateReport = generateReportToken.Value<bool>();
                }

                // Stop recording
                _coverageService.StopRecording();

                string coverageResultsPath = _coverageService.GetCoverageResultsPath();

                // Generate report if requested
                if (generateReport)
                {
                    try
                    {
                        string reportPath = await _coverageService.GenerateReportAsync();

                        var dataWithReport = new
                        {
                            recording = false,
                            reportGenerated = true,
                            reportPath = reportPath,
                            coverageResultsPath = coverageResultsPath
                        };

                        return Response.Success("Code coverage recording stopped and report generated", dataWithReport);
                    }
                    catch (Exception reportEx)
                    {
                        // Recording stopped successfully but report generation failed
                        var dataWithError = new
                        {
                            recording = false,
                            reportGenerated = false,
                            reportError = reportEx.Message,
                            coverageResultsPath = coverageResultsPath
                        };

                        return Response.Success(
                            "Code coverage recording stopped, but report generation failed",
                            dataWithError);
                    }
                }
                else
                {
                    var data = new
                    {
                        recording = false,
                        reportGenerated = false,
                        coverageResultsPath = coverageResultsPath
                    };

                    return Response.Success("Code coverage recording stopped", data);
                }
            }
            catch (NotSupportedException ex)
            {
                return Response.Error($"Code Coverage not supported: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to stop coverage recording: {ex.Message}");
            }
        }
    }
}
