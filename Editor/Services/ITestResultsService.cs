using System.Threading.Tasks;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Services
{
    /// <summary>
    /// Service interface for analyzing Unity test results.
    /// Provides methods to locate, read, and parse Unity test result XML files.
    /// </summary>
    public interface ITestResultsService
    {
        /// <summary>
        /// Locates the most recent Unity test results file.
        /// Searches standard Unity configuration directories based on the platform:
        /// - Linux/Mac: ~/.config/unity3d/[CompanyName]/[ProductName]/TestResults.xml
        /// - Windows: %APPDATA%\..\LocalLow\[CompanyName]\[ProductName]\TestResults.xml
        /// </summary>
        /// <returns>The absolute path to the test results file, or null if not found.</returns>
        string LocateTestResultsFile();

        /// <summary>
        /// Reads the contents of the Unity test results file.
        /// </summary>
        /// <returns>The raw XML content of the test results file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the test results file cannot be found.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error reading the file.</exception>
        string ReadTestResultsFile();

        /// <summary>
        /// Parses Unity test results XML content into a structured summary.
        /// </summary>
        /// <param name="xmlContent">The raw XML content from Unity's NUnit test results file.</param>
        /// <returns>A structured summary containing test statistics and failure details.</returns>
        /// <exception cref="System.Xml.XmlException">Thrown if the XML content is malformed.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the XML content is null or empty.</exception>
        TestResultsSummary ParseTestResults(string xmlContent);
    }
}
