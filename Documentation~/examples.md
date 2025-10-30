# Usage Examples

Detailed examples and workflows for using Unity Test Utilities for MCP in various scenarios.

## Table of Contents

- [Basic Workflows](#basic-workflows)
  - [Example 1: First-Time Setup](#example-1-first-time-setup)
  - [Example 2: Basic Coverage Workflow](#example-2-basic-coverage-workflow)
  - [Example 3: Auto-Generate Report on Stop](#example-3-auto-generate-report-on-stop)
  - [Example 4: Manual Report Generation](#example-4-manual-report-generation)
- [Advanced Workflows](#advanced-workflows)
  - [Example 5: Multiple Test Sessions](#example-5-multiple-test-sessions)
  - [Example 6: Coverage with Specific Test Categories](#example-6-coverage-with-specific-test-categories)
  - [Example 7: Coverage Comparison Over Time](#example-7-coverage-comparison-over-time)
- [CI/CD Integration](#cicd-integration)
  - [Example 8: GitHub Actions](#example-8-github-actions)
  - [Example 9: Jenkins Pipeline](#example-9-jenkins-pipeline)
  - [Example 10: GitLab CI](#example-10-gitlab-ci)
  - [Example 11: Unity Cloud Build](#example-11-unity-cloud-build)
- [Claude Code Integration](#claude-code-integration)
  - [Example 12: Interactive Coverage Session](#example-12-interactive-coverage-session)
  - [Example 13: Automated Test-and-Coverage](#example-13-automated-test-and-coverage)
- [Unity Editor Scripts](#unity-editor-scripts)
  - [Example 14: Custom Menu Item](#example-14-custom-menu-item)
  - [Example 15: Automated Pre-Commit Hook](#example-15-automated-pre-commit-hook)
- [Troubleshooting Examples](#troubleshooting-examples)
  - [Example 16: Diagnosing Coverage Issues](#example-16-diagnosing-coverage-issues)
  - [Example 17: Verifying Package Installation](#example-17-verifying-package-installation)

---

## Basic Workflows

### Example 1: First-Time Setup

Complete setup workflow for first-time users.

```bash
# Step 1: Verify Unity MCP is connected
# Open Unity Editor → Window → MCP for Unity
# Ensure "Connected" status is shown

# Step 2: Check if Code Coverage package is installed
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# Expected response if package is not installed:
{
  "content": [{
    "type": "text",
    "text": "Code Coverage package is not installed..."
  }],
  "isError": false,
  "_meta": {
    "coveragePackageInstalled": false,
    "enabled": false,
    "autoGenerateReport": false,
    "coverageResultsPath": null,
    "recording": false
  }
}

# Step 3: Install Code Coverage package
# Unity Editor → Window → Package Manager
# Unity Registry → Search "Code Coverage" → Install

# Step 4: Verify installation
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# Expected response after installation:
{
  "content": [{
    "type": "text",
    "text": "Code coverage is disabled"
  }],
  "isError": false,
  "_meta": {
    "coveragePackageInstalled": true,
    "enabled": false,
    "autoGenerateReport": false,
    "coverageResultsPath": "/path/to/project/CodeCoverage",
    "recording": false
  }
}

# Step 5: Enable coverage
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'

# Success! Ready to use coverage features.
```

---

### Example 2: Basic Coverage Workflow

Standard workflow for running tests with coverage.

```bash
# 1. Enable coverage
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'

# Response:
{
  "content": [{
    "type": "text",
    "text": "Code coverage enabled. Auto-generate report: false"
  }],
  "isError": false,
  "_meta": {
    "enabled": true,
    "autoGenerateReport": false,
    "coverageResultsPath": "/home/user/MyProject/CodeCoverage"
  }
}

# 2. Start recording
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# Response:
{
  "content": [{
    "type": "text",
    "text": "Code coverage recording started"
  }],
  "isError": false,
  "_meta": {
    "recording": true,
    "coverageResultsPath": "/home/user/MyProject/CodeCoverage"
  }
}

# 3. Run tests in Unity Editor
# Window → General → Test Runner
# Select tests and click "Run All" or "Run Selected"
# Wait for tests to complete

# 4. Stop recording and generate report
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'

# Response:
{
  "content": [{
    "type": "text",
    "text": "Code coverage recording stopped and report generated"
  }],
  "isError": false,
  "_meta": {
    "recording": false,
    "reportGenerated": true,
    "reportPath": "/home/user/MyProject/CodeCoverage/Report/index.html",
    "coverageResultsPath": "/home/user/MyProject/CodeCoverage"
  }
}

# 5. Open report in browser
# Navigate to the reportPath and open index.html
# Or use command line:
xdg-open "/home/user/MyProject/CodeCoverage/Report/index.html"
```

---

### Example 3: Auto-Generate Report on Stop

Enable automatic report generation to skip manual report creation.

```bash
# 1. Enable coverage WITH auto-report generation
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{
  "enabled": true,
  "autoGenerateReport": true
}'

# Response:
{
  "content": [{
    "type": "text",
    "text": "Code coverage enabled. Auto-generate report: true"
  }],
  "isError": false,
  "_meta": {
    "enabled": true,
    "autoGenerateReport": true,
    "coverageResultsPath": "/home/user/MyProject/CodeCoverage"
  }
}

# 2. Start recording
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# 3. Run tests (manual or automated)

# 4. Stop recording WITHOUT specifying generateReport
# Report will be generated automatically because autoGenerateReport is true
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{}'

# Response (report generated automatically):
{
  "content": [{
    "type": "text",
    "text": "Code coverage recording stopped and report generated"
  }],
  "isError": false,
  "_meta": {
    "recording": false,
    "reportGenerated": true,
    "reportPath": "/home/user/MyProject/CodeCoverage/Report/index.html",
    "coverageResultsPath": "/home/user/MyProject/CodeCoverage"
  }
}

# Note: Unity Console will show:
# [CoverageService] Code coverage recording stopped
# [CoverageService] Auto-generating coverage report...
# [CoverageService] Coverage report generated successfully at: /path/to/report
```

---

### Example 4: Manual Report Generation

Generate coverage report without stopping recording (future feature example).

```bash
# Current workflow requires stopping recording to generate report
# This is a limitation of Unity's Code Coverage API

# Workaround: Stop recording, generate report, then restart recording
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'
# Check report...
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'
# Continue testing...

# Future version may support generating report without stopping recording
```

---

## Advanced Workflows

### Example 5: Multiple Test Sessions

Running multiple test sessions with separate coverage tracking.

```bash
# Session 1: Test core functionality
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# Run core tests in Test Runner
# Filter: Category = "Core"

mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'
# Report saved to: CodeCoverage/Report/index.html

# Rename report for session 1
mv CodeCoverage/Report CodeCoverage/Report-Core

# Session 2: Test UI functionality
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# Run UI tests in Test Runner
# Filter: Category = "UI"

mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'
# Report saved to: CodeCoverage/Report/index.html

# Rename report for session 2
mv CodeCoverage/Report CodeCoverage/Report-UI

# Now you have separate coverage reports:
# - CodeCoverage/Report-Core/index.html
# - CodeCoverage/Report-UI/index.html

# Compare coverage between different test categories
```

---

### Example 6: Coverage with Specific Test Categories

Using Unity Test Runner filters with coverage.

```bash
# Unity Test Runner supports filtering by category
# This example shows how to use coverage with filtered tests

# 1. Enable coverage
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'

# 2. Start recording
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# 3. In Unity Test Runner:
# Window → General → Test Runner
# Click the filter icon (funnel)
# Enter category filter: Category=Integration
# Click "Run Filtered"

# This runs only tests with [Category("Integration")] attribute:
/*
[Test]
[Category("Integration")]
public void TestDatabaseIntegration()
{
    // Test code...
}
*/

# 4. Stop recording and generate report
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'

# Report will show coverage only from Integration tests
# Useful for understanding which code paths integration tests cover
```

---

### Example 7: Coverage Comparison Over Time

Track coverage improvements across development.

```bash
# Create a coverage tracking directory
mkdir -p CoverageHistory

# Run 1: Initial baseline (Week 1)
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true, "autoGenerateReport": true}'
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'
# Run all tests...
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{}'

# Save baseline report
cp -r CodeCoverage/Report CoverageHistory/week-1-baseline
echo "Week 1: 45% coverage" >> CoverageHistory/log.txt

# Run 2: After adding new tests (Week 2)
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'
# Run all tests...
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{}'

# Save week 2 report
cp -r CodeCoverage/Report CoverageHistory/week-2
echo "Week 2: 52% coverage (+7%)" >> CoverageHistory/log.txt

# Run 3: After refactoring (Week 3)
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'
# Run all tests...
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{}'

# Save week 3 report
cp -r CodeCoverage/Report CoverageHistory/week-3
echo "Week 3: 58% coverage (+6%)" >> CoverageHistory/log.txt

# Compare reports:
# CoverageHistory/
# ├── week-1-baseline/
# ├── week-2/
# ├── week-3/
# └── log.txt

# Commit CoverageHistory to git for historical tracking
git add CoverageHistory
git commit -m "Update coverage reports - Week 3: 58% coverage"
```

---

## CI/CD Integration

### Example 8: GitHub Actions

Complete GitHub Actions workflow with code coverage.

**.github/workflows/unity-tests.yml:**

```yaml
name: Unity Tests with Code Coverage

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  test-coverage:
    name: Run Tests with Coverage
    runs-on: ubuntu-latest

    steps:
      # Checkout repository
      - name: Checkout Repository
        uses: actions/checkout@v3
        with:
          lfs: true

      # Cache Unity Library folder
      - name: Cache Unity Library
        uses: actions/cache@v3
        with:
          path: Library
          key: Library-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-

      # Activate Unity License
      - name: Activate Unity License
        uses: game-ci/unity-activate@v2
        with:
          unity-version: 2022.3.0f1
          unity-license: ${{ secrets.UNITY_LICENSE }}

      # Run EditMode tests with coverage
      - name: Run EditMode Tests
        uses: game-ci/unity-test-runner@v2
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
        with:
          unityVersion: 2022.3.0f1
          testMode: EditMode
          # Enable code coverage in Unity Test Runner
          coverageOptions: 'generateAdditionalMetrics;generateHtmlReport;generateBadgeReport'

      # Run PlayMode tests with coverage
      - name: Run PlayMode Tests
        uses: game-ci/unity-test-runner@v2
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
        with:
          unityVersion: 2022.3.0f1
          testMode: PlayMode
          coverageOptions: 'generateAdditionalMetrics;generateHtmlReport;generateBadgeReport'

      # Upload coverage report as artifact
      - name: Upload Coverage Report
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: coverage-report
          path: CodeCoverage/Report/

      # Upload coverage badge
      - name: Upload Coverage Badge
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: coverage-badge
          path: CodeCoverage/Report/badge_linecoverage.svg

      # Parse coverage percentage from report
      - name: Extract Coverage Percentage
        if: always()
        run: |
          if [ -f "CodeCoverage/Report/index.html" ]; then
            coverage=$(grep -oP 'Line Coverage: \K[0-9.]+' CodeCoverage/Report/index.html | head -1)
            echo "COVERAGE_PERCENTAGE=$coverage" >> $GITHUB_ENV
            echo "📊 Code Coverage: $coverage%"
          fi

      # Comment coverage on PR
      - name: Comment Coverage on PR
        if: github.event_name == 'pull_request'
        uses: actions/github-script@v6
        with:
          script: |
            const coverage = process.env.COVERAGE_PERCENTAGE || 'N/A';
            const comment = `## 📊 Code Coverage Report\n\n**Coverage:** ${coverage}%\n\n[View Full Report](https://github.com/${{ github.repository }}/actions/runs/${{ github.run_id }})`;
            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: comment
            });

      # Fail if coverage is below threshold
      - name: Check Coverage Threshold
        if: always()
        run: |
          threshold=70.0
          coverage=${COVERAGE_PERCENTAGE:-0}
          echo "Coverage: $coverage%, Threshold: $threshold%"
          if (( $(echo "$coverage < $threshold" | bc -l) )); then
            echo "❌ Coverage $coverage% is below threshold $threshold%"
            exit 1
          else
            echo "✅ Coverage $coverage% meets threshold $threshold%"
          fi
```

---

### Example 9: Jenkins Pipeline

Jenkins declarative pipeline with code coverage.

**Jenkinsfile:**

```groovy
pipeline {
    agent any

    environment {
        UNITY_PATH = '/Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/MacOS/Unity'
        PROJECT_PATH = "${WORKSPACE}"
        COVERAGE_THRESHOLD = '70.0'
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out repository...'
                checkout scm
            }
        }

        stage('Setup Unity MCP') {
            steps {
                echo 'Verifying Unity MCP installation...'
                sh '''
                    # Unity MCP should be in Packages/manifest.json
                    if grep -q "com.coplaydev.unity-mcp" Packages/manifest.json; then
                        echo "✅ Unity MCP found in manifest"
                    else
                        echo "❌ Unity MCP not found - adding to manifest"
                        exit 1
                    fi
                '''
            }
        }

        stage('Enable Code Coverage') {
            steps {
                echo 'Enabling code coverage...'
                sh '''
                    "$UNITY_PATH" -batchmode -quit \
                        -projectPath "$PROJECT_PATH" \
                        -logFile - \
                        -executeMethod TheOneStudio.UnityTestUtilitiesMCP.Editor.CI.EnableCoverageCommand
                '''
            }
        }

        stage('Run EditMode Tests') {
            steps {
                echo 'Running EditMode tests...'
                sh '''
                    "$UNITY_PATH" -batchmode -quit \
                        -projectPath "$PROJECT_PATH" \
                        -logFile - \
                        -runTests -testPlatform EditMode \
                        -testResults "$PROJECT_PATH/TestResults-EditMode.xml"
                '''
            }
        }

        stage('Run PlayMode Tests') {
            steps {
                echo 'Running PlayMode tests...'
                sh '''
                    "$UNITY_PATH" -batchmode -quit \
                        -projectPath "$PROJECT_PATH" \
                        -logFile - \
                        -runTests -testPlatform PlayMode \
                        -testResults "$PROJECT_PATH/TestResults-PlayMode.xml"
                '''
            }
        }

        stage('Generate Coverage Report') {
            steps {
                echo 'Generating coverage report...'
                sh '''
                    "$UNITY_PATH" -batchmode -quit \
                        -projectPath "$PROJECT_PATH" \
                        -logFile - \
                        -executeMethod TheOneStudio.UnityTestUtilitiesMCP.Editor.CI.GenerateCoverageReportCommand
                '''
            }
        }

        stage('Analyze Coverage') {
            steps {
                echo 'Analyzing coverage results...'
                script {
                    // Parse coverage percentage from HTML report
                    def coverageHtml = readFile('CodeCoverage/Report/index.html')
                    def matcher = coverageHtml =~ /Line Coverage: ([0-9.]+)/
                    if (matcher) {
                        env.COVERAGE_PERCENTAGE = matcher[0][1]
                        echo "📊 Code Coverage: ${env.COVERAGE_PERCENTAGE}%"
                    } else {
                        env.COVERAGE_PERCENTAGE = '0.0'
                        echo "⚠️ Could not extract coverage percentage"
                    }

                    // Check threshold
                    def coverage = env.COVERAGE_PERCENTAGE.toDouble()
                    def threshold = env.COVERAGE_THRESHOLD.toDouble()

                    if (coverage < threshold) {
                        error("❌ Coverage ${coverage}% is below threshold ${threshold}%")
                    } else {
                        echo "✅ Coverage ${coverage}% meets threshold ${threshold}%"
                    }
                }
            }
        }

        stage('Publish Reports') {
            steps {
                echo 'Publishing coverage report...'
                publishHTML([
                    reportDir: 'CodeCoverage/Report',
                    reportFiles: 'index.html',
                    reportName: 'Code Coverage Report',
                    keepAll: true,
                    alwaysLinkToLastBuild: true
                ])

                // Publish test results
                junit testResults: 'TestResults-*.xml', allowEmptyResults: false
            }
        }
    }

    post {
        always {
            echo 'Cleaning up...'
            // Archive coverage report
            archiveArtifacts artifacts: 'CodeCoverage/Report/**/*', allowEmptyArchive: true
            archiveArtifacts artifacts: 'TestResults-*.xml', allowEmptyArchive: true
        }

        success {
            echo '✅ Build and tests completed successfully'
            // Send success notification
            script {
                def coverage = env.COVERAGE_PERCENTAGE ?: 'N/A'
                slackSend(
                    color: 'good',
                    message: "✅ Unity Tests Passed - Coverage: ${coverage}%\nJob: ${env.JOB_NAME} [${env.BUILD_NUMBER}]"
                )
            }
        }

        failure {
            echo '❌ Build or tests failed'
            // Send failure notification
            script {
                def coverage = env.COVERAGE_PERCENTAGE ?: 'N/A'
                slackSend(
                    color: 'danger',
                    message: "❌ Unity Tests Failed - Coverage: ${coverage}%\nJob: ${env.JOB_NAME} [${env.BUILD_NUMBER}]"
                )
            }
        }
    }
}
```

---

### Example 10: GitLab CI

GitLab CI pipeline with coverage reporting.

**.gitlab-ci.yml:**

```yaml
# GitLab CI configuration for Unity Tests with Code Coverage

variables:
  UNITY_VERSION: "2022.3.0f1"
  UNITY_LICENSE_FILE: $CI_PROJECT_DIR/.unity-license
  COVERAGE_THRESHOLD: "70.0"

# Cache Unity Library folder
cache:
  paths:
    - Library/

stages:
  - setup
  - test
  - report
  - deploy

# Setup Unity License
setup-license:
  stage: setup
  script:
    - echo "$UNITY_LICENSE" | base64 -d > $UNITY_LICENSE_FILE
  artifacts:
    paths:
      - .unity-license
    expire_in: 1 hour

# Run EditMode tests with coverage
test-editmode:
  stage: test
  image: unityci/editor:$UNITY_VERSION
  dependencies:
    - setup-license
  script:
    # Enable code coverage
    - echo "Enabling code coverage..."
    - unity-editor -batchmode -quit -projectPath . -logFile - -executeMethod TheOneStudio.UnityTestUtilitiesMCP.Editor.CI.EnableCoverageCommand

    # Run EditMode tests
    - echo "Running EditMode tests..."
    - unity-editor -batchmode -quit -projectPath . -logFile - -runTests -testPlatform EditMode -testResults TestResults-EditMode.xml

  artifacts:
    paths:
      - TestResults-EditMode.xml
      - CodeCoverage/
    reports:
      junit: TestResults-EditMode.xml
    expire_in: 1 week

# Run PlayMode tests with coverage
test-playmode:
  stage: test
  image: unityci/editor:$UNITY_VERSION
  dependencies:
    - setup-license
  script:
    # Enable code coverage
    - echo "Enabling code coverage..."
    - unity-editor -batchmode -quit -projectPath . -logFile - -executeMethod TheOneStudio.UnityTestUtilitiesMCP.Editor.CI.EnableCoverageCommand

    # Run PlayMode tests
    - echo "Running PlayMode tests..."
    - unity-editor -batchmode -quit -projectPath . -logFile - -runTests -testPlatform PlayMode -testResults TestResults-PlayMode.xml

  artifacts:
    paths:
      - TestResults-PlayMode.xml
      - CodeCoverage/
    reports:
      junit: TestResults-PlayMode.xml
    expire_in: 1 week

# Generate coverage report
generate-coverage-report:
  stage: report
  image: unityci/editor:$UNITY_VERSION
  dependencies:
    - setup-license
    - test-editmode
    - test-playmode
  script:
    # Generate HTML coverage report
    - echo "Generating coverage report..."
    - unity-editor -batchmode -quit -projectPath . -logFile - -executeMethod TheOneStudio.UnityTestUtilitiesMCP.Editor.CI.GenerateCoverageReportCommand

    # Extract coverage percentage
    - |
      if [ -f "CodeCoverage/Report/index.html" ]; then
        coverage=$(grep -oP 'Line Coverage: \K[0-9.]+' CodeCoverage/Report/index.html | head -1)
        echo "COVERAGE_PERCENTAGE=$coverage" >> coverage.env
        echo "📊 Code Coverage: $coverage%"
      else
        echo "COVERAGE_PERCENTAGE=0.0" >> coverage.env
        echo "⚠️ Coverage report not found"
      fi

    # Check coverage threshold
    - |
      source coverage.env
      threshold=$COVERAGE_THRESHOLD
      echo "Coverage: $COVERAGE_PERCENTAGE%, Threshold: $threshold%"
      if (( $(echo "$COVERAGE_PERCENTAGE < $threshold" | bc -l) )); then
        echo "❌ Coverage $COVERAGE_PERCENTAGE% is below threshold $threshold%"
        exit 1
      else
        echo "✅ Coverage $COVERAGE_PERCENTAGE% meets threshold $threshold%"
      fi

  artifacts:
    paths:
      - CodeCoverage/Report/
    reports:
      coverage_report:
        coverage_format: cobertura
        path: CodeCoverage/Report/Cobertura.xml
    expire_in: 1 month
  coverage: '/Line Coverage: (\d+\.\d+)/'

# Deploy coverage report to GitLab Pages
pages:
  stage: deploy
  dependencies:
    - generate-coverage-report
  script:
    - mkdir -p public
    - cp -r CodeCoverage/Report/* public/
  artifacts:
    paths:
      - public
  only:
    - main
```

---

### Example 11: Unity Cloud Build

Unity Cloud Build post-build script for coverage.

**CloudBuild/PostBuild.cs:**

```csharp
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;

public static class CloudBuildPostBuild
{
    /// <summary>
    /// Unity Cloud Build post-build script.
    /// Runs tests with code coverage and generates report.
    /// </summary>
    public static void OnPostBuild(string exportPath)
    {
        Debug.Log("[CloudBuild] Starting post-build code coverage...");

        try
        {
            // Enable code coverage
            var coverageService = new CoverageService();
            coverageService.EnableCoverage(true, autoGenerateReport: true);
            Debug.Log("[CloudBuild] Code coverage enabled");

            // Start recording
            coverageService.StartRecording();
            Debug.Log("[CloudBuild] Coverage recording started");

            // Run EditMode tests
            Debug.Log("[CloudBuild] Running EditMode tests...");
            RunTests("EditMode");

            // Run PlayMode tests
            Debug.Log("[CloudBuild] Running PlayMode tests...");
            RunTests("PlayMode");

            // Stop recording and generate report
            coverageService.StopRecording();
            Debug.Log("[CloudBuild] Coverage recording stopped");

            // Wait for report generation
            var reportTask = coverageService.GenerateReportAsync();
            reportTask.Wait();
            string reportPath = reportTask.Result;

            Debug.Log($"[CloudBuild] Coverage report generated: {reportPath}");

            // Copy report to Cloud Build artifacts directory
            string cloudBuildPath = Environment.GetEnvironmentVariable("CLOUD_BUILD_TARGET_PATH");
            if (!string.IsNullOrEmpty(cloudBuildPath))
            {
                string artifactsPath = Path.Combine(cloudBuildPath, "CoverageReport");
                CopyDirectory(Path.GetDirectoryName(reportPath), artifactsPath);
                Debug.Log($"[CloudBuild] Coverage report copied to: {artifactsPath}");
            }

            Debug.Log("[CloudBuild] Post-build coverage completed successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CloudBuild] Post-build coverage failed: {ex.Message}");
            // Don't fail the build on coverage errors
        }
    }

    private static void RunTests(string testMode)
    {
        // Unity Cloud Build doesn't support running tests during build
        // This is a placeholder for future functionality
        Debug.LogWarning($"[CloudBuild] Test execution not supported in Cloud Build. Use Unity Test Runner locally.");
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = file.Substring(sourceDir.Length + 1);
            string destPath = Path.Combine(destDir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            File.Copy(file, destPath, true);
        }
    }
}
```

---

## Claude Code Integration

### Example 12: Interactive Coverage Session

Using Claude Code CLI for interactive testing with coverage.

```
# User starts conversation with Claude Code
User: "I need to run tests with code coverage for my Unity project"

Claude: I'll help you set up and run code coverage. Let me first check the current coverage settings.

# Claude calls get_coverage_settings
{
  "coveragePackageInstalled": true,
  "enabled": false,
  "autoGenerateReport": false,
  "coverageResultsPath": "/home/user/MyProject/CodeCoverage",
  "recording": false
}

Claude: Code coverage package is installed but currently disabled. I'll enable it now.

# Claude calls enable_code_coverage
{"enabled": true, "autoGenerateReport": true}

Claude: Coverage enabled with auto-report generation. Starting recording...

# Claude calls start_coverage_recording
{}

Claude: Coverage recording started. Please run your tests in Unity Test Runner (Window → General → Test Runner → Run All), then let me know when they're complete.

User: "Tests are done, 132 tests passed"

Claude: Great! I'll stop the recording and generate the coverage report.

# Claude calls stop_coverage_recording
{}

Claude: Coverage recording stopped and report generated successfully!

Report location: /home/user/MyProject/CodeCoverage/Report/index.html

Would you like me to open the report in your browser?

User: "Yes please"

# Claude executes bash command
xdg-open "/home/user/MyProject/CodeCoverage/Report/index.html"

Claude: Report opened! The HTML report shows:
- Overall line coverage percentage
- Per-file coverage breakdown
- Line-by-line coverage annotations

Is there anything specific you'd like to analyze in the coverage data?
```

---

### Example 13: Automated Test-and-Coverage

Claude Code automatically runs tests with coverage.

```
User: "Run all tests with coverage and show me any failures"

Claude: I'll enable coverage, run the tests, and analyze the results. Let me start:

# Step 1: Enable coverage
# Claude calls enable_code_coverage
{"enabled": true, "autoGenerateReport": true}

# Step 2: Start recording
# Claude calls start_coverage_recording
{}

# Step 3: Prompt user
Claude: Coverage recording started. Since Unity Test Runner requires manual interaction, please:
1. Open Unity Test Runner (Window → General → Test Runner)
2. Click "Run All"
3. Wait for tests to complete
4. Come back here and type "done"

User: "done"

# Step 4: Stop recording
# Claude calls stop_coverage_recording
{"generateReport": true}

# Step 5: Analyze test results
# Claude reads TestResults.xml (future feature)
Claude: Test results:
- Total: 132 tests
- Passed: 130 tests
- Failed: 2 tests
- Skipped: 0 tests

Failed tests:
1. PlayerControllerTests.TestJumpHeight
   - Expected: 5.0, Actual: 4.8
   - File: Assets/Tests/PlayerControllerTests.cs:45

2. InventoryTests.TestMaxCapacity
   - Expected: 100, Actual: 99
   - File: Assets/Tests/InventoryTests.cs:78

Coverage report generated: /home/user/MyProject/CodeCoverage/Report/index.html

Overall coverage: 73.2%

Would you like me to help fix these failing tests?
```

---

## Unity Editor Scripts

### Example 14: Custom Menu Item

Create custom Unity Editor menu item for coverage.

**Editor/CoverageMenuItems.cs:**

```csharp
using UnityEditor;
using UnityEngine;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;

public static class CoverageMenuItems
{
    private static readonly ICoverageService _coverageService = new CoverageService();

    [MenuItem("Tools/Code Coverage/Enable Coverage", priority = 100)]
    public static void EnableCoverage()
    {
        try
        {
            _coverageService.EnableCoverage(true, autoGenerateReport: true);
            Debug.Log("✅ Code coverage enabled");
            EditorUtility.DisplayDialog(
                "Coverage Enabled",
                "Code coverage has been enabled with auto-report generation.",
                "OK"
            );
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Failed to enable coverage: {ex.Message}");
            EditorUtility.DisplayDialog(
                "Coverage Error",
                $"Failed to enable coverage:\n{ex.Message}",
                "OK"
            );
        }
    }

    [MenuItem("Tools/Code Coverage/Start Recording", priority = 101)]
    public static void StartRecording()
    {
        try
        {
            if (!_coverageService.IsCoverageEnabled())
            {
                EditorUtility.DisplayDialog(
                    "Coverage Not Enabled",
                    "Please enable coverage first using:\nTools → Code Coverage → Enable Coverage",
                    "OK"
                );
                return;
            }

            _coverageService.StartRecording();
            Debug.Log("✅ Coverage recording started");
            EditorUtility.DisplayDialog(
                "Recording Started",
                "Code coverage recording has started.\nRun your tests and then stop recording.",
                "OK"
            );
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Failed to start recording: {ex.Message}");
            EditorUtility.DisplayDialog(
                "Recording Error",
                $"Failed to start recording:\n{ex.Message}",
                "OK"
            );
        }
    }

    [MenuItem("Tools/Code Coverage/Stop Recording and Generate Report", priority = 102)]
    public static async void StopRecordingAndGenerateReport()
    {
        try
        {
            _coverageService.StopRecording();
            Debug.Log("✅ Coverage recording stopped");

            EditorUtility.DisplayProgressBar("Generating Coverage Report", "Please wait...", 0.5f);

            string reportPath = await _coverageService.GenerateReportAsync();

            EditorUtility.ClearProgressBar();

            Debug.Log($"✅ Coverage report generated: {reportPath}");

            bool openReport = EditorUtility.DisplayDialog(
                "Report Generated",
                $"Coverage report generated successfully!\n\nLocation: {reportPath}",
                "Open Report",
                "Close"
            );

            if (openReport)
            {
                Application.OpenURL($"file://{reportPath}");
            }
        }
        catch (System.Exception ex)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError($"❌ Failed to generate report: {ex.Message}");
            EditorUtility.DisplayDialog(
                "Report Error",
                $"Failed to generate coverage report:\n{ex.Message}",
                "OK"
            );
        }
    }

    [MenuItem("Tools/Code Coverage/Open Coverage Report", priority = 103)]
    public static void OpenCoverageReport()
    {
        string reportPath = _coverageService.GetCoverageResultsPath();
        if (string.IsNullOrEmpty(reportPath))
        {
            EditorUtility.DisplayDialog(
                "Report Not Found",
                "Coverage report path not available.\nMake sure Code Coverage package is installed.",
                "OK"
            );
            return;
        }

        string htmlPath = System.IO.Path.Combine(reportPath, "Report", "index.html");
        if (!System.IO.File.Exists(htmlPath))
        {
            EditorUtility.DisplayDialog(
                "Report Not Found",
                "Coverage report has not been generated yet.\nRun tests with coverage and generate report first.",
                "OK"
            );
            return;
        }

        Application.OpenURL($"file://{htmlPath}");
    }

    [MenuItem("Tools/Code Coverage/Check Coverage Status", priority = 200)]
    public static void CheckCoverageStatus()
    {
        bool isEnabled = _coverageService.IsCoverageEnabled();
        string resultsPath = _coverageService.GetCoverageResultsPath();

        string message = $"Coverage Enabled: {isEnabled}\n" +
                        $"Results Path: {resultsPath ?? "N/A"}";

        Debug.Log($"[Coverage Status] {message}");
        EditorUtility.DisplayDialog("Coverage Status", message, "OK");
    }
}
```

---

### Example 15: Automated Pre-Commit Hook

Run tests with coverage before committing.

**Editor/PreCommitCoverageCheck.cs:**

```csharp
using UnityEditor;
using UnityEngine;
using TheOneStudio.UnityTestUtilitiesMCP.Editor.Services;
using System.Threading.Tasks;

[InitializeOnLoad]
public static class PreCommitCoverageCheck
{
    private static readonly ICoverageService _coverageService = new CoverageService();
    private const float COVERAGE_THRESHOLD = 70.0f;

    static PreCommitCoverageCheck()
    {
        // Register pre-commit hook
        // This is a simplified example - actual git hook would need to be set up separately
        EditorApplication.update += CheckForPreCommit;
    }

    private static void CheckForPreCommit()
    {
        // Check if .git/COMMIT_EDITMSG exists (commit in progress)
        string commitMsgPath = System.IO.Path.Combine(
            System.IO.Directory.GetCurrentDirectory(),
            ".git", "COMMIT_EDITMSG"
        );

        if (System.IO.File.Exists(commitMsgPath))
        {
            // Read the last modification time
            var lastModified = System.IO.File.GetLastWriteTime(commitMsgPath);
            var now = System.DateTime.Now;

            // If commit message was just created (within last 5 seconds)
            if ((now - lastModified).TotalSeconds < 5)
            {
                // Run coverage check
                _ = RunCoverageCheckAsync();
            }
        }
    }

    private static async Task RunCoverageCheckAsync()
    {
        try
        {
            Debug.Log("[PreCommit] Running coverage check...");

            // Enable coverage
            _coverageService.EnableCoverage(true, autoGenerateReport: true);

            // Start recording
            _coverageService.StartRecording();

            // Run tests (this would need actual test runner integration)
            Debug.LogWarning("[PreCommit] Automated test execution not implemented - please run tests manually");

            // For now, just check if previous coverage report exists
            string resultsPath = _coverageService.GetCoverageResultsPath();
            string reportPath = System.IO.Path.Combine(resultsPath, "Report", "index.html");

            if (!System.IO.File.Exists(reportPath))
            {
                Debug.LogWarning("[PreCommit] No coverage report found - skipping check");
                return;
            }

            // Parse coverage percentage from report
            string htmlContent = System.IO.File.ReadAllText(reportPath);
            var match = System.Text.RegularExpressions.Regex.Match(
                htmlContent,
                @"Line Coverage: ([0-9.]+)"
            );

            if (match.Success)
            {
                float coverage = float.Parse(match.Groups[1].Value);
                Debug.Log($"[PreCommit] Current coverage: {coverage}%");

                if (coverage < COVERAGE_THRESHOLD)
                {
                    bool proceed = EditorUtility.DisplayDialog(
                        "Low Code Coverage",
                        $"Current coverage ({coverage}%) is below threshold ({COVERAGE_THRESHOLD}%).\n\n" +
                        "Proceeding with commit may reduce overall code quality.",
                        "Commit Anyway",
                        "Cancel Commit"
                    );

                    if (!proceed)
                    {
                        Debug.LogWarning("[PreCommit] Commit cancelled due to low coverage");
                        // Would need to abort git commit here
                    }
                }
                else
                {
                    Debug.Log($"[PreCommit] ✅ Coverage {coverage}% meets threshold {COVERAGE_THRESHOLD}%");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PreCommit] Coverage check failed: {ex.Message}");
        }
    }
}
```

---

## Troubleshooting Examples

### Example 16: Diagnosing Coverage Issues

Systematic diagnosis of coverage problems.

```bash
# Step 1: Check package installation
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# If coveragePackageInstalled is false:
# → Install com.unity.testtools.codecoverage via Package Manager

# Step 2: Check if coverage is enabled
# Response should show:
{
  "enabled": false,  # ← Need to enable
  "recording": false
}

# Step 3: Enable coverage
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'

# If error: "Code Coverage not supported: ..."
# → Package not installed correctly
# → Restart Unity Editor
# → Reimport package

# Step 4: Try starting recording
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# If error: "Code coverage is not enabled..."
# → Coverage enable command failed silently
# → Check Unity Console for errors
# → Verify EditorPrefs: UnityTestUtilitiesMCP.Coverage.Enabled

# Step 5: Check Unity Console logs
# Should see:
# [CoverageService] Code coverage enabled. Auto-generate report: false
# [CoverageService] Code coverage recording started

# Step 6: Run a simple test
# Window → General → Test Runner
# Run a single test to verify coverage is capturing data

# Step 7: Stop recording and check results
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'

# Step 8: Verify report exists
ls -la /path/to/project/CodeCoverage/Report/index.html

# If report doesn't exist:
# → Check CodeCoverage/ directory for .coverage files
# → If .coverage files exist, report generation failed
# → If no .coverage files, recording didn't capture data
# → Verify tests actually executed during recording session

# Step 9: Check Unity version compatibility
# Code Coverage requires Unity 2020.1+
# Unity Test Utilities MCP requires Unity 2022.3+
```

---

### Example 17: Verifying Package Installation

Complete verification of package setup.

```bash
# 1. Check Unity MCP is installed
cat Packages/manifest.json | grep "unity-mcp"
# Should see: "com.coplaydev.unity-mcp": "6.0.0"

# 2. Check Unity Test Utilities MCP is installed
cat Packages/manifest.json | grep "unity-test-utilities-mcp"
# Should see: "com.the1studio.unity-test-utilities-mcp": "..."

# 3. Check Code Coverage package (optional)
cat Packages/manifest.json | grep "codecoverage"
# Should see: "com.unity.testtools.codecoverage": "1.2.0"

# 4. Verify Unity MCP connection
# In Unity Editor:
# Window → MCP for Unity
# Should show "Connected" status

# 5. Test MCP tool availability
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# If command not recognized:
# → Unity MCP not connected
# → Restart Unity Editor
# → Click "Auto-Setup" in MCP for Unity window

# 6. Check package assembly definitions
ls -la Packages/com.the1studio.unity-test-utilities-mcp/Editor/*.asmdef
# Should see: UnityTestUtilitiesMCP.Editor.asmdef

# 7. Verify references in assembly definition
cat Packages/com.the1studio.unity-test-utilities-mcp/Editor/*.asmdef
# Should include references:
# - MCPForUnity.Editor
# - UnityEditor.TestRunner
# - UnityEngine.TestRunner

# 8. Check for compilation errors
# Open Unity Editor Console
# Should have no errors related to Unity Test Utilities MCP

# 9. Test each MCP tool
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{}'
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# If all commands succeed → Installation is correct
# If any command fails → Check error message and Unity Console
```

---

## Summary

This examples documentation covers:

- **Basic Workflows**: Getting started with code coverage
- **Advanced Workflows**: Multiple sessions, category filtering, historical tracking
- **CI/CD Integration**: GitHub Actions, Jenkins, GitLab CI, Unity Cloud Build
- **Claude Code Integration**: Interactive sessions, automated workflows
- **Unity Editor Scripts**: Custom menu items, pre-commit hooks
- **Troubleshooting**: Systematic diagnosis and verification

For more information, see:
- [API Reference](api-reference.md) - Complete API documentation
- [Index](index.md) - Documentation hub
- [GitHub Repository](https://github.com/The1Studio/UnityTestUtilitiesMCP)

---

**Navigation**: [← API Reference](api-reference.md) | [Back to Index](index.md) | [GitHub](https://github.com/The1Studio/UnityTestUtilitiesMCP)
