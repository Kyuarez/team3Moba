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

        // HACK: 타워 구현 때문에 일단 추가..
        float currentHp = gameEntity.GetHP();
        float maxHp = gameEntity.GetHP();
        hpBar.fillAmount = currentHp / maxHp;
    }
}
