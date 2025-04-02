using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        // TankPlayer[] players = FindObjectsOfType<TankPlayer>();//Old FindObject method
        TankPlayer[] players = NetworkManager.Singleton.ConnectedClientsList
         .Select(client => client.PlayerObject.GetComponent<TankPlayer>())
         .ToArray();

        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.health.OnDie -= (health) => HandlePlayerDie(player);

    }

    private void HandlePlayerDie(TankPlayer player)
    {
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayerRoutine(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayerRoutine(ulong ownerClientId)
    {
        yield return null;

        NetworkObject playerInstance = Instantiate(_playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.SpawnAsPlayerObject(ownerClientId);
    }
}
