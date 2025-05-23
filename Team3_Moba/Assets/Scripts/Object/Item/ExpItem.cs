using System;
using Unity.Netcode;
using UnityEngine;

public class ExpItem : NetworkBehaviour
{
    [SerializeField] private int exp;

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
        if (!IsServer) return;

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
                    champion.OnGetExpItem(exp);
                }
            }
        }
    }
    [Rpc(SendTo.Server)]
    private void DespawnItemRpc()
    {
        NetworkObject.Despawn();
    }


}
