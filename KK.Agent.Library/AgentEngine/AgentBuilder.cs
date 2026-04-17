
namespace KK.Agent.Library.Agents
{
    public class AgentBuilder<TAgent>
    where TAgent : AgentBase, new ()
    {
        private TAgent Instance;

        public AgentBuilder()
        {

        }

        public TAgent Build()
        {

            return new TAgent();
        }
    }
}
