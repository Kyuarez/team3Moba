using System;
using Unity.Netcode;

public enum Team
{
    None,
    Red,
    Blue,
}

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance { get; private set; }

    public Team gameResult = Team.None;

    public Action<Team> OnGameOver;
    public Action<int, int> OnChangedMatchScore;
    public Action<int, int> OnChangedPlayerStat;
    public Action<DateTime> OnUpdateMatchTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(Instance);
        }
        TableManager table = new TableManager();
        table.OnLoadGameAction();
    }

    [Rpc(SendTo.Server)]
    public void ServerSetGameResultRpc(Team loseTeam)
    {
        if(loseTeam == Team.Red)
        {
            ClientsOnGameoverRpc(Team.Blue);
        }
        else if(loseTeam == Team.Blue)
        {
            ClientsOnGameoverRpc(Team.Red);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ClientsOnGameoverRpc(Team team)
    {
        Logger.Log($"½Â¸®ÆÀ : {team}");
        OnGameOver?.Invoke(team);
    }

}
