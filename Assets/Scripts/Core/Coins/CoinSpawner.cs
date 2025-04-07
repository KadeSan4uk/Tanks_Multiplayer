using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin _coinPrefab;
    [SerializeField] private int _maxCoin = 50;
    [SerializeField] private int _coinValue = 5;
    [SerializeField] private Vector2 _xSpawnRange;
    [SerializeField] private Vector2 _ySpawnRange;
    [SerializeField] private LayerMask _layerMask;

    private float _coinRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < _maxCoin; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(_coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(_coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        const int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(_xSpawnRange.x, _xSpawnRange.y);
            float y = Random.Range(_ySpawnRange.x, _ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);

            Collider2D trySpawn = Physics2D.OverlapCircle(spawnPoint, _coinRadius, _layerMask);
            if (trySpawn == null)
            {
                return spawnPoint;
            }
        }

        return new Vector2(Random.Range(_xSpawnRange.x, _xSpawnRange.y), Random.Range(_ySpawnRange.x, _ySpawnRange.y));
    }
}
