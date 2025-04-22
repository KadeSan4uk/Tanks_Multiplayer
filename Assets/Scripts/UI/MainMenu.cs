using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeField;
    [SerializeField] private Button _enterCodeButton;

    private void Start()
    {
        if (ClientSingleton.Instance == null) { return; }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public void EnterCodeButtonOnClick()
    {
        _enterCodeButton.gameObject.SetActive(false);
        _joinCodeField.gameObject.SetActive(true);
        _joinCodeField.Select();

    }

    public async void StartClient()
    {
        if (ClientSingleton.Instance == null)
        {
            Debug.LogError("ClientSingleton does not exist. The scene may have been launched directly or initialization has not completed..");
            return;
        }

        if (ClientSingleton.Instance.GameManager == null)
        {
            Debug.LogError("GameManager not initialized in ClientSingleton.");
            return;
        }

        await ClientSingleton.Instance.GameManager.StartClientAsync(_joinCodeField.text);
    }

}