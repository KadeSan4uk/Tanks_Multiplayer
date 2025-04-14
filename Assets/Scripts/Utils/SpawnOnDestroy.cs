using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;

        if (!gameObject.scene.isLoaded) return;

        Instantiate(_prefab, transform.position, Quaternion.identity);
    }
}
