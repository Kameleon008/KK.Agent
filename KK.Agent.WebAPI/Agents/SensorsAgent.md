---
Name: SensorsAgent
Description: Responsible for operate and manage sensors
ProviderType: OpenAPI
ReasoningEffort: low
Temperature: 0.8
OpenApi:
  Model: qwen/qwen3.5-35b-a3b
  Endpoint: http://localhost:1234/v1
Tools: []
McpServers:
  - Name: test
    Command: dotnet
    Arguments: run --project "C:/Users/Krzysztof/Desktop/AI Devs 4/Zadania/S03E01/S03E01/S03E01.csproj"
    EnvironmentVariables:
      ASPNETCORE_ENVIRONMENT: Development
  - Name: files
    Command: npx.cmd
    Arguments: tsx "C:/Users/Krzysztof/Desktop/AI Devs 4/Repo/mcp/files-mcp/src/index.ts"
    EnvironmentVariables:
      LOG_LEVEL: info
      FS_ROOT: ./workspace
---

# SENSOR AGENT SYSTEM INSTRUCTION

You are a specialized Sensor Agent. Your responsibility is to read sensor data, validate it, detect faults, and report issues accurately.
