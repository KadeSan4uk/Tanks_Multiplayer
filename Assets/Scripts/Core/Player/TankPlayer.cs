using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("Referenses")]
    [SerializeField] private CinemachineCamera _camera;

    [Header("Settings")]
    [SerializeField] private int _ownerPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _camera.Priority = _ownerPriority;
        }
    }
}
