using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChampionHUDData : UIBaseData
{
    public Sprite championIcon;
    public int championLevel;
    public int championMaxHP;
    public int championCurrentHP;
    public int championMaxExp;
    public int championCurrentExp;
    public List<int> championSkillList;
}

public class UIChampionHUD : UIBase
{
    [SerializeField] private Image championImage;
    [SerializeField] private TextMeshProUGUI championLevelText;
    [SerializeField] private Image championHPSlider;
    [SerializeField] private TextMeshProUGUI championHPText;
    [SerializeField] private Image championExpSlider;
    [SerializeField] private TextMeshProUGUI championExpText;
    [SerializeField] private Transform skillSlotLayout;

    public override void SetInfo(UIBaseData uidata)
    {
        base.SetInfo(uidata);
        UIChampionHUDData data = uidata as UIChampionHUDData;
        //championImage.sprite = data.championIcon;
        championLevelText.text = data.championLevel.ToString();
        championHPText.text = $"{data.championCurrentHP} / {data.championMaxHP}";
        championHPSlider.fillAmount = data.championCurrentHP / data.championMaxHP;
        championExpText.text = $"{data.championCurrentExp} / {data.championMaxExp}";
        championExpSlider.fillAmount = data.championCurrentExp / data.championMaxExp;
    }

    //TODO : Event 연결 (챔피언 데이터와 동기화)

}
