using System;
using UnityEngine;

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
        float hitRadius = 0.5f; // 적절한 충돌 반경 설정
        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
        foreach (var hit in hits)
        {
            Champion champion = hit.GetComponent<Champion>();
            if (champion != null)
            {
                OnGetExpItem?.Invoke(exp);
                PoolManager.Instance.DespawnObject(poolPath, gameObject);
                //의존성 문제로 수정필요 - 05-22에 경험치 동기화 하면서 수정 예정
                //SpawnManager.Instance.DecreaseExpItemCount();
                break;
            }
        }
    }

    //@tk : 상호작용 영역
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float hitRadius = 0.5f; 
        Gizmos.DrawWireSphere(transform.position, hitRadius); 
    }
}
