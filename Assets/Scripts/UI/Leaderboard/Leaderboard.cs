using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform _leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay _leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntitySate> _leaderboardEntities;

    private void Awake()
    {
        _leaderboardEntities = new NetworkList<LeaderboardEntitySate>();
    }
}
