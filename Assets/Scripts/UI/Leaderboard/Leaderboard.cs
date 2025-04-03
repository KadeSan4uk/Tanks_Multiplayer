using NUnit.Framework.Constraints;
using System;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform _leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay _leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntityState> _leaderboardEntities;

    private void Awake()
    {
        _leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;

            foreach (LeaderboardEntityState entity in _leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

            foreach (var player in players)
            {
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                Instantiate(_leaderboardEntityPrefab, _leaderboardEntityHolder);
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:

                break;
        }
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        _leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            Coins = 0
        });
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (_leaderboardEntities == null) { return; }

        foreach (LeaderboardEntityState entity in _leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId) { continue; }

            _leaderboardEntities.Remove(entity);
            break;
        }
    }
}
