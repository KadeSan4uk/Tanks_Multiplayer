using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;
    [SerializeField] private HostSingleton _hostPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            HostSingleton hostSingleton = Instantiate(_hostPrefab);//It must be first created HostPrefab
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(_clientPrefab);// If clientPrefab was created firs then hostPrefab, we are having NullReferensExceptoin.
            bool authenticated = await clientSingleton.CreateClient();

            //go to Main Menu
            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }
}
