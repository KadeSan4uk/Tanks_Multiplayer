using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _queueStatusText;
    [SerializeField] private TMP_Text _queueTimerText;
    [SerializeField] private TMP_Text _findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;

    private void Start()
    {
        if (ClientSingleton.Instance == null) { return; }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        _queueStatusText.text = string.Empty;
        _queueTimerText.text = string.Empty;
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}
