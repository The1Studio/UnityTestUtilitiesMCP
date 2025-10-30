# Unity Test Utilities for MCP - Documentation

Welcome to the Unity Test Utilities for MCP documentation. This package extends Unity MCP with advanced testing capabilities.

## Documentation Overview

This documentation hub provides comprehensive guidance for using Unity Test Utilities for MCP in your Unity projects.

### Quick Links

- [API Reference](api-reference.md) - Complete API documentation for all MCP tools and resources
- [Examples](examples.md) - Detailed usage examples and workflows
- [GitHub Repository](https://github.com/The1Studio/UnityTestUtilitiesMCP)
- [Main README](../README.md) - Installation and quick start guide

## What is Unity Test Utilities for MCP?

Unity Test Utilities for MCP is a Unity Package Manager (UPM) package that provides advanced testing capabilities through the Model Context Protocol (MCP). It extends [com.coplaydev.unity-mcp](https://github.com/coplaydev/unity-mcp) with:

- **Code Coverage Integration**: Control Unity Code Coverage recording and report generation
- **Advanced Test Filtering**: Filter and run specific test subsets (coming soon)
- **Test Result Analysis**: Parse and analyze Unity test results (coming soon)

## Key Features

### Code Coverage (Available Now)

Enable, control, and monitor Unity Code Coverage through MCP tools:

- `enable_code_coverage` - Enable/disable coverage recording
- `start_coverage_recording` - Start recording coverage data
- `stop_coverage_recording` - Stop recording and optionally generate reports
- `get_coverage_settings` - Query current coverage settings and status

### Advanced Test Filtering (Coming Soon)

Run specific test subsets based on:
- Test name patterns (wildcards, regex)
- Test categories and tags
- Assembly names
- Custom filters

### Test Result Analysis (Coming Soon)

Programmatically analyze test execution:
- Parse TestResults.xml files
- Extract test statistics (pass/fail/skip counts)
- Identify failing tests and error messages
- Query test execution history

## Getting Started

### Prerequisites

Before using this package, ensure you have:

1. **Unity 2022.3+** - This package requires Unity 2022.3 or later
2. **Unity MCP** - Install [com.coplaydev.unity-mcp](https://github.com/coplaydev/unity-mcp) version 6.0.0+
3. **Unity Test Framework** - Version 1.4.0+ (usually included in Unity)
4. **Code Coverage Package** (Optional) - Install [com.unity.testtools.codecoverage](https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@latest) for coverage features

### Installation

Choose one of the following installation methods:

#### Via Package Manager (Recommended)

1. Open Unity Package Manager (`Window → Package Manager`)
2. Click `+` → `Add package from git URL...`
3. Enter: `https://github.com/The1Studio/UnityTestUtilitiesMCP.git`
4. Click `Add`

#### Via manifest.json

Add to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.the1studio.unity-test-utilities-mcp": "https://github.com/The1Studio/UnityTestUtilitiesMCP.git"
  }
}
```

#### As Git Submodule

```bash
cd Packages/
git submodule add https://github.com/The1Studio/UnityTestUtilitiesMCP.git
```

### Setup Unity MCP

After installation:

1. Open `Window → MCP for Unity` in Unity Editor
2. Click `Auto-Setup` if not already connected
3. Verify "Connected" status appears

### First Steps

Try the basic coverage workflow:

```bash
# 1. Check if coverage is available
mcp__UnityTestUtilitiesMCP__get_coverage_settings '{}'

# 2. Enable coverage
mcp__UnityTestUtilitiesMCP__enable_code_coverage '{"enabled": true}'

# 3. Start recording
mcp__UnityTestUtilitiesMCP__start_coverage_recording '{}'

# 4. Run tests in Unity Editor (Window → General → Test Runner)

# 5. Stop recording and generate report
mcp__UnityTestUtilitiesMCP__stop_coverage_recording '{"generateReport": true}'
```

## Usage Patterns

### With Claude Code CLI

Unity Test Utilities for MCP is designed to work seamlessly with Claude Code:

```
User: "Run tests with code coverage"

Claude:
1. Checks coverage availability using get_coverage_settings
2. Enables coverage with enable_code_coverage
3. Starts recording with start_coverage_recording
4. Prompts user to run tests in Unity Test Runner
5. Stops recording and generates report
6. Reports coverage results path
```

### In CI/CD Pipelines

Integrate with automated testing workflows:

```yaml
# GitHub Actions example
- name: Run Tests with Coverage
  run: |
    unity-editor -batchmode -quit -projectPath . \
      -executeMethod UnityTestUtilitiesMCP.EnableCoverage
    unity-editor -batchmode -quit -projectPath . \
      -runTests -testPlatform EditMode
```

See [Examples](examples.md) for complete CI/CD configurations.

### Manual Testing Workflows

Use MCP tools to control coverage during manual testing:

1. Enable coverage before starting testing session
2. Start/stop recording around specific test runs
3. Generate reports to analyze coverage
4. Iterate on improving test coverage

## Architecture

### Package Structure

```
UnityTestUtilitiesMCP/
├── Editor/
│   ├── Services/           # Core services
│   │   ├── ICoverageService.cs
│   │   └── CoverageService.cs
│   ├── Tools/              # MCP tool implementations
│   │   ├── CodeCoverage/
│   │   │   ├── EnableCodeCoverage.cs
│   │   │   ├── StartCoverageRecording.cs
│   │   │   ├── StopCoverageRecording.cs
│   │   │   └── GetCoverageSettings.cs
│   │   ├── TestFiltering/  # Coming soon
│   │   ├── TestAnalysis/   # Coming soon
│   │   └── BatchOperations/ # Coming soon
│   └── Utils/              # Utility classes
├── Tests/                  # Package tests
├── Documentation~/         # Package documentation
├── Samples~/              # Sample code
└── package.json           # Package manifest
```

### Design Principles

1. **MCP-First**: All functionality exposed through MCP tools and resources
2. **Service Layer**: Core logic separated from MCP tool implementations
3. **Optional Dependencies**: Code Coverage features only available when package installed
4. **Error Handling**: Comprehensive error messages and validation
5. **Async Support**: Async operations for report generation

### Integration with Unity MCP

This package registers MCP tools and resources with Unity MCP:

- **Tools**: Commands that perform actions (e.g., `enable_code_coverage`)
- **Resources**: Queries that retrieve data (e.g., `get_coverage_settings`)

Tools are registered using `[McpForUnityTool]` and resources using `[McpForUnityResource]` attributes.

## Best Practices

### Code Coverage Workflow

1. **Enable once per session**: Enable coverage at the start of testing session
2. **Record around test runs**: Start recording before tests, stop after
3. **Generate reports periodically**: Create reports to track progress
4. **Commit reports to source control**: Track coverage trends over time

### Performance Considerations

- **Coverage overhead**: Code coverage adds ~10-20% execution time
- **Disable when not needed**: Turn off coverage for regular development
- **Use filtered test runs**: Only run relevant tests with coverage enabled
- **Batch report generation**: Generate reports at end of session, not after each test

### CI/CD Best Practices

- **Cache Unity installation**: Reuse Unity installation across runs
- **Parallel test execution**: Run tests in parallel where possible
- **Artifact upload**: Save coverage reports as CI artifacts
- **Threshold enforcement**: Fail builds below coverage threshold

## Troubleshooting

### Common Issues

#### Code Coverage Package Not Installed

**Symptom**: Error message about Code Coverage package not available

**Solution**: Install `com.unity.testtools.codecoverage` via Package Manager

#### MCP Tools Not Appearing

**Symptom**: MCP commands not available

**Solution**:
1. Verify Unity MCP is installed (`Window → MCP for Unity`)
2. Click `Auto-Setup` in MCP window
3. Restart Unity Editor if needed

#### Coverage Not Recording

**Symptom**: Coverage data is empty after test run

**Solution**:
1. Ensure coverage is enabled before starting recording
2. Verify tests actually executed
3. Check Unity Console for errors
4. Restart Unity Editor and try again

#### Report Generation Fails

**Symptom**: HTML report is not created

**Solution**:
1. Check that coverage data exists in `CodeCoverage/` directory
2. Verify Unity has write permissions to project directory
3. Look for detailed error in Unity Console

See [Troubleshooting section in README](../README.md#troubleshooting) for more solutions.

## Support and Resources

### Documentation

- [API Reference](api-reference.md) - Complete API documentation
- [Examples](examples.md) - Usage examples and workflows
- [Main README](../README.md) - Installation and quick start

### External Resources

- [Unity MCP Documentation](https://github.com/coplaydev/unity-mcp)
- [Unity Code Coverage Package](https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@latest)
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [Model Context Protocol](https://modelcontextprotocol.io)

### Getting Help

- **Issues**: [GitHub Issues](https://github.com/The1Studio/UnityTestUtilitiesMCP/issues)
- **Discussions**: [GitHub Discussions](https://github.com/The1Studio/UnityTestUtilitiesMCP/discussions)
- **The1Studio**: [GitHub Organization](https://github.com/The1Studio)

## Contributing

We welcome contributions! See the [Contributing section](../README.md#contributing) in the main README for guidelines.

### Areas for Contribution

- **Test Filtering**: Implement advanced test filtering tools
- **Test Analysis**: Add test result parsing and analysis
- **Documentation**: Improve docs and examples
- **CI/CD Templates**: Add more CI/CD integration examples
- **Bug Fixes**: Fix reported issues

## Roadmap

### Current Version (1.0.0)

- Code coverage enable/disable
- Coverage recording control
- HTML report generation
- Coverage settings query

### Upcoming Features

- **v1.1.0**: Advanced test filtering
- **v1.2.0**: Test result analysis
- **v2.0.0**: Batch operations, custom reports, coverage thresholds

See [Roadmap in README](../README.md#roadmap) for detailed plans.

## License

MIT License - See [LICENSE.md](../LICENSE.md) for full text.

Copyright (c) 2024 The1Studio

---

**Navigation**: [API Reference](api-reference.md) | [Examples](examples.md) | [GitHub](https://github.com/The1Studio/UnityTestUtilitiesMCP)
