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
    [SerializeField] private Sprite victorySprite;
    [SerializeField] private Sprite defeatSprite;

    [SerializeField] private Image resultImage;
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
            SoundManager.Instance.PlaySFX(10); //승리 사운드 재생
            resultImage.sprite = victorySprite;
        }
        else
        {
            SoundManager.Instance.PlaySFX(11); //패배 사운드 재생
            resultImage.sprite = defeatSprite;
        }
    }
    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(this);
        
        //@tk : 로비 기능 생성되면, 개별 클라이언트 별로 매치 나가게 처리 
        //지금은 로비가 없어서 바로 앱 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
