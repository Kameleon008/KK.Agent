using System.Diagnostics;

namespace KK.Agent.Library.Mcp;

public class McpClient(ConfigMcpServer options)
{
    public string Name => options.Name;

    public Process? Process;
    
    public StreamWriter? Input => Process?.StandardInput;
    
    public StreamReader? Output => Process?.StandardOutput;

    public void Start()
    {
        var psi = new ProcessStartInfo
        {
            FileName = options.Command,
            Arguments = options.Arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = options.UseShellExecute
        };

        if (!string.IsNullOrEmpty(options.WorkingDirectory))
        {
            psi.WorkingDirectory = options.WorkingDirectory;
        }

        Process = new Process { StartInfo = psi };
        Process.Start();
    }
}