using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_CODE_COVERAGE
using UnityEditor.TestTools.CodeCoverage;
#endif

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Services
{
    /// <summary>
    /// Implementation of <see cref="ICoverageService"/> using Unity's Code Coverage API.
    /// Requires the Unity Code Coverage package (com.unity.testtools.codecoverage) to be installed.
    /// </summary>
    public class CoverageService : ICoverageService
    {
        private const string PREF_COVERAGE_ENABLED = "UnityTestUtilitiesMCP.Coverage.Enabled";
        private const string PREF_AUTO_GENERATE_REPORT = "UnityTestUtilitiesMCP.Coverage.AutoGenerateReport";

        /// <summary>
        /// Checks if code coverage recording is currently enabled.
        /// </summary>
        /// <returns>True if coverage is enabled and the Code Coverage package is installed, false otherwise.</returns>
        public bool IsCoverageEnabled()
        {
#if UNITY_CODE_COVERAGE
            return EditorPrefs.GetBool(PREF_COVERAGE_ENABLED, false);
#else
            return false;
#endif
        }

        /// <summary>
        /// Enables or disables code coverage recording.
        /// </summary>
        /// <param name="enabled">True to enable coverage, false to disable.</param>
        /// <param name="autoGenerateReport">If true, automatically generate HTML report after recording stops.</param>
        /// <exception cref="NotSupportedException">Thrown if the Code Coverage package is not installed.</exception>
        public void EnableCoverage(bool enabled, bool autoGenerateReport = false)
        {
#if UNITY_CODE_COVERAGE
            EditorPrefs.SetBool(PREF_COVERAGE_ENABLED, enabled);
            EditorPrefs.SetBool(PREF_AUTO_GENERATE_REPORT, autoGenerateReport);

            // Update Unity's Code Coverage settings
            Coverage.enabled = enabled;

            Debug.Log($"[CoverageService] Code coverage {(enabled ? "enabled" : "disabled")}. Auto-generate report: {autoGenerateReport}");
#else
            throw new NotSupportedException(
                "Unity Code Coverage package (com.unity.testtools.codecoverage) is not installed. " +
                "Install it via Package Manager to use code coverage features.");
#endif
        }

        /// <summary>
        /// Starts recording code coverage data.
        /// Coverage must be enabled via <see cref="EnableCoverage"/> before calling this method.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if coverage is not enabled.</exception>
        /// <exception cref="NotSupportedException">Thrown if the Code Coverage package is not installed.</exception>
        public void StartRecording()
        {
#if UNITY_CODE_COVERAGE
            if (!IsCoverageEnabled())
            {
                throw new InvalidOperationException("Code coverage is not enabled. Call EnableCoverage(true) first.");
            }

            try
            {
                Coverage.ResetAll();
                Coverage.StartRecording();
                Debug.Log("[CoverageService] Code coverage recording started");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CoverageService] Failed to start coverage recording: {ex.Message}");
                throw;
            }
#else
            throw new NotSupportedException(
                "Unity Code Coverage package (com.unity.testtools.codecoverage) is not installed. " +
                "Install it via Package Manager to use code coverage features.");
#endif
        }

        /// <summary>
        /// Stops recording code coverage data.
        /// If auto-generate report is enabled, a report will be generated after stopping.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the Code Coverage package is not installed.</exception>
        public void StopRecording()
        {
#if UNITY_CODE_COVERAGE
            try
            {
                Coverage.StopRecording();
                Debug.Log("[CoverageService] Code coverage recording stopped");

                // Auto-generate report if enabled
                bool autoGenerate = EditorPrefs.GetBool(PREF_AUTO_GENERATE_REPORT, false);
                if (autoGenerate)
                {
                    Debug.Log("[CoverageService] Auto-generating coverage report...");
                    _ = GenerateReportAsync(); // Fire and forget
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CoverageService] Failed to stop coverage recording: {ex.Message}");
                throw;
            }
#else
            throw new NotSupportedException(
                "Unity Code Coverage package (com.unity.testtools.codecoverage) is not installed. " +
                "Install it via Package Manager to use code coverage features.");
#endif
        }

        /// <summary>
        /// Generates an HTML code coverage report asynchronously.
        /// </summary>
        /// <returns>A task that resolves to the file path of the generated HTML report, or an error message if generation fails.</returns>
        /// <exception cref="NotSupportedException">Thrown if the Code Coverage package is not installed.</exception>
        public async Task<string> GenerateReportAsync()
        {
#if UNITY_CODE_COVERAGE
            try
            {
                // Use TaskCompletionSource to convert callback-based API to async
                var tcs = new TaskCompletionSource<string>();

                // Ensure we're on the main thread for Unity API calls
                await Task.Yield();

                string resultsPath = GetCoverageResultsPath();
                if (string.IsNullOrEmpty(resultsPath))
                {
                    throw new InvalidOperationException("Coverage results path is not available");
                }

                // Generate the report using Unity's Code Coverage API
                Debug.Log("[CoverageService] Generating HTML coverage report...");

                // Unity's Coverage API generates reports synchronously
                // The report is automatically saved to the CodeCoverage folder
                var reportPath = Path.Combine(resultsPath, "Report", "index.html");

                // Trigger report generation (this is synchronous in Unity's API)
                Coverage.GenerateHTMLReport(resultsPath);

                if (File.Exists(reportPath))
                {
                    Debug.Log($"[CoverageService] Coverage report generated successfully at: {reportPath}");
                    tcs.SetResult(reportPath);
                }
                else
                {
                    var errorMsg = "Report generation completed but index.html was not found";
                    Debug.LogError($"[CoverageService] {errorMsg}");
                    tcs.SetException(new FileNotFoundException(errorMsg));
                }

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CoverageService] Failed to generate coverage report: {ex.Message}");
                throw;
            }
#else
            await Task.CompletedTask; // Suppress async warning
            throw new NotSupportedException(
                "Unity Code Coverage package (com.unity.testtools.codecoverage) is not installed. " +
                "Install it via Package Manager to use code coverage features.");
#endif
        }

        /// <summary>
        /// Gets the file path where code coverage results are stored.
        /// </summary>
        /// <returns>The absolute path to the coverage results directory, or null if coverage is not available.</returns>
        public string GetCoverageResultsPath()
        {
#if UNITY_CODE_COVERAGE
            try
            {
                // Unity stores coverage results in the project's CodeCoverage folder
                string projectPath = Directory.GetParent(Application.dataPath).FullName;
                string coveragePath = Path.Combine(projectPath, "CodeCoverage");

                // Create directory if it doesn't exist
                if (!Directory.Exists(coveragePath))
                {
                    Directory.CreateDirectory(coveragePath);
                    Debug.Log($"[CoverageService] Created coverage results directory: {coveragePath}");
                }

                return coveragePath;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CoverageService] Failed to get coverage results path: {ex.Message}");
                return null;
            }
#else
            return null;
#endif
        }
    }
}
