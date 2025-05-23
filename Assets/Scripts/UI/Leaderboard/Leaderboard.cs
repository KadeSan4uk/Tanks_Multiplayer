using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform _leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay _leaderboardEntityPrefab;
    [SerializeField] private int _entitiesToDisplay = 8;

    private NetworkList<LeaderboardEntityState> _leaderboardEntities;

    private List<LeaderboardEntityDisplay> _entityDisplays = new List<LeaderboardEntityDisplay>();

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
                if (!_entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderboardEntityDisplay leaderboardEntity =
                    Instantiate(_leaderboardEntityPrefab, _leaderboardEntityHolder);

                    leaderboardEntity.Initialise(changeEvent.Value.ClientId, changeEvent.Value.PlayerName, changeEvent.Value.Coins);

                    _entityDisplays.Add(leaderboardEntity);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntityDisplay displayToRemove =
                _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    _entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate =
               _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        _entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for (int i = 0; i < _entityDisplays.Count; i++)
        {
            _entityDisplays[i].transform.SetSiblingIndex(i);
            _entityDisplays[i].UpdateText();
            //bool shouldShow = i <= _entitiesToDisplay - 1;
            _entityDisplays[i].gameObject.SetActive(i <= _entitiesToDisplay - 1);
        }

        LeaderboardEntityDisplay myDisplay =
        _entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= _entitiesToDisplay)
            {
                _leaderboardEntityHolder.GetChild(_entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
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

        player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) =>
             HandleCoinsChanged(player.OwnerClientId, newCoins);
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

        player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) =>
            HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleCoinsChanged(ulong clientId, int newCoins)
    {
        for (int i = 0; i < _leaderboardEntities.Count; i++)
        {
            if (_leaderboardEntities[i].ClientId != clientId) { continue; }

            _leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = _leaderboardEntities[i].ClientId,
                PlayerName = _leaderboardEntities[i].PlayerName,
                Coins = newCoins
            };
            return;
        }
    }
}
