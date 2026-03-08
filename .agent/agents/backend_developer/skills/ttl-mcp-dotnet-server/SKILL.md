---
name: ttl-mcp-dotnet-server
description: Standardized instructions for building a .NET MCP Server integrated with the TTL API.
---

# TTL .NET MCP Server

This skill guides you in creating a .NET-based MCP (Model Context Protocol) Server that exposes API functionalities as tools for AI agents.

## 1. Project structure
An MCP server follows the same **Clean Architecture** as the TTL APIs:

- **{{ProjectName}}.Mcp.Domain**: Core tool definitions and repository interfaces.
- **{{ProjectName}}.Mcp.Application**: Logic for executing tools and processing resources.
- **{{ProjectName}}.Mcp.Infrastructure**: MCP protocol implementation (Handshake, Transport).
- **{{ProjectName}}.Mcp.API**: The entry point (HTTP or SSE) for the MCP client.

## 2. Defining Tools
Every tool must be implemented as a MediatR Command in the `Application` layer.

```csharp
public class GetEmployeeDetailsTool : IRequest<string>
{
    public string EmployeeCode { get; set; }
}
```

## 3. MCP Handler
The `Infrastructure` layer must expose the `McpHandler` which maps incoming tool calls to MediatR requests.

- **Tools**: List all available tools and their JSON schemas.
- **Resources**: Expose documentation or specific files as resources.
- **Prompts**: Provide pre-defined prompts for specific agent tasks.

## 4. Gateway Integration
MCP servers MUST be routed through the `TTL.Gateway` (YARP).
- Configure the route in `appsettings.json` of the Gateway.
- Ensure JWT authentication is bypassed only for designated MCP handshake endpoints if necessary.

## 5. Security
- MCP servers MUST validate the `X-Mcp-Key` or `Authorization` header.
- Use `ICurrentUserService` to limit tool execution based on the agent's permissions.
