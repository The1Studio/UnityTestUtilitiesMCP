using UnityEditor.TestTools.TestRunner.Api;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Services
{
    /// <summary>
    /// Service interface for filtering Unity tests based on various criteria.
    /// Supports filtering by test mode, name patterns, categories, assemblies, and specific test names.
    /// </summary>
    public interface ITestFilterService
    {
        /// <summary>
        /// Builds a Unity Test Framework Filter object based on specified criteria.
        /// </summary>
        /// <param name="mode">The test mode to filter ("edit" for EditMode, "play" for PlayMode, or null for all modes).</param>
        /// <param name="namePattern">Regex pattern to match against test names (optional).</param>
        /// <param name="categories">Array of test category names to include (optional).</param>
        /// <param name="assemblies">Array of assembly names to filter by (optional).</param>
        /// <returns>A Unity Test Framework Filter configured with the specified criteria.</returns>
        Filter BuildTestFilter(string mode, string namePattern, string[] categories, string[] assemblies);

        /// <summary>
        /// Filters a test tree based on a Unity Test Framework Filter.
        /// </summary>
        /// <param name="testTree">The root test adaptor representing the test tree to filter.</param>
        /// <param name="filter">The filter to apply to the test tree.</param>
        /// <returns>An array of ITestAdaptor instances that match the filter criteria.</returns>
        ITestAdaptor[] FilterTests(ITestAdaptor testTree, Filter filter);
    }
}
