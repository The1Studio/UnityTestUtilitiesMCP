# API Reference

Complete API documentation for Unity Test Utilities for MCP. This package provides MCP tools and resources for controlling Unity testing features.

## Table of Contents

- [MCP Tools](#mcp-tools)
  - [enable_code_coverage](#enable_code_coverage)
  - [start_coverage_recording](#start_coverage_recording)
  - [stop_coverage_recording](#stop_coverage_recording)
- [MCP Resources](#mcp-resources)
  - [get_coverage_settings](#get_coverage_settings)
- [Services](#services)
  - [ICoverageService](#icoverageservice)
  - [CoverageService](#coverageservice)
- [Error Handling](#error-handling)
- [Return Value Structures](#return-value-structures)

---

## MCP Tools

MCP tools are commands that perform actions. They are exposed through Unity MCP and can be called from Claude Code or other MCP clients.

### enable_code_coverage

Enables or disables Unity Code Coverage recording.

**Tool Name**: `enable_code_coverage`

**Namespace**: `TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage`

**Description**: Controls Unity Code Coverage recording. When enabled, coverage data will be collected during test runs. Optionally configures automatic report generation when recording stops.

#### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `enabled` | boolean | Yes | - | True to enable coverage, false to disable |
| `autoGenerateReport` | boolean | No | false | If true, automatically generate HTML report when recording stops |

#### Returns

**Success Response**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code coverage enabled. Auto-generate report: true"
    }
  ],
  "isError": false,
  "_meta": {
    "enabled": true,
    "autoGenerateReport": true,
    "coverageResultsPath": "/absolute/path/to/project/CodeCoverage"
  }
}
```

**Error Response**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code Coverage not supported: Unity Code Coverage package not installed"
    }
  ],
  "isError": true
}
```

#### Examples

**Enable coverage with auto-report:**
```json
{
  "enabled": true,
  "autoGenerateReport": true
}
```

**Disable coverage:**
```json
{
  "enabled": false
}
```

**Enable coverage without auto-report:**
```json
{
  "enabled": true,
  "autoGenerateReport": false
}
```

#### Errors

- **Missing parameter**: "Required parameter 'enabled' (bool) is missing"
- **Invalid type**: "Parameter 'enabled' must be a boolean, got: String"
- **Package not installed**: "Code Coverage not supported: Unity Code Coverage package not installed"

#### Notes

- Requires `com.unity.testtools.codecoverage` package to be installed
- Settings are persisted in EditorPrefs
- Coverage must be enabled before starting recording
- Auto-generate report setting is stored and used by `StopCoverageRecording`

---

### start_coverage_recording

Starts recording code coverage data.

**Tool Name**: `start_coverage_recording`

**Namespace**: `TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage`

**Description**: Begins recording code coverage data. Coverage must be enabled via `enable_code_coverage` before calling this tool. Resets any previous coverage data.

#### Parameters

No parameters required. Pass an empty JSON object `{}`.

#### Returns

**Success Response**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code coverage recording started"
    }
  ],
  "isError": false,
  "_meta": {
    "recording": true,
    "coverageResultsPath": "/absolute/path/to/project/CodeCoverage"
  }
}
```

**Error Response**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code coverage is not enabled. Call enable_code_coverage with enabled=true first."
    }
  ],
  "isError": true
}
```

#### Examples

**Start recording:**
```json
{}
```

#### Errors

- **Coverage not enabled**: "Code coverage is not enabled. Call enable_code_coverage with enabled=true first."
- **Package not installed**: "Code Coverage not supported: Unity Code Coverage package not installed"
- **Already recording**: "Cannot start recording: Recording is already in progress"

#### Notes

- Must call `enable_code_coverage` with `enabled=true` first
- Automatically calls `Coverage.ResetAll()` to clear previous data
- Recording continues until `stop_coverage_recording` is called
- Coverage data is accumulated during test execution
- Unity Console will show "[CoverageService] Code coverage recording started" log

---

### stop_coverage_recording

Stops recording code coverage data and optionally generates an HTML report.

**Tool Name**: `stop_coverage_recording`

**Namespace**: `TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage`

**Description**: Stops recording code coverage data. If `generateReport` is true or auto-generate is enabled, generates an HTML coverage report. The report includes detailed coverage statistics and source code annotations.

#### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `generateReport` | boolean | No | false | If true, generate HTML report after stopping |

#### Returns

**Success Response (with report)**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code coverage recording stopped and report generated"
    }
  ],
  "isError": false,
  "_meta": {
    "recording": false,
    "reportGenerated": true,
    "reportPath": "/absolute/path/to/project/CodeCoverage/Report/index.html",
    "coverageResultsPath": "/absolute/path/to/project/CodeCoverage"
  }
}
```

**Success Response (without report)**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code coverage recording stopped"
    }
  ],
  "isError": false,
  "_meta": {
    "recording": false,
    "reportGenerated": false,
    "coverageResultsPath": "/absolute/path/to/project/CodeCoverage"
  }
}
```

**Success Response (report generation failed)**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code coverage recording stopped, but report generation failed"
    }
  ],
  "isError": false,
  "_meta": {
    "recording": false,
    "reportGenerated": false,
    "reportError": "Report generation completed but index.html was not found",
    "coverageResultsPath": "/absolute/path/to/project/CodeCoverage"
  }
}
```

#### Examples

**Stop recording and generate report:**
```json
{
  "generateReport": true
}
```

**Stop recording without report:**
```json
{
  "generateReport": false
}
```

**Stop recording (use auto-generate setting):**
```json
{}
```

#### Errors

- **Package not installed**: "Code Coverage not supported: Unity Code Coverage package not installed"
- **Invalid type**: "Parameter 'generateReport' must be a boolean, got: String"

#### Notes

- Recording stops immediately, regardless of report generation
- Report generation is asynchronous (uses async/await)
- Report is saved to `<project>/CodeCoverage/Report/index.html`
- If auto-generate report was enabled in `enable_code_coverage`, report generates automatically
- Unity Console shows "[CoverageService] Code coverage recording stopped" log
- Report generation logs: "[CoverageService] Auto-generating coverage report..." or "[CoverageService] Generating HTML coverage report..."
- Generated report includes:
  - Overall coverage percentage
  - Per-file coverage statistics
  - Source code with line-by-line coverage annotations
  - Detailed hit counts per line

---

## MCP Resources

MCP resources are queries that retrieve data without performing actions. They provide read-only access to system state.

### get_coverage_settings

Queries current code coverage settings and status.

**Resource Name**: `get_coverage_settings`

**Namespace**: `TheOneStudio.UnityTestUtilitiesMCP.Editor.Tools.CodeCoverage`

**Description**: Retrieves current code coverage configuration and status. Provides read-only access to coverage settings without modifying any state.

#### Parameters

No parameters required. Pass an empty JSON object `{}`.

#### Returns

**Success Response (package installed)**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code coverage is enabled"
    }
  ],
  "isError": false,
  "_meta": {
    "coveragePackageInstalled": true,
    "enabled": true,
    "autoGenerateReport": true,
    "coverageResultsPath": "/absolute/path/to/project/CodeCoverage",
    "recording": true
  }
}
```

**Success Response (package not installed)**:
```json
{
  "content": [
    {
      "type": "text",
      "text": "Code Coverage package is not installed. Install com.unity.testtools.codecoverage to use coverage features."
    }
  ],
  "isError": false,
  "_meta": {
    "coveragePackageInstalled": false,
    "enabled": false,
    "autoGenerateReport": false,
    "coverageResultsPath": null,
    "recording": false
  }
}
```

#### Return Fields

| Field | Type | Description |
|-------|------|-------------|
| `coveragePackageInstalled` | boolean | True if `com.unity.testtools.codecoverage` is installed |
| `enabled` | boolean | True if coverage recording is enabled |
| `autoGenerateReport` | boolean | True if auto-generate report is enabled |
| `coverageResultsPath` | string/null | Absolute path to coverage results directory, or null if package not installed |
| `recording` | boolean | True if coverage is currently recording (inferred from Coverage.enabled) |

#### Examples

**Query settings:**
```json
{}
```

#### Errors

- **Query failed**: "Failed to get coverage settings: [error message]"

#### Notes

- This is a resource (query), not a tool (command)
- Does not modify any state
- Safe to call at any time
- Returns current state even if coverage is not enabled
- `recording` status is inferred from Unity's `Coverage.enabled` property
- If package is not installed, all boolean fields return false except `coveragePackageInstalled`

---

## Services

Services provide the core functionality for code coverage operations. They are used internally by MCP tools but can also be used directly in Unity Editor scripts.

### ICoverageService

Interface for managing Unity Code Coverage functionality.

**Namespace**: `TheOneStudio.UnityTestUtilitiesMCP.Editor.Services`

#### Methods

##### IsCoverageEnabled()

Checks if code coverage recording is currently enabled.

**Signature**:
```csharp
bool IsCoverageEnabled()
```

**Returns**: `true` if coverage is enabled, `false` otherwise.

**Example**:
```csharp
var service = new CoverageService();
if (service.IsCoverageEnabled())
{
    Debug.Log("Coverage is enabled");
}
```

---

##### EnableCoverage(enabled, autoGenerateReport)

Enables or disables code coverage recording.

**Signature**:
```csharp
void EnableCoverage(bool enabled, bool autoGenerateReport = false)
```

**Parameters**:
- `enabled` (bool): True to enable coverage, false to disable
- `autoGenerateReport` (bool, optional): If true, automatically generate HTML report after recording stops. Default: false

**Throws**:
- `NotSupportedException`: If Code Coverage package is not installed

**Example**:
```csharp
var service = new CoverageService();
service.EnableCoverage(true, autoGenerateReport: true);
```

---

##### StartRecording()

Starts recording code coverage data.

**Signature**:
```csharp
void StartRecording()
```

**Throws**:
- `InvalidOperationException`: If coverage is not enabled
- `NotSupportedException`: If Code Coverage package is not installed

**Example**:
```csharp
var service = new CoverageService();
service.StartRecording();
```

**Notes**:
- Must call `EnableCoverage(true)` before starting recording
- Automatically resets previous coverage data

---

##### StopRecording()

Stops recording code coverage data.

**Signature**:
```csharp
void StopRecording()
```

**Throws**:
- `NotSupportedException`: If Code Coverage package is not installed

**Example**:
```csharp
var service = new CoverageService();
service.StopRecording();
```

**Notes**:
- If auto-generate report is enabled, report generation starts automatically
- Safe to call even if recording was not started

---

##### GenerateReportAsync()

Generates an HTML code coverage report asynchronously.

**Signature**:
```csharp
Task<string> GenerateReportAsync()
```

**Returns**: Task that resolves to the absolute file path of the generated HTML report (index.html)

**Throws**:
- `NotSupportedException`: If Code Coverage package is not installed
- `InvalidOperationException`: If coverage results path is not available
- `FileNotFoundException`: If report generation completes but index.html is not found

**Example**:
```csharp
var service = new CoverageService();
try
{
    string reportPath = await service.GenerateReportAsync();
    Debug.Log($"Report generated at: {reportPath}");
}
catch (Exception ex)
{
    Debug.LogError($"Report generation failed: {ex.Message}");
}
```

**Notes**:
- Report is generated to `<project>/CodeCoverage/Report/index.html`
- Uses Unity's `Coverage.GenerateHTMLReport()` API
- Must have coverage data available (run tests after starting recording)

---

##### GetCoverageResultsPath()

Gets the file path where code coverage results are stored.

**Signature**:
```csharp
string GetCoverageResultsPath()
```

**Returns**: Absolute path to the coverage results directory, or `null` if coverage is not available

**Example**:
```csharp
var service = new CoverageService();
string path = service.GetCoverageResultsPath();
Debug.Log($"Coverage results path: {path}");
```

**Notes**:
- Returns `<project>/CodeCoverage/` directory path
- Creates directory if it doesn't exist
- Returns `null` if Code Coverage package is not installed

---

### CoverageService

Implementation of `ICoverageService` using Unity's Code Coverage API.

**Namespace**: `TheOneStudio.UnityTestUtilitiesMCP.Editor.Services`

**Description**: Concrete implementation that wraps Unity's Code Coverage package API. Requires `com.unity.testtools.codecoverage` to be installed.

#### Implementation Details

##### Editor Preferences

Settings are stored in Unity EditorPrefs:

- `UnityTestUtilitiesMCP.Coverage.Enabled` - Coverage enabled state
- `UnityTestUtilitiesMCP.Coverage.AutoGenerateReport` - Auto-generate report setting

##### Conditional Compilation

Uses `#if UNITY_CODE_COVERAGE` directives to conditionally compile coverage features:

```csharp
#if UNITY_CODE_COVERAGE
    // Coverage features available
    Coverage.enabled = true;
#else
    // Coverage features not available
    throw new NotSupportedException("Package not installed");
#endif
```

The `UNITY_CODE_COVERAGE` define is automatically set when `com.unity.testtools.codecoverage` version 1.0.0+ is installed (configured in assembly definition's versionDefines).

##### Logging

All operations log to Unity Console with `[CoverageService]` prefix:

- `[CoverageService] Code coverage enabled. Auto-generate report: true`
- `[CoverageService] Code coverage recording started`
- `[CoverageService] Code coverage recording stopped`
- `[CoverageService] Generating HTML coverage report...`
- `[CoverageService] Coverage report generated successfully at: /path/to/report`

---

## Error Handling

All MCP tools and resources follow consistent error handling patterns.

### Error Response Structure

```json
{
  "content": [
    {
      "type": "text",
      "text": "Error message describing what went wrong"
    }
  ],
  "isError": true
}
```

### Common Error Types

#### Package Not Installed

**Message**: "Code Coverage not supported: Unity Code Coverage package (com.unity.testtools.codecoverage) is not installed. Install it via Package Manager to use code coverage features."

**Cause**: Code Coverage package is not installed

**Solution**: Install `com.unity.testtools.codecoverage` via Package Manager

---

#### Missing Required Parameter

**Message**: "Required parameter 'enabled' (bool) is missing"

**Cause**: Required parameter was not provided in request

**Solution**: Include all required parameters in request JSON

---

#### Invalid Parameter Type

**Message**: "Parameter 'enabled' must be a boolean, got: String"

**Cause**: Parameter value has wrong type

**Solution**: Ensure parameter types match API specification

---

#### Coverage Not Enabled

**Message**: "Code coverage is not enabled. Call enable_code_coverage with enabled=true first."

**Cause**: Attempted to start recording without enabling coverage first

**Solution**: Call `enable_code_coverage` with `enabled=true` before `start_coverage_recording`

---

#### Recording Already in Progress

**Message**: "Cannot start recording: Recording is already in progress"

**Cause**: Attempted to start recording while already recording

**Solution**: Call `stop_coverage_recording` first, then start new recording session

---

#### Report Generation Failed

**Message**: "Report generation completed but index.html was not found"

**Cause**: Unity's report generation API completed but output file is missing

**Solution**:
- Ensure coverage data exists (run tests after starting recording)
- Check Unity has write permissions to project directory
- Verify disk space is available
- Check Unity Console for additional error details

---

## Return Value Structures

### Success Response

```typescript
{
  content: [
    {
      type: "text",
      text: string  // Success message
    }
  ],
  isError: false,
  _meta: {
    // Tool-specific metadata
  }
}
```

### Error Response

```typescript
{
  content: [
    {
      type: "text",
      text: string  // Error message
    }
  ],
  isError: true
}
```

### Metadata Structures

#### enable_code_coverage

```typescript
{
  enabled: boolean,
  autoGenerateReport: boolean,
  coverageResultsPath: string
}
```

#### start_coverage_recording

```typescript
{
  recording: boolean,
  coverageResultsPath: string
}
```

#### stop_coverage_recording (success with report)

```typescript
{
  recording: boolean,
  reportGenerated: boolean,
  reportPath: string,
  coverageResultsPath: string
}
```

#### stop_coverage_recording (success without report)

```typescript
{
  recording: boolean,
  reportGenerated: boolean,
  coverageResultsPath: string
}
```

#### stop_coverage_recording (report generation failed)

```typescript
{
  recording: boolean,
  reportGenerated: boolean,
  reportError: string,
  coverageResultsPath: string
}
```

#### get_coverage_settings

```typescript
{
  coveragePackageInstalled: boolean,
  enabled: boolean,
  autoGenerateReport: boolean,
  coverageResultsPath: string | null,
  recording: boolean
}
```

---

## Version Information

**Current Version**: 1.0.0

**Compatibility**:
- Unity 2022.3+
- Unity MCP 6.0.0+
- Unity Test Framework 1.4.0+
- Code Coverage Package 1.0.0+ (optional)

---

**Navigation**: [← Back to Index](index.md) | [Examples →](examples.md) | [GitHub](https://github.com/The1Studio/UnityTestUtilitiesMCP)
