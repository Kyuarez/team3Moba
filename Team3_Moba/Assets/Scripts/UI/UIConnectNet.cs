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
        //@tk : �˾� �Ŵ��� (��Ʈ��ũ�� �� �����ǰ�) ����� ������ �׳� �ϳ� ���� �ֱ�
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
