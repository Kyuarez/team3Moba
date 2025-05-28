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

        ChampionTable table = TableManager.Instance.FindTableData<ChampionTable>(champion.GetEntityID());
        LevelTable levelTable = TableManager.Instance.FindTableData<LevelTable>(1);
        float currentHp = table.hp;
        float maxHp = table.hp;
        hpBar.fillAmount = currentHp / maxHp;
        float currentExp = 0;
        float maxExp = levelTable.require_exp;
        expBar.fillAmount = currentExp / maxExp;
        levelText.text = champion.CurrentLevel.ToString();
    }
}
