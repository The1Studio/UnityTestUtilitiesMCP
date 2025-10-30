using System;
using System.Collections.Generic;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Services
{
    /// <summary>
    /// Represents a summary of Unity test results, including statistics and failure details.
    /// </summary>
    [Serializable]
    public class TestResultsSummary
    {
        /// <summary>
        /// Total number of tests executed.
        /// </summary>
        public int TotalTests { get; set; }

        /// <summary>
        /// Number of tests that passed.
        /// </summary>
        public int PassedTests { get; set; }

        /// <summary>
        /// Number of tests that failed.
        /// </summary>
        public int FailedTests { get; set; }

        /// <summary>
        /// Number of tests that were skipped or inconclusive.
        /// </summary>
        public int SkippedTests { get; set; }

        /// <summary>
        /// Total duration of the test run in seconds.
        /// </summary>
        public double DurationSeconds { get; set; }

        /// <summary>
        /// Timestamp when the test run started.
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Timestamp when the test run ended.
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// List of failed test details.
        /// </summary>
        public List<FailedTestDetail> FailedTestDetails { get; set; }

        /// <summary>
        /// Initializes a new instance of the TestResultsSummary class.
        /// </summary>
        public TestResultsSummary()
        {
            FailedTestDetails = new List<FailedTestDetail>();
        }
    }

    /// <summary>
    /// Represents detailed information about a failed test.
    /// </summary>
    [Serializable]
    public class FailedTestDetail
    {
        /// <summary>
        /// Fully qualified name of the failed test.
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// Name of the test suite or class containing the test.
        /// </summary>
        public string TestSuite { get; set; }

        /// <summary>
        /// Failure message describing what went wrong.
        /// </summary>
        public string FailureMessage { get; set; }

        /// <summary>
        /// Stack trace showing where the failure occurred.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Duration of the failed test in seconds.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Test result status (e.g., "Failed", "Error").
        /// </summary>
        public string Result { get; set; }
    }
}
