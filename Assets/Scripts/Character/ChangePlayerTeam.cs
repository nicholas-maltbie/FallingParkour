using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    public class ChangePlayerTeam : NetworkBehaviour
    {
        public INetworkService networkService;
        public Team setTeam;
        public GameObject newPrefab;

        public void Awake()
        {
            this.networkService = new NetworkService(this);
        }

        public override void OnStartClient()
        {
            if (!NetworkClient.prefabs.ContainsValue(newPrefab))
            {
                NetworkClient.RegisterPrefab(newPrefab);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            // Only change teams if running from server
            if (!this.networkService.isServer)
            {
                return;
            }

            var team = other.GetComponent<PlayerTeam>();
            var networkIdentity = other.GetComponent<NetworkIdentity>();
            if (team != null && team.playerTeam != setTeam &&
                networkIdentity != null &&
                networkIdentity.connectionToClient != null)
            {
                NetworkConnection conn = networkIdentity.connectionToClient;
                GameObject oldPlayer = other.gameObject;
                GameObject newPlayer = Instantiate(newPrefab);
                newPlayer.GetComponent<PlayerTeam>().playerTeam = setTeam;
                NetworkServer.ReplacePlayerForConnection(conn, newPlayer);
                NetworkServer.Destroy(oldPlayer);
            }
        }
    }
}
