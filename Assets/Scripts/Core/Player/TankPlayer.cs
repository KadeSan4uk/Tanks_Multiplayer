using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Collections;
using System;

public class TankPlayer : NetworkBehaviour
{
    [Header("Referenses")]
    [SerializeField] private CinemachineCamera _camera;
    [field: SerializeField] public Health health { get; private set; }

    [Header("Settings")]
    [SerializeField] private int _ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager.networkServer.GetuserDataByClientId(OwnerClientId);

            playerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            _camera.Priority = _ownerPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
