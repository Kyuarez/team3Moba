using UnityEngine;
using UnityEngine.UI;

public enum UISlotType
{
    Item, //info tooltip
    Skill, //key, cooltime
    Buff, //cooltime, poolable
}

public class UISlotBaseData
{
    public UISlotType slotType; //@tk Ÿ�� ���� ũ�� ����
    public Sprite slotIcon;
}

public class UISlotBase : MonoBehaviour
{
    protected UISlotType slotType;
    public Image slotIcon;

    public virtual void SetInfo(UISlotBaseData slotData)
    {
        this.slotType = slotData.slotType;
        this.slotIcon.sprite = slotData.slotIcon;
    }


}
