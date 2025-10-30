using System;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage
{
    /// <summary>
    /// MCP tool for starting code coverage recording.
    /// Coverage must be enabled via <see cref="EnableCodeCoverage"/> before calling this tool.
    /// </summary>
    [McpForUnityTool("start_coverage_recording")]
    public static class StartCoverageRecording
    {
        private static readonly ICoverageService _coverageService = new CoverageService();

        /// <summary>
        /// Handles the start_coverage_recording command.
        /// Starts recording code coverage data. Coverage must be enabled first.
        /// </summary>
        /// <param name="params">No parameters required.</param>
        /// <returns>Success response with recording status, or error response if coverage is not enabled or operation fails.</returns>
        public static object HandleCommand(JObject @params)
        {
            try
            {
                // Check if coverage is enabled
                if (!_coverageService.IsCoverageEnabled())
                {
                    return Response.Error(
                        "Code coverage is not enabled. Call enable_code_coverage with enabled=true first.");
                }

                // Start recording
                _coverageService.StartRecording();

                var data = new
                {
                    recording = true,
                    coverageResultsPath = _coverageService.GetCoverageResultsPath()
                };

                return Response.Success("Code coverage recording started", data);
            }
            catch (InvalidOperationException ex)
            {
                return Response.Error($"Cannot start recording: {ex.Message}");
            }
            catch (NotSupportedException ex)
            {
                return Response.Error($"Code Coverage not supported: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to start coverage recording: {ex.Message}");
            }
        }
    }
}
