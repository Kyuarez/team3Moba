using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField ipInputField;

    public override void Initialize(Transform anchor)
    {
        base.Initialize(anchor);
        connectPanel.SetActive(true);
        waitingPanel.SetActive(false);

        hostButton.onClick.AddListener(OnClickConnectHost);
        clientButton.onClick.AddListener(OnClickConnectClient);
        ipInputField.onEndEdit.AddListener(OnEndEditIpAddress);
    }

    public void OnEndEditIpAddress(string ipAddress)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
#if UNITY_EDITOR
        transport.SetConnectionData("127.0.0.1", 7777);
#else
        transport.SetConnectionData(ipAddress, 7777);
#endif
    }

    public void OnClickConnectClient()
    {
        NetworkManager.Singleton.StartClient();
        WaitingWithConnect();
    }

    public void OnClickConnectHost()
    {
        NetworkManager.Singleton.StartHost();
        WaitingWithConnect();
    }

    public void WaitingWithConnect()
    {
        connectPanel.SetActive(false);
        waitingPanel.SetActive(true);
    }
}
