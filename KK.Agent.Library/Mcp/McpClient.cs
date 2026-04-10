using System.Diagnostics;

namespace KK.Agent.Library.Mcp;

public class McpClient(ConfigMcpServers options)
{
    private readonly ConfigMcpServer _options = options.Servers.First();
    public Process? Process;

    public StreamWriter? Input => Process?.StandardInput;
    public StreamReader? Output => Process?.StandardOutput;

    public void Start()
    {
        var psi = new ProcessStartInfo
        {
            FileName = _options.Command,
            Arguments = _options.Arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = _options.UseShellExecute
        };

        if (!string.IsNullOrEmpty(_options.WorkingDirectory))
        {
            psi.WorkingDirectory = _options.WorkingDirectory;
        }

        Process = new Process { StartInfo = psi };
        Process.Start();
    }
}