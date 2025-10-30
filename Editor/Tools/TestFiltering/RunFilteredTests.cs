using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.TestFiltering
{
    /// <summary>
    /// MCP tool for running Unity tests with advanced filtering capabilities.
    /// Supports filtering by mode, name patterns (regex), categories, assemblies, and specific full names.
    /// </summary>
    [McpForUnityTool("run_filtered_tests")]
    public static class RunFilteredTests
    {
        private static readonly ITestFilterService _filterService = new TestFilterService();
        private static readonly TestRunnerApi _testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        private static TaskCompletionSource<TestRunResult> _testRunTcs;

        /// <summary>
        /// Handles the run_filtered_tests command.
        /// Filters and executes Unity tests based on specified criteria.
        /// </summary>
        /// <param name="params">
        /// JSON parameters:
        /// - mode (string, required): "edit" for EditMode or "play" for PlayMode
        /// - namePattern (string, optional): Regex pattern to match test names
        /// - categories (string[], optional): Array of category names to include
        /// - assemblies (string[], optional): Array of assembly names to filter
        /// - fullNames (string[], optional): Array of specific test full names to run
        /// </param>
        /// <returns>
        /// Success response with test results including:
        /// - totalTests: Number of tests executed
        /// - passedTests: Number of tests that passed
        /// - failedTests: Number of tests that failed
        /// - skippedTests: Number of tests that were skipped
        /// - duration: Total execution time in seconds
        /// - results: Array of individual test results with name, status, message, and duration
        /// </returns>
        public static object HandleCommand(JObject @params)
        {
            try
            {
                // Parse required 'mode' parameter
                var modeToken = @params["mode"];
                if (modeToken == null)
                {
                    return Response.Error("Required parameter 'mode' (string) is missing. Must be 'edit' or 'play'.");
                }

                if (modeToken.Type != JTokenType.String)
                {
                    return Response.Error($"Parameter 'mode' must be a string, got: {modeToken.Type}");
                }

                string mode = modeToken.Value<string>();
                if (string.IsNullOrEmpty(mode))
                {
                    return Response.Error("Parameter 'mode' cannot be empty. Must be 'edit' or 'play'.");
                }

                // Validate mode
                mode = mode.ToLowerInvariant();
                if (mode != "edit" && mode != "play")
                {
                    return Response.Error($"Invalid mode '{mode}'. Must be 'edit' or 'play'.");
                }

                // Parse optional 'namePattern' parameter
                string namePattern = null;
                var namePatternToken = @params["namePattern"];
                if (namePatternToken != null)
                {
                    if (namePatternToken.Type != JTokenType.String)
                    {
                        return Response.Error($"Parameter 'namePattern' must be a string, got: {namePatternToken.Type}");
                    }
                    namePattern = namePatternToken.Value<string>();
                }

                // Parse optional 'categories' parameter
                string[] categories = null;
                var categoriesToken = @params["categories"];
                if (categoriesToken != null)
                {
                    if (categoriesToken.Type != JTokenType.Array)
                    {
                        return Response.Error($"Parameter 'categories' must be an array, got: {categoriesToken.Type}");
                    }
                    categories = categoriesToken.ToObject<string[]>();
                }

                // Parse optional 'assemblies' parameter
                string[] assemblies = null;
                var assembliesToken = @params["assemblies"];
                if (assembliesToken != null)
                {
                    if (assembliesToken.Type != JTokenType.Array)
                    {
                        return Response.Error($"Parameter 'assemblies' must be an array, got: {assembliesToken.Type}");
                    }
                    assemblies = assembliesToken.ToObject<string[]>();
                }

                // Parse optional 'fullNames' parameter (specific test names to run)
                string[] fullNames = null;
                var fullNamesToken = @params["fullNames"];
                if (fullNamesToken != null)
                {
                    if (fullNamesToken.Type != JTokenType.Array)
                    {
                        return Response.Error($"Parameter 'fullNames' must be an array, got: {fullNamesToken.Type}");
                    }
                    fullNames = fullNamesToken.ToObject<string[]>();
                }

                // Build filter
                Filter filter;
                try
                {
                    filter = _filterService.BuildTestFilter(mode, namePattern, categories, assemblies);
                }
                catch (ArgumentException ex)
                {
                    return Response.Error($"Invalid filter parameters: {ex.Message}");
                }

                // If fullNames are provided, override the filter's test names
                if (fullNames != null && fullNames.Length > 0)
                {
                    filter.testNames = fullNames;
                }

                // Retrieve test tree
                TestMode testMode = mode == "edit" ? TestMode.EditMode : TestMode.PlayMode;
                ITestAdaptor testTree;
                try
                {
                    testTree = _testRunnerApi.RetrieveTestList(testMode);
                }
                catch (Exception ex)
                {
                    return Response.Error($"Failed to retrieve test tree: {ex.Message}");
                }

                if (testTree == null)
                {
                    return Response.Error("Failed to retrieve test tree. Ensure Unity Test Framework is properly configured.");
                }

                // Filter tests
                ITestAdaptor[] filteredTests;
                try
                {
                    filteredTests = _filterService.FilterTests(testTree, filter);
                }
                catch (Exception ex)
                {
                    return Response.Error($"Failed to filter tests: {ex.Message}");
                }

                if (filteredTests.Length == 0)
                {
                    return Response.Success("No tests matched the filter criteria", new
                    {
                        totalTests = 0,
                        passedTests = 0,
                        failedTests = 0,
                        skippedTests = 0,
                        duration = 0.0,
                        results = Array.Empty<object>()
                    });
                }

                Debug.Log($"[RunFilteredTests] Running {filteredTests.Length} filtered tests in {mode} mode");

                // Run tests asynchronously
                var task = RunTestsAsync(filter, testMode);

                // Wait for completion (blocking, but necessary for MCP synchronous interface)
                task.Wait();

                var result = task.Result;

                // Format response
                var data = new
                {
                    totalTests = result.TotalTests,
                    passedTests = result.PassedTests,
                    failedTests = result.FailedTests,
                    skippedTests = result.SkippedTests,
                    duration = result.Duration,
                    results = result.TestResults.Select(tr => new
                    {
                        fullName = tr.FullName,
                        status = tr.Status,
                        message = tr.Message,
                        stackTrace = tr.StackTrace,
                        duration = tr.Duration
                    }).ToArray()
                };

                string message = $"Executed {result.TotalTests} tests: {result.PassedTests} passed, {result.FailedTests} failed, {result.SkippedTests} skipped";
                return Response.Success(message, data);
            }
            catch (AggregateException ex)
            {
                var innerException = ex.InnerException ?? ex;
                return Response.Error($"Test execution failed: {innerException.Message}");
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to run filtered tests: {ex.Message}");
            }
        }

        /// <summary>
        /// Runs tests asynchronously using Unity Test Runner API.
        /// </summary>
        /// <param name="filter">The filter specifying which tests to run.</param>
        /// <param name="testMode">The test mode (EditMode or PlayMode).</param>
        /// <returns>A task that resolves to the test run results.</returns>
        private static async Task<TestRunResult> RunTestsAsync(Filter filter, TestMode testMode)
        {
            _testRunTcs = new TaskCompletionSource<TestRunResult>();

            try
            {
                // Create callback handler
                var callbackHandler = new TestRunCallbackHandler(_testRunTcs);
                _testRunnerApi.RegisterCallbacks(callbackHandler);

                // Execute tests with filter
                _testRunnerApi.Execute(new ExecutionSettings(filter));

                // Await completion
                var result = await _testRunTcs.Task;

                // Unregister callbacks
                _testRunnerApi.UnregisterCallbacks(callbackHandler);

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RunFilteredTests] Test execution failed: {ex.Message}");
                _testRunTcs?.TrySetException(ex);
                throw;
            }
        }

        /// <summary>
        /// Callback handler for Unity Test Runner API events.
        /// Collects test results and signals completion.
        /// </summary>
        private class TestRunCallbackHandler : ICallbacks
        {
            private readonly TaskCompletionSource<TestRunResult> _tcs;
            private readonly List<TestResult> _testResults = new List<TestResult>();
            private DateTime _startTime;

            public TestRunCallbackHandler(TaskCompletionSource<TestRunResult> tcs)
            {
                _tcs = tcs;
            }

            public void RunStarted(ITestAdaptor testsToRun)
            {
                _startTime = DateTime.Now;
                Debug.Log($"[RunFilteredTests] Test run started");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                var duration = (DateTime.Now - _startTime).TotalSeconds;

                var runResult = new TestRunResult
                {
                    TotalTests = _testResults.Count,
                    PassedTests = _testResults.Count(tr => tr.Status == "passed"),
                    FailedTests = _testResults.Count(tr => tr.Status == "failed"),
                    SkippedTests = _testResults.Count(tr => tr.Status == "skipped"),
                    Duration = duration,
                    TestResults = _testResults.ToArray()
                };

                Debug.Log($"[RunFilteredTests] Test run finished: {runResult.PassedTests} passed, {runResult.FailedTests} failed, {runResult.SkippedTests} skipped");
                _tcs.TrySetResult(runResult);
            }

            public void TestStarted(ITestAdaptor test)
            {
                Debug.Log($"[RunFilteredTests] Starting test: {test.FullName}");
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                string status;
                switch (result.TestStatus)
                {
                    case TestStatus.Passed:
                        status = "passed";
                        break;
                    case TestStatus.Failed:
                        status = "failed";
                        break;
                    case TestStatus.Skipped:
                        status = "skipped";
                        break;
                    default:
                        status = "inconclusive";
                        break;
                }

                var testResult = new TestResult
                {
                    FullName = result.Test.FullName,
                    Status = status,
                    Message = result.Message ?? string.Empty,
                    StackTrace = result.StackTrace ?? string.Empty,
                    Duration = result.Duration
                };

                _testResults.Add(testResult);

                if (status == "failed")
                {
                    Debug.LogError($"[RunFilteredTests] Test failed: {result.Test.FullName}\n{result.Message}");
                }
                else
                {
                    Debug.Log($"[RunFilteredTests] Test {status}: {result.Test.FullName}");
                }
            }
        }

        /// <summary>
        /// Container for aggregated test run results.
        /// </summary>
        private class TestRunResult
        {
            public int TotalTests { get; set; }
            public int PassedTests { get; set; }
            public int FailedTests { get; set; }
            public int SkippedTests { get; set; }
            public double Duration { get; set; }
            public TestResult[] TestResults { get; set; }
        }

        /// <summary>
        /// Container for individual test result data.
        /// </summary>
        private class TestResult
        {
            public string FullName { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public double Duration { get; set; }
        }
    }
}
