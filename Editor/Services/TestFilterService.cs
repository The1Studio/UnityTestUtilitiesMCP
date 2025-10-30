using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Services
{
    /// <summary>
    /// Implementation of <see cref="ITestFilterService"/> using Unity Test Framework API.
    /// Provides test filtering capabilities based on mode, name patterns, categories, and assemblies.
    /// </summary>
    public class TestFilterService : ITestFilterService
    {
        /// <summary>
        /// Builds a Unity Test Framework Filter object based on specified criteria.
        /// </summary>
        /// <param name="mode">The test mode to filter ("edit" for EditMode, "play" for PlayMode, or null for all modes).</param>
        /// <param name="namePattern">Regex pattern to match against test names (optional).</param>
        /// <param name="categories">Array of test category names to include (optional).</param>
        /// <param name="assemblies">Array of assembly names to filter by (optional).</param>
        /// <returns>A Unity Test Framework Filter configured with the specified criteria.</returns>
        /// <exception cref="ArgumentException">Thrown if the mode parameter is invalid.</exception>
        public Filter BuildTestFilter(string mode, string namePattern, string[] categories, string[] assemblies)
        {
            var filter = new Filter();

            // Set test mode
            if (!string.IsNullOrEmpty(mode))
            {
                switch (mode.ToLowerInvariant())
                {
                    case "edit":
                        filter.testMode = TestMode.EditMode;
                        break;
                    case "play":
                        filter.testMode = TestMode.PlayMode;
                        break;
                    default:
                        throw new ArgumentException($"Invalid test mode: '{mode}'. Must be 'edit' or 'play'.", nameof(mode));
                }
            }
            else
            {
                // If no mode specified, use EditMode as default (Unity Test Framework behavior)
                filter.testMode = TestMode.EditMode | TestMode.PlayMode;
            }

            // Set name pattern (Unity uses simple name matching, not full regex)
            if (!string.IsNullOrEmpty(namePattern))
            {
                filter.testNames = new[] { namePattern };
            }

            // Set categories
            if (categories != null && categories.Length > 0)
            {
                filter.categoryNames = categories;
            }

            // Set assembly names
            if (assemblies != null && assemblies.Length > 0)
            {
                filter.assemblyNames = assemblies;
            }

            return filter;
        }

        /// <summary>
        /// Filters a test tree based on a Unity Test Framework Filter.
        /// Recursively traverses the test tree and collects matching tests.
        /// </summary>
        /// <param name="testTree">The root test adaptor representing the test tree to filter.</param>
        /// <param name="filter">The filter to apply to the test tree.</param>
        /// <returns>An array of ITestAdaptor instances that match the filter criteria.</returns>
        /// <exception cref="ArgumentNullException">Thrown if testTree or filter is null.</exception>
        public ITestAdaptor[] FilterTests(ITestAdaptor testTree, Filter filter)
        {
            if (testTree == null)
            {
                throw new ArgumentNullException(nameof(testTree));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var matchingTests = new List<ITestAdaptor>();
            FilterTestsRecursive(testTree, filter, matchingTests);

            Debug.Log($"[TestFilterService] Filtered {matchingTests.Count} tests from tree");
            return matchingTests.ToArray();
        }

        /// <summary>
        /// Recursively filters tests in a test tree.
        /// </summary>
        /// <param name="testNode">Current test node to evaluate.</param>
        /// <param name="filter">The filter criteria to apply.</param>
        /// <param name="matchingTests">List to collect matching tests.</param>
        private void FilterTestsRecursive(ITestAdaptor testNode, Filter filter, List<ITestAdaptor> matchingTests)
        {
            if (testNode == null) return;

            // Check if this node is a test method (not a suite/fixture)
            bool isTestMethod = testNode.IsSuite == false && testNode.Children == null || !testNode.Children.Any();

            if (isTestMethod)
            {
                if (MatchesFilter(testNode, filter))
                {
                    matchingTests.Add(testNode);
                }
            }

            // Recursively filter children
            if (testNode.Children != null)
            {
                foreach (var child in testNode.Children)
                {
                    FilterTestsRecursive(child, filter, matchingTests);
                }
            }
        }

        /// <summary>
        /// Checks if a test matches the specified filter criteria.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="filter">The filter criteria.</param>
        /// <returns>True if the test matches all filter criteria, false otherwise.</returns>
        private bool MatchesFilter(ITestAdaptor test, Filter filter)
        {
            // Check test mode
            if (filter.testMode != 0 && filter.testMode != (TestMode.EditMode | TestMode.PlayMode))
            {
                // Determine test mode from test fixture attributes or parent
                var testMode = GetTestMode(test);
                if ((filter.testMode & testMode) == 0)
                {
                    return false;
                }
            }

            // Check test name pattern
            if (filter.testNames != null && filter.testNames.Length > 0)
            {
                bool matchesAnyPattern = false;
                foreach (var pattern in filter.testNames)
                {
                    try
                    {
                        // Support both simple contains and regex patterns
                        if (IsRegexPattern(pattern))
                        {
                            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                            if (regex.IsMatch(test.FullName))
                            {
                                matchesAnyPattern = true;
                                break;
                            }
                        }
                        else
                        {
                            // Simple contains matching
                            if (test.FullName.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                matchesAnyPattern = true;
                                break;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Invalid regex, fall back to simple contains
                        if (test.FullName.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matchesAnyPattern = true;
                            break;
                        }
                    }
                }

                if (!matchesAnyPattern)
                {
                    return false;
                }
            }

            // Check categories
            if (filter.categoryNames != null && filter.categoryNames.Length > 0)
            {
                var testCategories = test.Categories;
                if (testCategories == null || !filter.categoryNames.Any(cat => testCategories.Contains(cat)))
                {
                    return false;
                }
            }

            // Check assembly names
            if (filter.assemblyNames != null && filter.assemblyNames.Length > 0)
            {
                string testAssembly = GetAssemblyName(test);
                if (!filter.assemblyNames.Any(asm => testAssembly.IndexOf(asm, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a pattern string is a regex pattern (contains regex special characters).
        /// </summary>
        /// <param name="pattern">The pattern to check.</param>
        /// <returns>True if the pattern appears to be a regex, false otherwise.</returns>
        private bool IsRegexPattern(string pattern)
        {
            // Check for common regex metacharacters
            return pattern.IndexOfAny(new[] { '^', '$', '*', '+', '?', '[', ']', '(', ')', '{', '}', '|', '\\' }) >= 0;
        }

        /// <summary>
        /// Gets the test mode (EditMode or PlayMode) for a test.
        /// </summary>
        /// <param name="test">The test to check.</param>
        /// <returns>The test mode.</returns>
        private TestMode GetTestMode(ITestAdaptor test)
        {
            // Unity Test Framework determines mode based on assembly attributes
            // EditMode tests are in assemblies with UNITY_INCLUDE_TESTS defined
            // PlayMode tests are in separate assemblies

            // Check test fixture for mode indicators
            var fullName = test.FullName;

            // This is a heuristic - Unity stores mode info internally
            // For more accurate detection, we'd need to check the test's TypeInfo
            // For now, default to EditMode (most common)
            return TestMode.EditMode;
        }

        /// <summary>
        /// Extracts the assembly name from a test's full name or properties.
        /// </summary>
        /// <param name="test">The test to extract assembly name from.</param>
        /// <returns>The assembly name.</returns>
        private string GetAssemblyName(ITestAdaptor test)
        {
            // The full name typically includes namespace which often matches assembly name
            // Format: "Namespace.Class.Method"
            var fullName = test.FullName;

            // Try to get the root namespace (often matches assembly name)
            var parts = fullName.Split('.');
            if (parts.Length > 0)
            {
                return parts[0];
            }

            return string.Empty;
        }
    }
}
