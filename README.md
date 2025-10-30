# Unity Test Utilities for MCP

Advanced testing utilities for Unity projects using MCP (Model Context Protocol). This package extends [com.coplaydev.unity-mcp](https://github.com/coplaydev/unity-mcp) with additional testing capabilities including code coverage integration, advanced test filtering, and test result analysis.

## Features

- **Code Coverage Integration**
  - Enable/disable Unity Code Coverage recording
  - Start and stop coverage recording programmatically
  - Automatic HTML report generation
  - Query coverage settings and status

- **Advanced Test Filtering** (Coming Soon)
  - Filter tests by pattern, category, or assembly
  - Run specific test subsets via MCP commands

- **Test Result Analysis** (Coming Soon)
  - Parse Unity TestResults.xml files
  - Extract test statistics and failure information
  - Analyze test execution data programmatically

- **CI/CD Integration**
  - Designed for automated testing workflows
  - Compatible with GitHub Actions, Jenkins, GitLab CI
  - Supports batch mode execution

## Installation

### Prerequisites

- Unity 2022.3 or later
- [com.coplaydev.unity-mcp](https://github.com/coplaydev/unity-mcp) version 6.0.0 or later
- Unity Test Framework 1.4.0 or later
- **Optional:** [com.unity.testtools.codecoverage](https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@latest) for code coverage features

### Via Package Manager (Git URL)

1. Open Unity Package Manager (`Window → Package Manager`)
2. Click the `+` button in the top-left corner
3. Select `Add package from git URL...`
4. Enter: `https://github.com/The1Studio/UnityTestUtilitiesMCP.git`
5. Click `Add`

### Via manifest.json

Add the following to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.the1studio.unity-test-utilities-mcp": "https://github.com/The1Studio/UnityTestUtilitiesMCP.git",
    "com.coplaydev.unity-mcp": "6.0.0",
    "com.unity.test-framework": "1.4.0"
  }
}
```

### As Git Submodule (For Package Development)

```bash
# Clone into your Unity project's Packages directory
cd Packages/
git submodule add https://github.com/The1Studio/UnityTestUtilitiesMCP.git

# Update submodules
git submodule update --init --recursive
```

### Installing Code Coverage Package (Optional)

For code coverage features, install the Unity Code Coverage package:

```bash
# Via Package Manager
Window → Package Manager → Unity Registry → Code Coverage → Install

# Or add to manifest.json
"com.unity.testtools.codecoverage": "1.2.0"
```

## Quick Start

### Setting Up Unity MCP

1. Ensure Unity MCP is configured in your Unity Editor (`Window → MCP for Unity`)
2. Click `Auto-Setup` if not already connected
3. Verify "Connected" status appears

### Using Code Coverage

```bash
# Enable code coverage with auto-report generation
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true, "autoGenerateReport": true}'

# Start recording coverage
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# Run your tests in Unity Editor (Window → General → Test Runner)

# Stop recording and generate report
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'

# Query coverage settings
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'
```

### Example Workflow with Claude Code

When using Claude Code CLI with Unity MCP:

```
User: "Enable code coverage and run tests"

Claude:
1. Calls enable_code_coverage tool with enabled=true
2. Calls start_coverage_recording tool
3. Prompts user to run tests in Unity Test Runner
4. Calls stop_coverage_recording with generateReport=true
5. Reports coverage results path
```

## API Reference

### MCP Tools

#### `enable_code_coverage`

Enables or disables Unity Code Coverage recording.

**Parameters:**
- `enabled` (boolean, required): True to enable coverage, false to disable
- `autoGenerateReport` (boolean, optional): If true, automatically generate HTML report after recording stops. Default: false

**Returns:**
```json
{
  "enabled": true,
  "autoGenerateReport": true,
  "coverageResultsPath": "/path/to/project/CodeCoverage"
}
```

**Example:**
```json
{"enabled": true, "autoGenerateReport": true}
```

#### `start_coverage_recording`

Starts recording code coverage data. Coverage must be enabled first.

**Parameters:** None

**Returns:**
```json
{
  "recording": true,
  "coverageResultsPath": "/path/to/project/CodeCoverage"
}
```

**Example:**
```json
{}
```

#### `stop_coverage_recording`

Stops recording code coverage data and optionally generates an HTML report.

**Parameters:**
- `generateReport` (boolean, optional): If true, generate HTML report after stopping. Default: false

**Returns:**
```json
{
  "recording": false,
  "reportGenerated": true,
  "reportPath": "/path/to/project/CodeCoverage/Report/index.html",
  "coverageResultsPath": "/path/to/project/CodeCoverage"
}
```

**Example:**
```json
{"generateReport": true}
```

### MCP Resources

#### `get_coverage_settings`

Queries current code coverage settings and status.

**Parameters:** None

**Returns:**
```json
{
  "coveragePackageInstalled": true,
  "enabled": true,
  "autoGenerateReport": true,
  "coverageResultsPath": "/path/to/project/CodeCoverage",
  "recording": true
}
```

**Example:**
```json
{}
```

## Usage Examples

### Example 1: Basic Coverage Workflow

```bash
# Step 1: Check coverage availability
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# Step 2: Enable coverage
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'

# Step 3: Start recording
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# Step 4: Run tests manually in Unity Test Runner
# Window → General → Test Runner → Run All

# Step 5: Stop recording and generate report
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'

# Step 6: Open report in browser
# File located at: <project>/CodeCoverage/Report/index.html
```

### Example 2: Auto-Generate Report on Stop

```bash
# Enable coverage with auto-report generation
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true, "autoGenerateReport": true}'

# Start recording
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# Run tests...

# Stop recording (report automatically generated)
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{}'
```

### Example 3: Checking Coverage Status

```bash
# Query current settings
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# Example response:
{
  "coveragePackageInstalled": true,
  "enabled": true,
  "autoGenerateReport": false,
  "coverageResultsPath": "/home/user/MyProject/CodeCoverage",
  "recording": false
}
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Unity Tests with Coverage

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Unity
        uses: game-ci/unity-builder@v2
        with:
          unityVersion: 2022.3.0f1

      - name: Run Tests with Coverage
        run: |
          # Enable code coverage
          unity-editor -batchmode -quit -projectPath . \
            -executeMethod UnityTestUtilitiesMCP.EnableCoverage

          # Run tests
          unity-editor -batchmode -quit -projectPath . \
            -runTests -testPlatform EditMode

          # Generate coverage report
          unity-editor -batchmode -quit -projectPath . \
            -executeMethod UnityTestUtilitiesMCP.GenerateCoverageReport

      - name: Upload Coverage Report
        uses: actions/upload-artifact@v3
        with:
          name: coverage-report
          path: CodeCoverage/Report/
```

### Jenkins Pipeline Example

```groovy
pipeline {
    agent any

    stages {
        stage('Setup') {
            steps {
                checkout scm
            }
        }

        stage('Enable Coverage') {
            steps {
                sh '''
                    /opt/Unity/Editor/Unity -batchmode -quit \
                        -projectPath . \
                        -executeMethod UnityTestUtilitiesMCP.EnableCoverage
                '''
            }
        }

        stage('Run Tests') {
            steps {
                sh '''
                    /opt/Unity/Editor/Unity -batchmode -quit \
                        -projectPath . \
                        -runTests -testPlatform EditMode
                '''
            }
        }

        stage('Generate Coverage Report') {
            steps {
                sh '''
                    /opt/Unity/Editor/Unity -batchmode -quit \
                        -projectPath . \
                        -executeMethod UnityTestUtilitiesMCP.GenerateCoverageReport
                '''
            }
        }

        stage('Publish Coverage') {
            steps {
                publishHTML([
                    reportDir: 'CodeCoverage/Report',
                    reportFiles: 'index.html',
                    reportName: 'Code Coverage Report'
                ])
            }
        }
    }
}
```

## Troubleshooting

### Code Coverage Package Not Installed

**Error:** "Code Coverage package (com.unity.testtools.codecoverage) is not installed."

**Solution:** Install the Code Coverage package via Package Manager:
1. Open `Window → Package Manager`
2. Select `Unity Registry` from the dropdown
3. Search for "Code Coverage"
4. Click `Install`

### Coverage Not Recording

**Issue:** Coverage recording doesn't capture any data.

**Solutions:**
1. Ensure coverage is enabled: `mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'`
2. Verify recording started before running tests
3. Check Unity Console for error messages
4. Restart Unity Editor and try again

### Report Generation Fails

**Issue:** HTML report is not generated.

**Solutions:**
1. Check that coverage data exists in `CodeCoverage/` directory
2. Ensure tests were run after starting recording
3. Verify Unity has write permissions to project directory
4. Check Unity Console for detailed error messages

### MCP Connection Issues

**Issue:** MCP tools not available or not responding.

**Solutions:**
1. Verify Unity MCP is installed and configured (`Window → MCP for Unity`)
2. Click `Auto-Setup` in MCP for Unity window
3. Restart Unity Editor
4. Check that `com.coplaydev.unity-mcp` version 6.0.0+ is installed

### Unity Batch Mode Conflicts

**Important:** Never run Unity in batch mode while Unity Editor is open when using Unity MCP. This will cause connection conflicts and may corrupt project state.

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Setup

```bash
# Clone the repository
git clone https://github.com/The1Studio/UnityTestUtilitiesMCP.git

# Create a Unity project for testing
mkdir TestProject
cd TestProject
unity-hub create --name TestProject --version 2022.3.0f1

# Link package for local development
cd Packages
ln -s /path/to/UnityTestUtilitiesMCP com.the1studio.unity-test-utilities-mcp
```

### Code Style

- Follow Unity C# coding conventions
- Use PascalCase for classes, methods, and properties
- Use camelCase for private fields
- Add XML documentation comments for public APIs
- Include error handling and logging

### Testing

- Add unit tests for new features in `Tests/` directory
- Ensure all tests pass before submitting PR
- Test with Unity MCP integration

## License

MIT License

Copyright (c) 2024 The1Studio

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Links

- [GitHub Repository](https://github.com/The1Studio/UnityTestUtilitiesMCP)
- [Unity MCP](https://github.com/coplaydev/unity-mcp)
- [Unity Code Coverage Documentation](https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@latest)
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [The1Studio](https://github.com/The1Studio)

## Support

For issues, questions, or feature requests:
- Open an issue on [GitHub](https://github.com/The1Studio/UnityTestUtilitiesMCP/issues)
- Contact The1Studio team

## Roadmap

### Version 1.0.0 (Current)
- ✅ Code coverage enable/disable
- ✅ Coverage recording start/stop
- ✅ HTML report generation
- ✅ Coverage settings query

### Version 1.1.0 (Planned)
- Advanced test filtering by pattern
- Test filtering by category
- Test filtering by assembly
- Run filtered tests via MCP

### Version 1.2.0 (Planned)
- Parse TestResults.xml files
- Extract test statistics
- Analyze test failures
- Test result query API

### Version 2.0.0 (Future)
- Batch test operations
- Custom report templates
- Coverage threshold validation
- Test execution history tracking
