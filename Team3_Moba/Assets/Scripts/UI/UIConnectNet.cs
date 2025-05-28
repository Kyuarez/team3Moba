using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectNetData : UIBaseData
{
    //
}

public class UIConnectNet : UIBase
{
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private GameObject waitingPanel;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    public override void Initialize(Transform anchor)
    {
        base.Initialize(anchor);
        connectPanel.SetActive(true);
        waitingPanel.SetActive(false);

        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            WaitingWithConnect();
        });

        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            WaitingWithConnect();
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            WaitingWithConnect();
        });
    }

    public void WaitingWithConnect()
    {
        connectPanel.SetActive(false);
        waitingPanel.SetActive(true);
    }
}
