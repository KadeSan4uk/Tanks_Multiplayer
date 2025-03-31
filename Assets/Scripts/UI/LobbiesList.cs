using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform _lobbyItemParent;
    [SerializeField] private LobbyItem _lobbyItemPrefab;

    private bool _isJoining;
    private bool _isRefreshing;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (_isRefreshing) { return; }
        _isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>
            {
               new QueryFilter
               (   field:QueryFilter.FieldOptions.AvailableSlots,
                   op:QueryFilter.OpOptions.GT,
                   value:"0"
               ),
               new QueryFilter
               (   field:QueryFilter.FieldOptions.IsLocked,
                   op:QueryFilter.OpOptions.EQ,
                   value:"0"
               )
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in _lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(_lobbyItemPrefab, _lobbyItemParent);
                lobbyItem.Initialise(this, lobby);
            }
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }



        _isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (_isJoining) { return; }
        _isJoining = true;

        try
        {
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }

        _isJoining = false;
    }
}
