using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public interface IBillboardActor
{
    public void Bind(GameEntity gameEntity);
}


//������ ī�޶� ���� ó��
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

        // HACK: Ÿ�� ���� ������ �ϴ� �߰�..
        float currentHp = gameEntity.GetHP();
        float maxHp = gameEntity.GetHP();
        hpBar.fillAmount = currentHp / maxHp;
    }
}
