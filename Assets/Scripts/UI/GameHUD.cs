using Unity.Netcode;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogWarning("NetworkManager not found, exit without disconnecting.");
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            if (HostSingleton.Instance != null && HostSingleton.Instance.GameManager != null)
            {
                HostSingleton.Instance.GameManager.Shutdown();
            }
            else
            {
                Debug.LogWarning("HostSingleton or GameManager is not initialized.");
            }
        }

        if (ClientSingleton.Instance != null && ClientSingleton.Instance.GameManager != null)
        {
            ClientSingleton.Instance.GameManager.Disconnect();
        }
        else
        {
            Debug.LogWarning("ClientSingleton or GameManager is not initialized.");
        }
    }
}
