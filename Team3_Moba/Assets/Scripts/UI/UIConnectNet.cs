using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class UIConnectNet : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        //@tk : 팝업 매니저 (네트워크와 잘 연동되게) 만들기 전에는 그냥 하나 씬에 넣기
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Destroy(gameObject);
        });

        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            Destroy(gameObject);
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Destroy(gameObject);
        });
    }
}
