using System;
using Unity.Netcode;
using UnityEngine;

public enum Team
{
    None,
    Red,
    Blue,
}

//@tk ��ġ�� ���� �� ���� ���� ų�� ����
public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance { get; private set; }

    [SerializeField] private Champion _playerPrefab;

    public Team gameResult = Team.None;

    private NetworkVariable<int> redTeamKills = new NetworkVariable<int>();
    private NetworkVariable<int> blueTeamKills = new NetworkVariable<int>();
    private NetworkVariable<float> matchTime = new NetworkVariable<float>();

    public Action<Team, bool> OnGameOver; //�¸���, ���� Ŭ���̾�Ʈ�� �̰����
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
                    //�÷��̾� ����
                    foreach (var client in NetworkManager.Singleton.ConnectedClients)
                    {
                        var instance = Instantiate(_playerPrefab);
                        var networkObject = instance.GetComponent<NetworkObject>();
                        networkObject.SpawnAsPlayerObject(client.Key);
                    }
                }
                SoundManager.Instance.PlayBGM(1);
                SoundManager.Instance.PlayBGM(8);
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

    //[Rpc(SendTo.Everyone)]
    //public void ClientsStartGameRpc()
    //{
    //}

    /// <summary>
    /// parameter team : ���� ����� ��
    /// </summary>
    [Rpc(SendTo.Server)]
    public void ServerUpdateTeamKillRpc(Team team)
    {
        if (team == Team.Red)
        {
            blueTeamKills.Value++;
        }
        else if (team == Team.Blue)
        {
            redTeamKills.Value++;
        }
    }

    [Rpc(SendTo.Server)]
    public void ServerSetGameResultRpc(Team loseTeam)
    {
        if(loseTeam == Team.Red)
        {
            ClientsOnGameoverRpc(Team.Blue);
            SoundManager.Instance.PlaySFX(11); //�й� ���� ���
        }
        else if(loseTeam == Team.Blue)
        {
            ClientsOnGameoverRpc(Team.Red);
            SoundManager.Instance.PlaySFX(11); //�й� ���� ���
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ClientsOnGameoverRpc(Team winTeam)
    {
        Logger.Log($"�¸��� : {winTeam}");
        bool isWin = false;
        if (IsHost)
        {
            isWin = (winTeam == Team.Red);
            SoundManager.Instance.PlaySFX(10); //�й� ���� ���
        }
        else if (IsClient)
        {
            isWin = (winTeam == Team.Blue);
            SoundManager.Instance.PlaySFX(10); //�й� ���� ���
        }

        OnGameOver?.Invoke(winTeam, isWin);
    }

    public DateTime GetMatchTime(float matchTime)
    {
        return DateTime.MinValue.AddSeconds(matchTime);
    }

}
