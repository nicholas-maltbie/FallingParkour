using MLAPI;
using MLAPI.NetworkVariable;

namespace PropHunt.Character
{
    public enum Team
    {
        Hunter,
        Prop,
        Spectator
    }

    public class PlayerTeam : NetworkBehaviour
    {
        public NetworkVariable<Team> playerTeam = new NetworkVariable<Team>();
    }
}