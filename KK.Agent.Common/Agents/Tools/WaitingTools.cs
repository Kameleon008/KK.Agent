using System.ComponentModel;
using KK.Agent.Common.Attributes;

namespace KK.Agent.Common.Agents.Tools
{

    public class WaitingTools(IServiceProvider provider)
    {
        [AgentTool("Waits for the specified number of milliseconds and returns a confirmation message.")]
        public async Task<string> wait_milliseconds_async(
            [Description("The number of milliseconds to wait (must be non-negative). Example: 5000")] int milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentException("Milliseconds must be non-negative.", nameof(milliseconds));
            }


            await Task.Delay(TimeSpan.FromMilliseconds(milliseconds)).ConfigureAwait(false);


            return $"Successfully waited for {milliseconds} millisecond{(milliseconds == 1 ? "" : "s")}.";
        }
    }
}
