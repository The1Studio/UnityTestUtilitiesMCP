using System;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Resources;
using Newtonsoft.Json.Linq;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;
using UnityEditor;

#if UNITY_CODE_COVERAGE
using UnityEditor.TestTools.CodeCoverage;
#endif

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage
{
    /// <summary>
    /// MCP resource for querying current code coverage settings.
    /// Provides read-only access to coverage configuration and status.
    /// </summary>
    [McpForUnityResource("get_coverage_settings")]
    public static class GetCoverageSettings
    {
        private static readonly ICoverageService _coverageService = new CoverageService();
        private const string PREF_AUTO_GENERATE_REPORT = "UnityTestUtilitiesMCP.Coverage.AutoGenerateReport";

        /// <summary>
        /// Handles the get_coverage_settings resource query.
        /// Returns current coverage settings and availability status.
        /// </summary>
        /// <param name="params">No parameters required.</param>
        /// <returns>Success response with coverage settings, or error response if query fails.</returns>
        public static object HandleCommand(JObject @params)
        {
            try
            {
                bool isCoveragePackageInstalled = IsCoveragePackageInstalled();
                bool isEnabled = _coverageService.IsCoverageEnabled();
                bool autoGenerateReport = EditorPrefs.GetBool(PREF_AUTO_GENERATE_REPORT, false);
                string coverageResultsPath = _coverageService.GetCoverageResultsPath();

                var data = new
                {
                    coveragePackageInstalled = isCoveragePackageInstalled,
                    enabled = isEnabled,
                    autoGenerateReport = autoGenerateReport,
                    coverageResultsPath = coverageResultsPath,
                    recording = GetRecordingStatus()
                };

                if (!isCoveragePackageInstalled)
                {
                    return Response.Success(
                        "Code Coverage package is not installed. Install com.unity.testtools.codecoverage to use coverage features.",
                        data);
                }

                string message = isEnabled
                    ? "Code coverage is enabled"
                    : "Code coverage is disabled";

                return Response.Success(message, data);
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to get coverage settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if the Unity Code Coverage package is installed.
        /// </summary>
        /// <returns>True if the package is installed, false otherwise.</returns>
        private static bool IsCoveragePackageInstalled()
        {
#if UNITY_CODE_COVERAGE
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Gets the current recording status from Unity's Code Coverage API.
        /// </summary>
        /// <returns>True if coverage is currently recording, false otherwise or if package is not installed.</returns>
        private static bool GetRecordingStatus()
        {
#if UNITY_CODE_COVERAGE
            try
            {
                // Unity's Coverage API doesn't expose a direct "IsRecording" property,
                // but we can infer it from whether coverage is enabled
                return Coverage.enabled;
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }
    }
}
