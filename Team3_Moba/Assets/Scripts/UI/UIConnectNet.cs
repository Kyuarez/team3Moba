using System;
using System.Net.Sockets;
using System.Net;
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
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private Button titleButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TextMeshProUGUI hostIPText;


    public override void Initialize(Transform anchor)
    {
        base.Initialize(anchor);
        titlePanel.SetActive(true);
        connectPanel.SetActive(false);
        waitingPanel.SetActive(false);

        titleButton.onClick.AddListener(OnClickTitleButton);
        hostButton.onClick.AddListener(OnClickConnectHost);
        clientButton.onClick.AddListener(OnClickConnectClient);
        ipInputField.onEndEdit.AddListener(OnEndEditIpAddress);
    }

    public void OnEndEditIpAddress(string ipAddress)
    {
#if UNITY_EDITOR
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = "127.0.0.1";
#else
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipAddress;
#endif
    }

    public void OnClickTitleButton()
    {
        titlePanel.SetActive(false);
        connectPanel.SetActive(true);
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
        string ipText = string.Empty;
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipText = ip.ToString();
            }
        }
        connectPanel.SetActive(false);
        waitingPanel.SetActive(true);
        hostIPText.text = $"Host IP: {ipText}";
    }
}
