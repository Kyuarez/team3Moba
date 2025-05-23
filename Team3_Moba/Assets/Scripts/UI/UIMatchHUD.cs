using System;
using System.Data;
using TMPro;
using UnityEngine;

public class UIMatchHUDData : UIBaseData
{
    public string teamScoreText;
    public string playerStatText;
    public string timerText;
}

public class UIMatchHUD : UIBase
{
    [SerializeField] private TextMeshProUGUI teamScoreText;
    [SerializeField] private TextMeshProUGUI playerStatText;
    [SerializeField] private TextMeshProUGUI timerText;

    public override void Initialize(Transform anchor)
    {
        base.Initialize(anchor);
        MatchManager.Instance.OnChangedMatchScore += OnChangeMatchScore;
        MatchManager.Instance.OnUpdateMatchTimer += OnUpdateMatchTimer;

        OnClose += OnUnRegisterMatchEvent;
    }

    public override void SetInfo(UIBaseData uidata)
    {
        base.SetInfo(uidata);
        UIMatchHUDData data = uidata as UIMatchHUDData;
        teamScoreText.text = data.teamScoreText;
        playerStatText.text = data.playerStatText;
        timerText.text = data.timerText;
    }
    public void OnUnRegisterMatchEvent()
    {
        MatchManager.Instance.OnChangedMatchScore -= OnChangeMatchScore;
        MatchManager.Instance.OnUpdateMatchTimer -= OnUpdateMatchTimer;
    }
    public void OnChangeMatchScore(int redScore, int blueScore)
    {
        teamScoreText.text = $"<color=red>{redScore}</color> vs <color=blue>{blueScore}</color>";
    }

    public void OnChangedPlayerStat(int playerKillCount, int playerDeadCount)
    {
        playerStatText.text = $"{playerKillCount} / {playerDeadCount}";
    }

    public void OnUpdateMatchTimer(DateTime dateTime)
    {
        timerText.text = dateTime.ToString("mm:ss");
    }

}
