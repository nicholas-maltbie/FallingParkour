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

        public void Start()
        {
            this.networkService = new NetworkService(this);
            if (!NetworkClient.prefabs.ContainsValue(newPrefab))
            {
                NetworkClient.RegisterPrefab(newPrefab);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            var team = other.GetComponent<PlayerTeam>();
            var networkIdentity = other.GetComponent<NetworkIdentity>();
            if (this.networkService.isServer && team != null && team.playerTeam != setTeam && networkIdentity.connectionToClient != null)
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
