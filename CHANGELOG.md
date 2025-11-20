# Changelog

All notable changes to Unity Test Utilities for MCP will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-11-20

### Changed
- Update CHANGELOG: Document comprehensive unit test suite
- Add comprehensive unit tests for all services and tools
- Initial commit: Unity Test Utilities for MCP v1.0.0

## [Unreleased]

### Planned
- Advanced test filtering by pattern, category, and assembly
- Test result analysis and parsing of TestResults.xml
- Batch test operations
- Custom coverage report templates
- Coverage threshold validation
- Test execution history tracking

## [1.0.0] - 2024-10-30

### Added
- Initial release of Unity Test Utilities for MCP
- Code Coverage integration features:
  - `enable_code_coverage` MCP tool for enabling/disabling coverage
  - `start_coverage_recording` MCP tool for starting coverage recording
  - `stop_coverage_recording` MCP tool for stopping recording and generating reports
  - `get_coverage_settings` MCP resource for querying coverage status
- Core services:
  - `ICoverageService` interface for coverage operations
  - `CoverageService` implementation using Unity Code Coverage API
- Conditional compilation support for optional Code Coverage package
- EditorPrefs persistence for coverage settings
- Automatic HTML report generation
- Comprehensive error handling and validation
- Unity Console logging with `[CoverageService]` prefix
- Support for Unity 2022.3+
- Dependency on Unity MCP 6.0.0+
- Dependency on Unity Test Framework 1.4.0+
- Optional integration with Code Coverage package 1.0.0+

### Testing
- Comprehensive unit test suite with 59 tests
- Test coverage for all services:
  - `CoverageServiceTests` (11 tests): Enable/disable, start/stop recording, EditorPrefs persistence
  - `TestFilterServiceTests` (12 tests): Mode filtering, category/assembly filtering, pattern matching
  - `TestResultsServiceTests` (10 tests): File location, XML parsing, failure detail extraction
- Test coverage for MCP tools:
  - `EnableCodeCoverageTests` (10 tests): Parameter handling, validation, conditional compilation
  - `RunFilteredTestsTests` (16 tests): Mode selection, filtering, regex patterns, error handling
- Test assembly definition with proper configuration
- Conditional tests for optional Code Coverage package
- Platform-specific test scenarios (Linux/Mac/Windows)

### Documentation
- Comprehensive README.md with installation and usage instructions
- Complete API reference documentation
- Detailed usage examples including:
  - Basic coverage workflows
  - Advanced multi-session coverage
  - CI/CD integration examples (GitHub Actions, Jenkins, GitLab CI)
  - Claude Code integration patterns
  - Unity Editor script examples
  - Troubleshooting guides
- Documentation hub (index.md) linking all documentation
- MIT License

### Package Structure
- Editor assembly with proper assembly definition
- Service layer architecture separating business logic from MCP tools
- Conditional compilation with `UNITY_CODE_COVERAGE` define
- Version defines for automatic Code Coverage package detection
- Editor-only platform restriction for proper Unity integration

### Known Limitations
- Code Coverage package is optional but required for coverage features
- Report generation requires Unity Editor (not available in batch mode)
- Coverage recording must be stopped before generating reports (Unity API limitation)
- Test execution must be triggered manually in Test Runner or via batch mode

---

## Version History

### Version Numbering

Unity Test Utilities for MCP follows [Semantic Versioning](https://semver.org/):

- **MAJOR** version for incompatible API changes
- **MINOR** version for new functionality in a backwards compatible manner
- **PATCH** version for backwards compatible bug fixes

### Release Types

- **Stable Releases**: Versions like `1.0.0`, `1.1.0`, `2.0.0`
- **Pre-releases**: Versions like `1.1.0-beta.1`, `2.0.0-rc.1`
- **Development**: Unreleased changes tracked in `[Unreleased]` section

### Upgrade Guide

#### Upgrading from Pre-release to 1.0.0

This is the first stable release. No migration needed.

#### Future Upgrades

When upgrading between versions:

1. **Check Breaking Changes**: Review CHANGELOG for breaking changes in MAJOR versions
2. **Update Dependencies**: Ensure Unity MCP and other dependencies meet minimum version requirements
3. **Test Coverage Workflows**: Verify existing coverage scripts and workflows after upgrade
4. **Review API Changes**: Check API reference for deprecated or changed methods
5. **Update CI/CD Pipelines**: Update CI/CD configurations if API changes affect them

### Deprecation Policy

- Deprecated features will be marked in documentation and logs
- Deprecated features remain functional for at least one MINOR version
- Breaking removal only occurs in MAJOR version updates
- Migration guides provided for breaking changes

### Support Policy

- **Current Version (1.x)**: Full support with bug fixes and new features
- **Previous Major Version (0.x)**: No previous version exists yet
- **Older Versions**: Community support only, no official updates

### Compatibility Matrix

| Unity Test Utilities MCP | Unity MCP | Unity Version | Code Coverage |
|--------------------------|-----------|---------------|---------------|
| 1.0.0                    | 6.0.0+    | 2022.3+       | 1.0.0+ (opt)  |

---

## Contributing

When contributing, please:

1. Update CHANGELOG.md under `[Unreleased]` section
2. Follow [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) format
3. Categorize changes as: Added, Changed, Deprecated, Removed, Fixed, Security
4. Reference issue numbers where applicable
5. Update version compatibility matrix if needed

### Change Categories

- **Added**: New features or capabilities
- **Changed**: Changes to existing functionality
- **Deprecated**: Soon-to-be removed features
- **Removed**: Features removed in this version
- **Fixed**: Bug fixes
- **Security**: Security vulnerability fixes

---

## Links

- [GitHub Repository](https://github.com/The1Studio/UnityTestUtilitiesMCP)
- [Issue Tracker](https://github.com/The1Studio/UnityTestUtilitiesMCP/issues)
- [Pull Requests](https://github.com/The1Studio/UnityTestUtilitiesMCP/pulls)
- [Releases](https://github.com/The1Studio/UnityTestUtilitiesMCP/releases)
- [Documentation](Documentation~/index.md)

---

## Feedback

We welcome feedback on releases! Please:

- Report bugs via [GitHub Issues](https://github.com/The1Studio/UnityTestUtilitiesMCP/issues)
- Suggest features via [GitHub Discussions](https://github.com/The1Studio/UnityTestUtilitiesMCP/discussions)
- Share your use cases and success stories

Thank you for using Unity Test Utilities for MCP!
