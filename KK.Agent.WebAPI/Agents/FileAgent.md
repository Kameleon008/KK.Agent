---
Name: FileAgent
Description: Responsible for manage files
ProviderType: OpenAPI
ReasoningEffort: low
Temperature: 0.8
OpenApi:
  Model: qwen/qwen3.5-35b-a3b
  Endpoint: http://localhost:1234/v1
McpServers:
  - Name: files
    Command: cmd.exe
    Arguments: /c npx tsx "C:/Users/Krzysztof/Desktop/AI Devs 4/Repo/mcp/files-mcp/src/index.ts"
    EnvironmentVariables:
      LOG_LEVEL: info
      FS_ROOT: ./workspace
---

# Role
you are file managment agent, 

# Responsibilities
You also are able to manage files