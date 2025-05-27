using UnityEngine;

public class UIUtil 
{
    public static void OnMatchWithChampion(Champion champion)
    {
        UIMatchHUDData matchHUD = new UIMatchHUDData();
        matchHUD.teamScoreText = "<color=red>0</color> vs <color=blue>0</color>";
        matchHUD.playerStatText = "0 / 0";
        matchHUD.timerText = "00:00";
        UIManager.Instance.OpenUI<UIMatchHUD>(matchHUD);
        UIChampionHUDData championHUD = new UIChampionHUDData();
        championHUD.champion = champion;
        UIManager.Instance.OpenUI<UIChampionHUD>(championHUD);
    }

    public static void SetCursorTeam(Team team)
    {
        CursorManager cursorManager = Object.FindFirstObjectByType<CursorManager>();
        if (cursorManager != null)
        {
            cursorManager.SetTeam(team);
        }
    }
}
