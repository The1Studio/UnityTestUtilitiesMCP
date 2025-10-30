using NUnit.Framework;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;
using System.IO;
using UnityEngine;

namespace TheOneStudio.UnityTestUtilitiesMCP.Editor.Tests.Services
{
    [TestFixture]
    public class TestResultsServiceTests
    {
        private ITestResultsService _resultsService;
        private string _testFilePath;

        [SetUp]
        public void Setup()
        {
            _resultsService = new TestResultsService();
            _testFilePath = Path.Combine(Application.temporaryCachePath, "TestResults_Test.xml");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test file
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Test]
        public void LocateTestResultsFile_ReturnsStringOrNull()
        {
            // Act
            string result = _resultsService.LocateTestResultsFile();

            // Assert - Result can be null or a valid path
            if (result != null)
            {
                Assert.IsTrue(result.EndsWith("TestResults.xml"));
            }
        }

        [Test]
        public void ReadTestResultsFile_WithValidPath_ReturnsContent()
        {
            // Arrange
            string testContent = "<test-results>Test Content</test-results>";
            File.WriteAllText(_testFilePath, testContent);

            // Act
            string result = _resultsService.ReadTestResultsFile(_testFilePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(testContent, result);
        }

        [Test]
        public void ReadTestResultsFile_WithInvalidPath_ThrowsFileNotFoundException()
        {
            // Arrange
            string invalidPath = "/invalid/path/TestResults.xml";

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _resultsService.ReadTestResultsFile(invalidPath));
        }

        [Test]
        public void ParseTestResults_WithValidXml_ReturnsCorrectSummary()
        {
            // Arrange
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<test-run id=""2"" testcasecount=""10"" result=""Passed"" total=""10"" passed=""8"" failed=""1"" inconclusive=""0"" skipped=""1""
          start-time=""2025-10-30 09:00:00Z"" end-time=""2025-10-30 09:00:05Z"" duration=""5.123"">
  <test-suite type=""Assembly"" name=""UnityTestUtilitiesMCP.Editor.Tests"">
    <test-case id=""1001"" name=""TestMethod1"" result=""Passed"" duration=""0.123"" />
    <test-case id=""1002"" name=""TestMethod2"" result=""Failed"" duration=""0.456"">
      <failure>
        <message>Expected: True But was: False</message>
        <stack-trace>at TestClass.TestMethod2() in /path/to/test.cs:line 42</stack-trace>
      </failure>
    </test-case>
    <test-case id=""1003"" name=""TestMethod3"" result=""Skipped"" duration=""0.001"">
      <reason>
        <message>Test skipped</message>
      </reason>
    </test-case>
  </test-suite>
</test-run>";

            // Act
            TestResultsSummary summary = _resultsService.ParseTestResults(xmlContent);

            // Assert
            Assert.IsNotNull(summary);
            Assert.AreEqual(10, summary.TotalTests);
            Assert.AreEqual(8, summary.PassedTests);
            Assert.AreEqual(1, summary.FailedTests);
            Assert.AreEqual(1, summary.SkippedTests);
            Assert.AreEqual(5.123, summary.DurationSeconds, 0.001);
            Assert.AreEqual("2025-10-30 09:00:00Z", summary.StartTime);
            Assert.AreEqual("2025-10-30 09:00:05Z", summary.EndTime);
        }

        [Test]
        public void ParseTestResults_WithFailedTests_CapturesFailureDetails()
        {
            // Arrange
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<test-run id=""2"" testcasecount=""2"" result=""Failed"" total=""2"" passed=""1"" failed=""1"">
  <test-suite type=""Assembly"" name=""UnityTestUtilitiesMCP.Editor.Tests"">
    <test-case id=""1001"" name=""PassingTest"" result=""Passed"" duration=""0.1"" />
    <test-case id=""1002"" name=""FailingTest"" result=""Failed"" duration=""0.2"">
      <failure>
        <message>Expected: 5 But was: 3</message>
        <stack-trace>at TestClass.FailingTest() in /path/test.cs:line 10</stack-trace>
      </failure>
    </test-case>
  </test-suite>
</test-run>";

            // Act
            TestResultsSummary summary = _resultsService.ParseTestResults(xmlContent);

            // Assert
            Assert.IsNotNull(summary);
            Assert.IsNotNull(summary.FailedTests);
            Assert.AreEqual(1, summary.FailedTests.Count);

            FailedTestDetail failedTest = summary.FailedTests[0];
            Assert.AreEqual("FailingTest", failedTest.TestName);
            Assert.AreEqual("Expected: 5 But was: 3", failedTest.FailureMessage);
            Assert.IsTrue(failedTest.StackTrace.Contains("TestClass.FailingTest"));
            Assert.AreEqual(0.2, failedTest.Duration, 0.001);
        }

        [Test]
        public void ParseTestResults_WithEmptyXml_ThrowsException()
        {
            // Arrange
            string xmlContent = "";

            // Act & Assert
            Assert.Throws<System.Xml.XmlException>(() => _resultsService.ParseTestResults(xmlContent));
        }

        [Test]
        public void ParseTestResults_WithInvalidXml_ThrowsException()
        {
            // Arrange
            string xmlContent = "<invalid>xml<without-closing-tag>";

            // Act & Assert
            Assert.Throws<System.Xml.XmlException>(() => _resultsService.ParseTestResults(xmlContent));
        }

        [Test]
        public void ParseTestResults_WithNoFailedTests_ReturnsEmptyFailedList()
        {
            // Arrange
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<test-run id=""2"" testcasecount=""2"" result=""Passed"" total=""2"" passed=""2"" failed=""0"">
  <test-suite type=""Assembly"" name=""UnityTestUtilitiesMCP.Editor.Tests"">
    <test-case id=""1001"" name=""Test1"" result=""Passed"" duration=""0.1"" />
    <test-case id=""1002"" name=""Test2"" result=""Passed"" duration=""0.2"" />
  </test-suite>
</test-run>";

            // Act
            TestResultsSummary summary = _resultsService.ParseTestResults(xmlContent);

            // Assert
            Assert.IsNotNull(summary);
            Assert.AreEqual(2, summary.PassedTests);
            Assert.AreEqual(0, summary.FailedTests);
            Assert.IsNotNull(summary.FailedTests);
            Assert.AreEqual(0, summary.FailedTests.Count);
        }

        [Test]
        public void ParseTestResults_WithSkippedTests_CapturesSkipCount()
        {
            // Arrange
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<test-run id=""2"" testcasecount=""3"" result=""Passed"" total=""3"" passed=""2"" failed=""0"" skipped=""1"">
  <test-suite type=""Assembly"" name=""UnityTestUtilitiesMCP.Editor.Tests"">
    <test-case id=""1001"" name=""Test1"" result=""Passed"" duration=""0.1"" />
    <test-case id=""1002"" name=""Test2"" result=""Skipped"" duration=""0.0"">
      <reason><message>Ignored test</message></reason>
    </test-case>
    <test-case id=""1003"" name=""Test3"" result=""Passed"" duration=""0.2"" />
  </test-suite>
</test-run>";

            // Act
            TestResultsSummary summary = _resultsService.ParseTestResults(xmlContent);

            // Assert
            Assert.IsNotNull(summary);
            Assert.AreEqual(3, summary.TotalTests);
            Assert.AreEqual(2, summary.PassedTests);
            Assert.AreEqual(0, summary.FailedTests);
            Assert.AreEqual(1, summary.SkippedTests);
        }

        [Test]
        public void ParseTestResults_WithMultipleFailures_CapturesAllFailures()
        {
            // Arrange
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<test-run id=""2"" testcasecount=""3"" result=""Failed"" total=""3"" passed=""1"" failed=""2"">
  <test-suite type=""Assembly"" name=""UnityTestUtilitiesMCP.Editor.Tests"">
    <test-case id=""1001"" name=""Test1"" result=""Failed"" duration=""0.1"">
      <failure><message>Error 1</message><stack-trace>Stack 1</stack-trace></failure>
    </test-case>
    <test-case id=""1002"" name=""Test2"" result=""Passed"" duration=""0.2"" />
    <test-case id=""1003"" name=""Test3"" result=""Failed"" duration=""0.3"">
      <failure><message>Error 2</message><stack-trace>Stack 2</stack-trace></failure>
    </test-case>
  </test-suite>
</test-run>";

            // Act
            TestResultsSummary summary = _resultsService.ParseTestResults(xmlContent);

            // Assert
            Assert.IsNotNull(summary);
            Assert.AreEqual(2, summary.FailedTests);
            Assert.IsNotNull(summary.FailedTests);
            Assert.AreEqual(2, summary.FailedTests.Count);
            Assert.AreEqual("Test1", summary.FailedTests[0].TestName);
            Assert.AreEqual("Test3", summary.FailedTests[1].TestName);
        }
    }
}
