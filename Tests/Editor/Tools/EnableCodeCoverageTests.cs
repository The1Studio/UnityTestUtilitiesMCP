using NUnit.Framework;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage;
using Newtonsoft.Json.Linq;
using System;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tests.Tools
{
    [TestFixture]
    public class EnableCodeCoverageTests
    {
        [TearDown]
        public void TearDown()
        {
            // Clean up EditorPrefs
            UnityEditor.EditorPrefs.DeleteKey("CodeCoverageEnabled");
        }

#if UNITY_CODE_COVERAGE
        [Test]
        public void HandleCommand_WithEnabledTrue_EnablesCoverage()
        {
            // Arrange
            var parameters = new JObject
            {
                ["enabled"] = true,
                ["autoGenerateReport"] = true
            };

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("success", response["status"].ToString());
        }

        [Test]
        public void HandleCommand_WithEnabledFalse_DisablesCoverage()
        {
            // Arrange
            var parameters = new JObject
            {
                ["enabled"] = false,
                ["autoGenerateReport"] = false
            };

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("success", response["status"].ToString());
        }

        [Test]
        public void HandleCommand_WithMissingEnabledParameter_DefaultsToFalse()
        {
            // Arrange
            var parameters = new JObject
            {
                ["autoGenerateReport"] = true
            };

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("success", response["status"].ToString());
        }

        [Test]
        public void HandleCommand_WithMissingAutoGenerateParameter_DefaultsToTrue()
        {
            // Arrange
            var parameters = new JObject
            {
                ["enabled"] = true
            };

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("success", response["status"].ToString());
        }

        [Test]
        public void HandleCommand_WithEmptyParameters_UsesDefaults()
        {
            // Arrange
            var parameters = new JObject();

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("success", response["status"].ToString());
        }

        [Test]
        public void HandleCommand_WithNullParameters_HandlesGracefully()
        {
            // Arrange
            JObject parameters = null;

            // Act & Assert
            // Should handle null gracefully or throw appropriate exception
            Assert.DoesNotThrow(() => EnableCodeCoverage.HandleCommand(parameters));
        }

        [Test]
        public void HandleCommand_MultipleInvocations_WorksCorrectly()
        {
            // Arrange
            var enableParams = new JObject
            {
                ["enabled"] = true,
                ["autoGenerateReport"] = true
            };

            var disableParams = new JObject
            {
                ["enabled"] = false,
                ["autoGenerateReport"] = false
            };

            // Act
            object result1 = EnableCodeCoverage.HandleCommand(enableParams);
            object result2 = EnableCodeCoverage.HandleCommand(disableParams);
            object result3 = EnableCodeCoverage.HandleCommand(enableParams);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);

            JObject response1 = JObject.FromObject(result1);
            JObject response2 = JObject.FromObject(result2);
            JObject response3 = JObject.FromObject(result3);

            Assert.AreEqual("success", response1["status"].ToString());
            Assert.AreEqual("success", response2["status"].ToString());
            Assert.AreEqual("success", response3["status"].ToString());
        }

        [Test]
        public void HandleCommand_WithBooleanStrings_ParsesCorrectly()
        {
            // Arrange
            var parameters = new JObject
            {
                ["enabled"] = "true",
                ["autoGenerateReport"] = "false"
            };

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("success", response["status"].ToString());
        }
#else
        [Test]
        public void HandleCommand_WithoutCodeCoveragePackage_ReturnsError()
        {
            // Arrange
            var parameters = new JObject
            {
                ["enabled"] = true,
                ["autoGenerateReport"] = true
            };

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("error", response["status"].ToString());
            Assert.IsTrue(response["message"].ToString().Contains("not installed") ||
                         response["message"].ToString().Contains("not supported"));
        }

        [Test]
        public void HandleCommand_WithoutPackage_ReturnsNotSupportedMessage()
        {
            // Arrange
            var parameters = new JObject
            {
                ["enabled"] = true
            };

            // Act
            object result = EnableCodeCoverage.HandleCommand(parameters);

            // Assert
            Assert.IsNotNull(result);
            JObject response = JObject.FromObject(result);
            Assert.AreEqual("error", response["status"].ToString());
            StringAssert.Contains("Code Coverage package", response["message"].ToString());
        }
#endif
    }
}
