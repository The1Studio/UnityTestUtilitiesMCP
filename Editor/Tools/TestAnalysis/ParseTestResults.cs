using System;
using System.Linq;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Resources;
using Newtonsoft.Json.Linq;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.TestAnalysis
{
    /// <summary>
    /// MCP resource for parsing Unity test results into a structured summary.
    /// Analyzes TestResults.xml and provides statistics, failure details, and test metrics.
    /// </summary>
    [McpForUnityResource("parse_test_results")]
    public static class ParseTestResults
    {
        private static readonly ITestResultsService _testResultsService = new TestResultsService();

        /// <summary>
        /// Handles the parse_test_results resource query.
        /// Reads and parses Unity test results, returning comprehensive test statistics and failure details.
        /// </summary>
        /// <param name="params">No parameters required. Automatically locates and parses the most recent test results.</param>
        /// <returns>Success response with parsed test summary, or error response if parsing fails.</returns>
        public static object HandleCommand(JObject @params)
        {
            try
            {
                // Read test results file
                string xmlContent = _testResultsService.ReadTestResultsFile();

                // Parse results
                TestResultsSummary summary = _testResultsService.ParseTestResults(xmlContent);

                // Calculate success rate
                double successRate = summary.TotalTests > 0
                    ? (double)summary.PassedTests / summary.TotalTests * 100.0
                    : 0.0;

                // Build response data with comprehensive test information
                var data = new
                {
                    // Overall statistics
                    totalTests = summary.TotalTests,
                    passedTests = summary.PassedTests,
                    failedTests = summary.FailedTests,
                    skippedTests = summary.SkippedTests,
                    successRate = Math.Round(successRate, 2),

                    // Timing information
                    durationSeconds = Math.Round(summary.DurationSeconds, 3),
                    startTime = summary.StartTime,
                    endTime = summary.EndTime,

                    // Failed test details
                    failedTestDetails = summary.FailedTestDetails.Select(f => new
                    {
                        testName = f.TestName,
                        testSuite = f.TestSuite,
                        failureMessage = f.FailureMessage,
                        stackTrace = f.StackTrace,
                        duration = Math.Round(f.Duration, 3),
                        result = f.Result
                    }).ToList(),

                    // Summary flags
                    hasFailures = summary.FailedTests > 0,
                    allTestsPassed = summary.FailedTests == 0 && summary.TotalTests > 0
                };

                // Build status message
                string message;
                if (summary.TotalTests == 0)
                {
                    message = "No tests found in test results";
                }
                else if (summary.FailedTests == 0)
                {
                    message = $"All {summary.PassedTests} tests passed! Duration: {summary.DurationSeconds:F2}s";
                }
                else
                {
                    message = $"Test results: {summary.PassedTests}/{summary.TotalTests} passed, {summary.FailedTests} failed";
                }

                return Response.Success(message, data);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                return Response.Error(
                    $"Test results file not found: {ex.Message}. " +
                    "Run tests via Test Runner (Window → General → Test Runner) first.");
            }
            catch (System.Xml.XmlException ex)
            {
                return Response.Error($"Failed to parse test results XML: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return Response.Error($"Invalid test results data: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to parse test results: {ex.Message}");
            }
        }
    }
}
