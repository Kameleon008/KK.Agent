using KK.Agent.Library.Attributes;
using System.ComponentModel;

namespace KK.Agent.WebAPI.Tools;

/// <summary>
/// A plugin that provides functionality to wait for a specified duration.
/// </summary>
public class WaitingTools()
{
    [AgentTool("Waits for the specified number of seconds and returns a confirmation message.")]
    public async Task<string> WaitAsync(
        [Description("The number of seconds to wait (must be non-negative). Example: 5")] int seconds)
    {
        if (seconds < 0)
        {
            throw new ArgumentException("Seconds must be non-negative.", nameof(seconds));
        }

        
        await Task.Delay(TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
        
        
        return $"Successfully waited for {seconds} second{(seconds == 1 ? "" : "s")}.";
    }

    [AgentTool("Waits for the specified number of milliseconds and returns a confirmation message.")]
    public async Task<string> WaitMillisecondsAsync(
        [Description("The number of milliseconds to wait (must be non-negative). Example: 5000")] int milliseconds)
    {
        if (milliseconds < 0)
        {
            throw new ArgumentException("Milliseconds must be non-negative.", nameof(milliseconds));
        }

        
        await Task.Delay(TimeSpan.FromMilliseconds(milliseconds)).ConfigureAwait(false);
        
        
        return $"Successfully waited for {milliseconds} millisecond{(milliseconds == 1 ? "" : "s")}.";
    }

    [AgentTool("Waits for a specified time (in seconds and optional milliseconds) and returns a confirmation message.")]
    public async Task<string> WaitCustomAsync(
        [Description("The number of seconds to wait. Example: 5")] int seconds,
        [Description("Optional additional milliseconds. Default is 0. Example: 500")] int milliseconds = 0)
    {
        if (seconds < 0 || milliseconds < 0)
        {
            throw new ArgumentException("Seconds and milliseconds must be non-negative.");
        }

        var totalDuration = TimeSpan.FromSeconds(seconds) + TimeSpan.FromMilliseconds(milliseconds);
        
        
        await Task.Delay(totalDuration).ConfigureAwait(false);
        
        
        return $"Successfully waited for {seconds} second{(seconds == 1 ? "" : "s")} and {milliseconds} millisecond{(milliseconds == 1 ? "" : "s")}.";
    }
}
