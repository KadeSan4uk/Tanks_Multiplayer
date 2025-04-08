using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image _healPowerBar;

    [Header("Settings")]

    [SerializeField] private int _maxHealPower = 30;
    [SerializeField] private float _healCooldown = 60f;
    [SerializeField] private float _healTickRate = 1f;
    [SerializeField] private int _coinsPerTick = 10;
    [SerializeField] private int _healthPerTick = 10;

    private float _remainingCooldown;
    private float _tickTimer;


    private List<TankPlayer> _playersInZone = new List<TankPlayer>();

    private NetworkVariable<int> _healPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _healPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, _healPower.Value);
        }

        if (IsServer)
        {
            _healPower.Value = _maxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _healPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        _playersInZone.Add(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        _playersInZone.Remove(player);
    }

    private void Update()
    {
        if (!IsServer) { return; }

        if (_remainingCooldown > 0)
        {
            _remainingCooldown -= Time.deltaTime;

            if (_remainingCooldown <= 0)
            {
                _healPower.Value = _maxHealPower;
            }
            else
            {
                return;
            }
        }

        _tickTimer += Time.deltaTime;

        if (_tickTimer >= 1 / _healTickRate)
        {
            foreach (TankPlayer player in _playersInZone)
            {
                if (_healPower.Value == 0) { break; }

                if (player.Health.CurrentHealth.Value == player.Health.MaxHealth) { continue; }

                if (player.Wallet.TotalCoins.Value < _coinsPerTick) { continue; }

                player.Wallet.SpendCoins(_coinsPerTick);
                player.Health.RestoreHealth(_healthPerTick);

                _healPower.Value -= 1;

                if (_healPower.Value == 0)
                {
                    _remainingCooldown = _healCooldown;
                }
            }

            _tickTimer = _tickTimer % (1 / _healTickRate);
        }
    }

    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        _healPowerBar.fillAmount = (float) newHealPower / _maxHealPower;
    }


}
