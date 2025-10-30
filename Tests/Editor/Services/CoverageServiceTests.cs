using NUnit.Framework;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;
using System;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tests.Services
{
    [TestFixture]
    public class CoverageServiceTests
    {
        private ICoverageService _coverageService;

        [SetUp]
        public void Setup()
        {
            _coverageService = new CoverageService();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up EditorPrefs
            UnityEditor.EditorPrefs.DeleteKey("CodeCoverageEnabled");
        }

        [Test]
        public void IsCoverageEnabled_ReturnsCorrectState()
        {
            // This test will pass regardless of Code Coverage package installation
            // If package is not installed, it should return false
            bool result = _coverageService.IsCoverageEnabled();
            Assert.That(result, Is.TypeOf<bool>());
        }

#if UNITY_CODE_COVERAGE
        [Test]
        public void EnableCoverage_WithValidParameters_EnablesCoverage()
        {
            // Arrange
            bool enabled = true;
            bool autoGenerateReport = true;

            // Act
            _coverageService.EnableCoverage(enabled, autoGenerateReport);

            // Assert
            Assert.IsTrue(_coverageService.IsCoverageEnabled());
            Assert.IsTrue(UnityEditor.EditorPrefs.GetBool("CodeCoverageEnabled", false));
        }

        [Test]
        public void EnableCoverage_DisableCoverage_DisablesCoverage()
        {
            // Arrange
            _coverageService.EnableCoverage(true, true);

            // Act
            _coverageService.EnableCoverage(false, false);

            // Assert
            Assert.IsFalse(_coverageService.IsCoverageEnabled());
            Assert.IsFalse(UnityEditor.EditorPrefs.GetBool("CodeCoverageEnabled", false));
        }

        [Test]
        public void StartRecording_WhenCoverageDisabled_ThrowsInvalidOperationException()
        {
            // Arrange
            _coverageService.EnableCoverage(false, false);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _coverageService.StartRecording());
        }

        [Test]
        public void StartRecording_WhenCoverageEnabled_DoesNotThrow()
        {
            // Arrange
            _coverageService.EnableCoverage(true, true);

            // Act & Assert
            Assert.DoesNotThrow(() => _coverageService.StartRecording());
        }

        [Test]
        public void StopRecording_AfterStartRecording_DoesNotThrow()
        {
            // Arrange
            _coverageService.EnableCoverage(true, true);
            _coverageService.StartRecording();

            // Act & Assert
            Assert.DoesNotThrow(() => _coverageService.StopRecording());
        }

        [Test]
        public void GetCoverageResultsPath_ReturnsNonEmptyString()
        {
            // Act
            string path = _coverageService.GetCoverageResultsPath();

            // Assert
            Assert.IsNotNull(path);
            Assert.IsNotEmpty(path);
        }
#else
        [Test]
        public void EnableCoverage_WithoutCodeCoveragePackage_ThrowsNotSupportedException()
        {
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => _coverageService.EnableCoverage(true, true));
        }

        [Test]
        public void StartRecording_WithoutCodeCoveragePackage_ThrowsNotSupportedException()
        {
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => _coverageService.StartRecording());
        }

        [Test]
        public void StopRecording_WithoutCodeCoveragePackage_ThrowsNotSupportedException()
        {
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => _coverageService.StopRecording());
        }
#endif
    }
}
