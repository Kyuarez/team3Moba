using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BillboardChampionActor : MonoBehaviour, IBillboardActor
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI levelText;

    public void OnUpdateLevel(int currentLevel)
    {
        levelText.text = currentLevel.ToString();
    }

    public void OnUpdateHP(float currentHP, float maxHP)
    {
        hpBar.fillAmount = currentHP / maxHP;
    }

    public void OnUpdateExp(float currentExp, float maxExp)
    {
        expBar.fillAmount = currentExp / maxExp;
    }
    public void Bind(GameEntity gameEntity)
    {
        Champion champion = gameEntity as Champion;
        if(champion != null)
        {
            champion.OnLevelChanged += OnUpdateLevel;
            champion.OnHPChanged += OnUpdateHP;
            champion.OnExpChanged += OnUpdateExp;
        }

        // HACK: 타워 구현 때문에 일단 추가..
        float currentHp = champion.GetHP();
        float maxHp = champion.GetHP();
        hpBar.fillAmount = currentHp / maxHp;
        float currentExp = champion.CurrentExp;
        float maxExp = champion.MaxExp;
        expBar.fillAmount = currentExp / maxExp;
        levelText.text = champion.CurrentLevel.ToString();
    }
}
