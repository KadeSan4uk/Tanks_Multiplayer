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

    private List<TankPlayer> _playersInZone = new List<TankPlayer>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        _playersInZone.Add(player);

        Debug.Log($"Entered: {player.playerName.Value}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        _playersInZone.Remove(player);

        Debug.Log($"Left: {player.playerName.Value}");
    }



}
