using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIResultData : UIBaseData
{
    public bool isWin;
}

public class UIResult : UIBase
{

    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button exitButton;

    public override void Initialize(Transform anchor)
    {
        base.Initialize(anchor);
        exitButton.onClick.AddListener(OnClickExitButton);
    }


    public override void SetInfo(UIBaseData baseData)
    {
        UIResultData data = baseData as UIResultData;

        if(data.isWin == true)
        {
            resultText.text = "Victory!";
        }
        else
        {
            resultText.text = "Lose :/";
        }
    }
    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(this);
        
        //@tk : �κ� ��� �����Ǹ�, ���� Ŭ���̾�Ʈ ���� ��ġ ������ ó�� 
        //������ �κ� ��� �ٷ� �� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
