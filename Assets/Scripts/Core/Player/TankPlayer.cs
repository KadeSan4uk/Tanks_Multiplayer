using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Collections;

public class TankPlayer : NetworkBehaviour
{
    [Header("Referenses")]
    [SerializeField] private CinemachineCamera _camera;

    [Header("Settings")]
    [SerializeField] private int _ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager.networkServer.GetuserDataByClientId(OwnerClientId);

            playerName.Value = userData.userName;
        }

        if (IsOwner)
        {
            _camera.Priority = _ownerPriority;
        }
    }
}
