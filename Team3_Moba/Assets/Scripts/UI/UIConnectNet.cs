using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectNetData : UIBaseData
{
    //
}

public class UIConnectNet : UIBase
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(() => {
            //���� ó�� ���� (������ ���Ѿ� �Ѵ�.)
            UIManager.Instance.CloseUI(this);
            NetworkManager.Singleton.StartHost();
        });

        serverButton.onClick.AddListener(() => {
            UIManager.Instance.CloseUI(this);
            NetworkManager.Singleton.StartServer();
        });

        clientButton.onClick.AddListener(() => {
            UIManager.Instance.CloseUI(this);
            NetworkManager.Singleton.StartClient();
        });
    }
}
