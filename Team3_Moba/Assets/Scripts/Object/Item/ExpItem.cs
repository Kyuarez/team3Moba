using System;
using Unity.Netcode;
using UnityEngine;

public class ExpItem : NetworkBehaviour
{
    [SerializeField] private int exp;
    public Action OnDespawnExpItem;

    private void Update()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        float hitRadius = 0.5f; // 적절한 충돌 반경 설정
        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
        foreach (var hit in hits)
        {
            Champion champion = hit.GetComponent<Champion>();
            if (champion != null)
            {
                ServerGetItemRpc(champion.NetworkObjectId);
            }
        }
    }
    [Rpc(SendTo.Server)]
    public void ServerGetItemRpc(ulong networkObjectID)
    {
        if (!IsServer) return; // 삭제해도 될 것 같은 부분?

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectID, out var championObject))
        {
            Champion champion = championObject.GetComponent<Champion>();
            if (champion != null)
            {
                ClientsGetItemRpc(networkObjectID, championObject.OwnerClientId);
                DespawnItemRpc();
            }
        }
    }
    [Rpc(SendTo.Everyone)]
    public void ClientsGetItemRpc(ulong networkObjectID, ulong targetClientID)
    {
        if (NetworkManager.Singleton.LocalClientId == targetClientID)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectID, out var championObject))
            {
                Champion champion = championObject.GetComponent<Champion>();
                if (champion != null)
                {
                    EffectManager.Instance.PlayEffect(5, gameObject.transform.position, new Vector3(1, 1, 1), Quaternion.identity);
                    champion.OnGetExpItem(exp);
                }
            }
        }
    }
    [Rpc(SendTo.Server)]
    private void DespawnItemRpc()
    {
        OnDespawnExpItem?.Invoke();
        NetworkObject.Despawn();
    }


}
