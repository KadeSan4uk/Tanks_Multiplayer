using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private BountyCoin _coinPrefab;

    [Header("Settings")]
    [SerializeField] private float _coinSpread = 3f;
    [SerializeField] private float _bountyPercentage = 50f;
    [SerializeField] private int _bountyCoinCount = 5;
    [SerializeField] private int _minBountyCoinValue = 5;
    [SerializeField] private LayerMask _layerMask;

    private float _coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        _health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        _health.OnDie -= HandleDie;
    }

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<Coin>(out Coin coin)) { return; }

        int coinValue = coin.Collect();

        if (!IsServer) { return; }

        TotalCoins.Value += coinValue;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int) (TotalCoins.Value * (_bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / _bountyCoinCount;

        if (bountyCoinValue < _minBountyCoinValue) { return; }

        for (int i = 0; i < _bountyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(_coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }

    }

    private Vector2 GetSpawnPoint()
    {
        Vector2 spawnPoint = transform.position;
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            spawnPoint = (Vector2) transform.position + Random.insideUnitCircle * _coinSpread;

            Collider2D trySpawn = Physics2D.OverlapCircle(spawnPoint, _coinRadius, _layerMask);
            if (trySpawn == null)
            {
                return spawnPoint;
            }
        }

        return spawnPoint;
    }
}
