using System.Threading.Tasks;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Services
{
    /// <summary>
    /// Service interface for managing Unity Code Coverage functionality.
    /// Provides methods to control code coverage recording, report generation, and settings management.
    /// </summary>
    public interface ICoverageService
    {
        /// <summary>
        /// Checks if code coverage recording is currently enabled.
        /// </summary>
        /// <returns>True if coverage is enabled, false otherwise.</returns>
        bool IsCoverageEnabled();

        /// <summary>
        /// Enables or disables code coverage recording.
        /// </summary>
        /// <param name="enabled">True to enable coverage, false to disable.</param>
        /// <param name="autoGenerateReport">If true, automatically generate HTML report after recording stops.</param>
        void EnableCoverage(bool enabled, bool autoGenerateReport = false);

        /// <summary>
        /// Starts recording code coverage data.
        /// Coverage must be enabled via <see cref="EnableCoverage"/> before calling this method.
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Stops recording code coverage data.
        /// If auto-generate report is enabled, a report will be generated after stopping.
        /// </summary>
        void StopRecording();

        /// <summary>
        /// Generates an HTML code coverage report asynchronously.
        /// </summary>
        /// <returns>A task that resolves to the file path of the generated HTML report, or an error message if generation fails.</returns>
        Task<string> GenerateReportAsync();

        /// <summary>
        /// Gets the file path where code coverage results are stored.
        /// </summary>
        /// <returns>The absolute path to the coverage results directory, or null if coverage is not available.</returns>
        string GetCoverageResultsPath();
    }
}
