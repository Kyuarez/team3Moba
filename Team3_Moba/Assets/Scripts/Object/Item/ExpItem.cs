using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ExpItem : MonoBehaviour
{
    private int exp;
    private string poolPath;
    private Action<int> OnGetExpItem;

    public void Initialize(string poolPath, int exp, Action<int> OnGetItem)
    {
        this.poolPath = poolPath;
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
                OnGetExpItem?.Invoke(exp);
                PoolManager.Instance.DespawnObject(poolPath, gameObject);
                MatchManager.Instance.DecreaseExpItemCount();
                break;
            }
        }
    }

    //@tk : ��ȣ�ۿ� ����
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float hitRadius = 0.5f; 
        Gizmos.DrawWireSphere(transform.position, hitRadius); 
    }
}
