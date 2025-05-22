using System;
using Unity.Netcode;
using UnityEngine;

public class ExpItem : NetworkBehaviour
{
    private int exp;
    private Action<int> OnGetExpItem;

    public void Initialize(int exp, Action<int> OnGetItem)
    {
        this.exp = exp;
        this.OnGetExpItem = OnGetItem;
    }

    private void Update()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        float hitRadius = 0.5f; // ������ �浹 �ݰ� ����
        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
        foreach (var hit in hits)
        {
            Champion champion = hit.GetComponent<Champion>();
            if (champion != null)
            {
                //TODO
            }
        }
    }
}
