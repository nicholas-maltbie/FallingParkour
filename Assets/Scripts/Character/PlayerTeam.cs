
using Mirror;

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
        [SyncVar]
        public Team playerTeam;
    }
}