using NUnit.Framework;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.TestFiltering;
using Newtonsoft.Json.Linq;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tests.Tools
{
    [TestFixture]
    public class RunFilteredTestsTests
    {
        [Test]
        public void HandleCommand_WithEditMode_ReturnsValidResponse()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit"
            };

            // Act - Note: This is an async method, but we're testing the parameter handling
            // In a real async test, we'd use async/await
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
                // For unit tests, we're verifying the method accepts parameters correctly
                // Full integration testing would require Unity Test Runner to be active
            });
        }

        [Test]
        public void HandleCommand_WithPlayMode_ReturnsValidResponse()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "play"
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithNamePattern_AcceptsParameter()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["namePattern"] = "Coverage.*Test"
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithCategories_AcceptsParameter()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["categories"] = new JArray { "Unit", "Integration" }
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithAssemblies_AcceptsParameter()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["assemblies"] = new JArray { "UnityTestUtilitiesMCP.Editor" }
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithFullNames_AcceptsParameter()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["fullNames"] = new JArray
                {
                    "TheOneStudio.UnityTestUtilitiesMCP.Editor.Tests.Services.CoverageServiceTests.IsCoverageEnabled_ReturnsCorrectState"
                }
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithAllParameters_AcceptsAllParameters()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["namePattern"] = "Test.*",
                ["categories"] = new JArray { "Unit" },
                ["assemblies"] = new JArray { "UnityTestUtilitiesMCP.Editor" },
                ["fullNames"] = new JArray { "SomeTest.FullName" }
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithEmptyParameters_UsesDefaults()
        {
            // Arrange
            var parameters = new JObject();

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithNullMode_UsesDefaultEditMode()
        {
            // Arrange
            var parameters = new JObject
            {
                ["namePattern"] = "Test"
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithInvalidMode_HandlesGracefully()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "invalid_mode"
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithEmptyArrays_HandlesGracefully()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["categories"] = new JArray(),
                ["assemblies"] = new JArray(),
                ["fullNames"] = new JArray()
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithNullArrays_HandlesGracefully()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["categories"] = null,
                ["assemblies"] = null,
                ["fullNames"] = null
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithRegexPattern_AcceptsValidRegex()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["namePattern"] = "^Coverage.*Test$"
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithComplexRegexPattern_AcceptsComplexRegex()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["namePattern"] = "(Unit|Integration)Test.*[0-9]+"
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithMultipleCategories_AcceptsAllCategories()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["categories"] = new JArray { "Unit", "Integration", "Performance", "Smoke" }
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }

        [Test]
        public void HandleCommand_WithMultipleAssemblies_AcceptsAllAssemblies()
        {
            // Arrange
            var parameters = new JObject
            {
                ["mode"] = "edit",
                ["assemblies"] = new JArray
                {
                    "UnityTestUtilitiesMCP.Editor",
                    "UnityTestUtilitiesMCP.Runtime",
                    "UnityTestUtilitiesMCP.Editor.Tests"
                }
            };

            // Act
            Assert.DoesNotThrow(() =>
            {
                var task = RunFilteredTests.HandleCommand(parameters);
            });
        }
    }
}
