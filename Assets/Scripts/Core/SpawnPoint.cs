using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    private void OnEnable()
    {
        _spawnPoints.Add(this);
    }

    private void OnDisable()
    {
        _spawnPoints.Remove(this);
    }

    public static Vector3 GetRandomSpawnPos()
    {
        if (_spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }

        return _spawnPoints[Random.Range(0, _spawnPoints.Count)].transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
