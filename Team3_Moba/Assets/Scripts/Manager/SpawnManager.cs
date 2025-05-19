using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    private Transform playerTransform;
    private Champion playerChampion;


    private Vector3 spawnRedTeamPosition = new Vector3(19f, 6f, 5f);
    private Vector3 spawnBlueTeamPosition = new Vector3(-135f, 6f, -140f);

    public Transform PlayerTransform => playerTransform;

    private void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {

        base.OnNetworkSpawn();

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            var localPlayerObject = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (localPlayerObject != null)
            {
                playerChampion = localPlayerObject.GetComponent<Champion>();
                playerTransform = playerChampion.transform;
                playerChampion.OnDeadComplete += OnChampionDeadComplete;

                if (playerChampion.IsLocalPlayer)
                {
                    if (IsHost)
                    {
                        playerChampion.SetTeam(Team.Red);
                        playerChampion.transform.position = spawnRedTeamPosition;
                    }
                    else
                    {
                        playerChampion.SetTeam(Team.Blue);
                        playerChampion.transform.position = spawnBlueTeamPosition;
                    }

                }

            }
            else
            {
                Logger.LogError("Local PlayerObject not found after client connected!");
            }
        };


        SetMatchUI();
    }


    public void OnChampionDeadComplete()
    {
        //
        if (playerChampion.GetTeam() == Team.Red)
        {
            playerChampion.transform.position = spawnRedTeamPosition;
        }
        else if (playerChampion.GetTeam() == Team.Blue)
        {
            playerChampion.transform.position = spawnBlueTeamPosition;
        }
    }

    private void SetMatchUI()
    {
        UIMatchHUDData matchHUD = new UIMatchHUDData();
        matchHUD.teamScoreText = "<color=red>0</color> vs <color=blue>0</color>";
        matchHUD.playerStatText = "0 / 0";
        matchHUD.timerText = "00:00";
        UIManager.Instance.OpenUI<UIMatchHUD>(matchHUD);
        UIChampionHUDData championHUD = new UIChampionHUDData();
        championHUD.champion = playerChampion;
        UIManager.Instance.OpenUI<UIChampionHUD>(championHUD);
    }
}
