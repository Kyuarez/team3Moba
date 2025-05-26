using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectNetData : UIBaseData
{
    //
}

public class UIConnectNet : UIBase
{
    [SerializeField] private GameObject _connectPanel;
    [SerializeField] private GameObject _waitingPanel;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        _connectPanel.SetActive(true);
        _waitingPanel.SetActive(false);

        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            _connectPanel.SetActive(false);
            _waitingPanel.SetActive(true);
        });

        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            _connectPanel.SetActive(false);
            _waitingPanel.SetActive(true);
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            _connectPanel.SetActive(false);
            _waitingPanel.SetActive(true);
        });

        MatchManager.Instance.OnGameStart += () =>
        {
            UIManager.Instance.CloseUI(this);
        };
    }

}
