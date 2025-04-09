using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Collections;
using System;

public class TankPlayer : NetworkBehaviour
{
    [Header("Referenses")]
    [SerializeField] private CinemachineCamera _camera;

    [SerializeField] private SpriteRenderer _minimapIconRenderer;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet Wallet { get; private set; }

    [Header("Settings")]
    [SerializeField] private int _ownerPriority = 15;
    [SerializeField] private Color _ownerColor;

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
            _minimapIconRenderer.color = _ownerColor;
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
