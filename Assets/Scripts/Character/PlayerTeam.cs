
using Mirror;

namespace PropHunt.Character
{
    public class PlayerTeam : NetworkBehaviour
    {
        [SyncVar]
        public string playerTeam;
    }
}