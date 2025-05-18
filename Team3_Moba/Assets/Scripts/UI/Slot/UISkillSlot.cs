using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillSlotData : UISlotBaseData
{
    public Champion champion;
    public SkillTable skillTable;
    public SkillInputType skillInputType;
}

public class UISkillSlot : UISlotBase
{
    private string skillKey;

    public Image coolTimeImage;
    public TextMeshProUGUI coolTimeText;
    public TextMeshProUGUI keyText;

    public override void SetInfo(UISlotBaseData slotData)
    {
        base.SetInfo(slotData);
        UISkillSlotData skillSlotData = slotData as UISkillSlotData;

        skillKey = skillSlotData.skillTable.skill_name;
        coolTimeImage.fillAmount = 0f;
        coolTimeText.text = string.Empty;
        keyText.text = skillSlotData.skillInputType.ToString();

        skillSlotData.champion.PlayerCoolTime.OnCoolTimeUpdate += OnCoolTimeUpdate;        
    }

    public void OnCoolTimeUpdate(CoolTimeData coolTimeData)
    {
        if(skillKey != coolTimeData.key)
        {
            return;
        }

        if(coolTimeData.IsOver == true)
        {
            coolTimeImage.fillAmount = 0f;
            coolTimeText.text = string.Empty;
            return;
        }

        coolTimeImage.fillAmount = coolTimeData.RemainingTime / coolTimeData.duration;
        coolTimeText.text = coolTimeData.RemainingTime.ToString("F0");
    }
}
