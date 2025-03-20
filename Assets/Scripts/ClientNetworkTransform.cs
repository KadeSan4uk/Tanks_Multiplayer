using Unity.Netcode.Components;
using UnityEngine;
using System;
public class ClientNetworkTransform : NetworkTransform
{


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }

    public override void OnUpdate()
    {
        CanCommitToTransform = IsOwner;
        base.OnUpdate();

        if (NetworkManager is not null)
        {
            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                if (CanCommitToTransform)
                {
                    SetState(transform.position, transform.rotation, transform.localScale);
                }
            }
        }
    }

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
