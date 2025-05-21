using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public enum Team
{
    None,
    Red,
    Blue,
}

public class MatchManager : MonoNetSingleton<MatchManager>
{

    public Action<int, int> OnChangedMatchScore;
    public Action<int, int> OnChangedPlayerStat;
    public Action<DateTime> OnUpdateMatchTimer;


    protected override void Awake()
    {
        base.Awake();
        TableManager table = new TableManager();
        table.OnLoadGameAction();

    }

    private void Start()
    {


    }

    private void Update()
    {

    }

    public void OnGameResult(Team team)
    {
        if (IsHost)
        {
            if (team == Team.Red)
            {
                Logger.Log("ÆÐ¹è");
            }
            else 
            {
                Logger.Log("½Â¸®");
            }
        }
        else if (IsClient)
        {
            if (team == Team.Blue)
            {
                Logger.Log("ÆÐ¹è");
            }
            else 
            {
                Logger.Log("½Â¸®");
            }
        }
    }


}
