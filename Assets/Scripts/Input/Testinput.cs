using UnityEngine;

public class Testinput : MonoBehaviour
{
    [SerializeField] InputReader inputReader;

    void Start()
    {
        inputReader.MoveEvent += HandleMove;
    }
    private void OnDestroy()
    {
        inputReader.MoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 movement)
    {
        Debug.Log(movement);
    }
}
