using System;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private CoinWallet _coinWallet;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _muzzleFlashDuration;
    [SerializeField] private int _costToFire;

    private bool _shouldFire;
    private float _timer;
    private float _muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        _inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        _inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void Update()
    {
        MuzzleFlashTimer();

        if (!IsOwner) { return; }

        CheckTimer();

        if (!_shouldFire) { return; }

        PrimaryFireServerAndSpawnDummyProjrctileAndFireRate();
    }

    private void CheckTimer()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
    }

    private void PrimaryFireServerAndSpawnDummyProjrctileAndFireRate()
    {
        if (_coinWallet.TotalCoins.Value < _costToFire) { return; }

        if (_timer > 0) { return; }

        PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

        _timer = 1 / _fireRate;
    }

    private void MuzzleFlashTimer()
    {
        if (_muzzleFlashTimer > 0f)
        {
            _muzzleFlashTimer -= Time.deltaTime;

            if (_muzzleFlashTimer <= 0f)
            {
                _muzzleFlash.SetActive(false);
            }
        }
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this._shouldFire = shouldFire;
    }

    [ClientRpc]
    private void PrimaryFireClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) { return; }

        SpawnDummyProjectile(spawnPos, direction);
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (_coinWallet.TotalCoins.Value < _costToFire) { return; }
        _coinWallet.SpendCoins(_costToFire);

        GameObject projectileInstance = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * _projectileSpeed;
        }

        SpawnDummyProjectile(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        _muzzleFlash.SetActive(true);
        _muzzleFlashTimer = _muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(_clientProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * _projectileSpeed;
        }
    }
}
