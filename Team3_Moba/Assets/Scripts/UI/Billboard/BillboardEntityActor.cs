using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public interface IBillboardActor
{
    public void Bind(GameEntity gameEntity);
}


//빌보드 카메라 공동 처리
public class BillboardEntityActor : MonoBehaviour, IBillboardActor
{
    [SerializeField] private Image hpBar;

    public void OnUpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
    }

    public void Bind(GameEntity gameEntity)
    {
        gameEntity.OnHPChanged += OnUpdateHpBar;

        EntityTable table = TableManager.Instance.FindTableData<EntityTable>(gameEntity.GetEntityID());
        float currentHp = table.hp;
        float maxHp = table.hp;
        hpBar.fillAmount = currentHp / maxHp;
    }
}
