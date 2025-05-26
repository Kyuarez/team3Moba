using System;
using Unity.Netcode;
using UnityEngine;

public enum Team
{
    None,
    Red,
    Blue,
}

//@tk ¸ÅÄ¡ÀÇ ½ÂÆÐ ¹× ÇöÀç ÆÀÀÇ Å³µî °ü¸®
public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance { get; private set; }

    [SerializeField] private Champion _playerPrefab;

    public Team gameResult = Team.None;

    private NetworkVariable<int> redTeamKills = new NetworkVariable<int>();
    private NetworkVariable<int> blueTeamKills = new NetworkVariable<int>();
    private NetworkVariable<float> matchTime = new NetworkVariable<float>();

    public Action<Team> OnGameOver;
    public Action<int, int> OnChangedMatchScore;
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

    public event Action OnGameStart;

    private void Start()
    {
        UIConnectNetData connectUI = new UIConnectNetData();
        UIManager.Instance.OpenUI<UIConnectNet>(connectUI);

        NetworkManager.Singleton.OnConnectionEvent += (networkManager, connectionEventData) =>
        {
            if (networkManager.ConnectedClients.Count == 2)
            {
                OnGameStart?.Invoke();

                if (NetworkManager.Singleton.IsServer)
                {
                    foreach (var client in NetworkManager.Singleton.ConnectedClients)
                    {
                        var instance = Instantiate(_playerPrefab);
                        var networkObject = instance.GetComponent<NetworkObject>();
                        networkObject.SpawnAsPlayerObject(client.Key);
                    }
                }
            }
        };
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        matchTime.Value += Time.deltaTime;
    }

    public override void OnNetworkSpawn()
    {
        redTeamKills.OnValueChanged += (prev, next) =>
        {
            OnChangedMatchScore?.Invoke(next, blueTeamKills.Value);
        };
        blueTeamKills.OnValueChanged += (prev, next) =>
        {
            OnChangedMatchScore?.Invoke(redTeamKills.Value, next);
        }; 
        matchTime.OnValueChanged += (prev, next) =>
        {
            OnUpdateMatchTimer?.Invoke(GetMatchTime(next));
        };
    }   

    [Rpc(SendTo.Server)]
    public void ServerUpdateTeamKillRpc(Team team)
    {
        if (team == Team.Red)
        {
            redTeamKills.Value++;
        }
        else if (team == Team.Blue)
        {
            blueTeamKills.Value++;
        }
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

    public DateTime GetMatchTime(float matchTime)
    {
        return DateTime.MinValue.AddSeconds(matchTime);
    }
}
