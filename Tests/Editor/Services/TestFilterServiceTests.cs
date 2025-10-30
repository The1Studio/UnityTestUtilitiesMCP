using NUnit.Framework;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;
using UnityEditor.TestTools.TestRunner.Api;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tests.Services
{
    [TestFixture]
    public class TestFilterServiceTests
    {
        private ITestFilterService _filterService;

        [SetUp]
        public void Setup()
        {
            _filterService = new TestFilterService();
        }

        [Test]
        public void BuildTestFilter_WithEditMode_ReturnsEditModeFilter()
        {
            // Arrange
            string mode = "edit";

            // Act
            Filter filter = _filterService.BuildTestFilter(mode, null, null, null);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(TestMode.EditMode, filter.testMode);
        }

        [Test]
        public void BuildTestFilter_WithPlayMode_ReturnsPlayModeFilter()
        {
            // Arrange
            string mode = "play";

            // Act
            Filter filter = _filterService.BuildTestFilter(mode, null, null, null);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(TestMode.PlayMode, filter.testMode);
        }

        [Test]
        public void BuildTestFilter_WithCategories_SetsCategoryNames()
        {
            // Arrange
            string[] categories = new[] { "Unit", "Integration" };

            // Act
            Filter filter = _filterService.BuildTestFilter("edit", null, categories, null);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(categories, filter.categoryNames);
        }

        [Test]
        public void BuildTestFilter_WithAssemblies_SetsAssemblyNames()
        {
            // Arrange
            string[] assemblies = new[] { "UnityTestUtilitiesMCP.Editor" };

            // Act
            Filter filter = _filterService.BuildTestFilter("edit", null, null, assemblies);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(assemblies, filter.assemblyNames);
        }

        [Test]
        public void BuildTestFilter_WithNamePattern_SetsTestNames()
        {
            // Arrange
            string namePattern = "CoverageService";

            // Act
            Filter filter = _filterService.BuildTestFilter("edit", namePattern, null, null);

            // Assert
            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.testNames);
            Assert.Greater(filter.testNames.Length, 0);
        }

        [Test]
        public void BuildTestFilter_WithNullParameters_ReturnsValidFilter()
        {
            // Act
            Filter filter = _filterService.BuildTestFilter("edit", null, null, null);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(TestMode.EditMode, filter.testMode);
        }

        [Test]
        public void BuildTestFilter_WithEmptyStrings_HandlesGracefully()
        {
            // Arrange
            string namePattern = "";
            string[] categories = new string[] { };
            string[] assemblies = new string[] { };

            // Act
            Filter filter = _filterService.BuildTestFilter("edit", namePattern, categories, assemblies);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(TestMode.EditMode, filter.testMode);
        }

        [Test]
        public void BuildTestFilter_WithInvalidMode_DefaultsToEditMode()
        {
            // Arrange
            string mode = "invalid";

            // Act
            Filter filter = _filterService.BuildTestFilter(mode, null, null, null);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(TestMode.EditMode, filter.testMode);
        }

        [Test]
        public void BuildTestFilter_WithCaseInsensitiveMode_WorksCorrectly()
        {
            // Arrange & Act
            Filter filterUpper = _filterService.BuildTestFilter("PLAY", null, null, null);
            Filter filterLower = _filterService.BuildTestFilter("play", null, null, null);
            Filter filterMixed = _filterService.BuildTestFilter("Play", null, null, null);

            // Assert
            Assert.AreEqual(TestMode.PlayMode, filterUpper.testMode);
            Assert.AreEqual(TestMode.PlayMode, filterLower.testMode);
            Assert.AreEqual(TestMode.PlayMode, filterMixed.testMode);
        }

        [Test]
        public void BuildTestFilter_WithMultipleCategories_SetsAllCategories()
        {
            // Arrange
            string[] categories = new[] { "Unit", "Integration", "Performance" };

            // Act
            Filter filter = _filterService.BuildTestFilter("edit", null, categories, null);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(3, filter.categoryNames.Length);
            CollectionAssert.AreEqual(categories, filter.categoryNames);
        }

        [Test]
        public void BuildTestFilter_WithMultipleAssemblies_SetsAllAssemblies()
        {
            // Arrange
            string[] assemblies = new[] { "UnityTestUtilitiesMCP.Editor", "UnityTestUtilitiesMCP.Runtime" };

            // Act
            Filter filter = _filterService.BuildTestFilter("edit", null, null, assemblies);

            // Assert
            Assert.IsNotNull(filter);
            Assert.AreEqual(2, filter.assemblyNames.Length);
            CollectionAssert.AreEqual(assemblies, filter.assemblyNames);
        }
    }
}
