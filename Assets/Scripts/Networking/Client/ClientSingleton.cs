using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton _instance;
    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            if (_instance is not null) { return _instance; }

            _instance = FindFirstObjectByType<ClientSingleton>();

            if (_instance == null)
            {
                Debug.LogError("No ClientSingleton in the scene!");
                return null;
            }
            return _instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);


    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        return await GameManager.InintAsync();
    }

    private void OnDestroy()
    {
        GameManager.Dispose();
    }
}
