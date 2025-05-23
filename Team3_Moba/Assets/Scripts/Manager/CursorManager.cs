using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public enum CursorState
{
    None,
    Base,
    Attack,
    Target,
}

//付快胶 目辑 包府
public class CursorManager : MonoBehaviour
{
    [SerializeField] private Team playerTeam;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private Sprite baseCursorSprite;
    [SerializeField] private Sprite attackCursorSprite;
    [SerializeField] private Sprite targetCursorSprite;


    private Image cursorImage;
    private CursorState currentState;

    private void Awake()
    {
        if(cursor.gameObject.activeSelf == false)
        {
            cursor.gameObject.SetActive(true);
        }

        Cursor.visible = false;

        currentState = CursorState.None;
        cursorImage = cursor.GetComponent<Image>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.LocalClient?.PlayerObject != null);
        var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Champion>();
        if (player != null) 
        {
            playerTeam = player.GetTeam();
            SetCursorState(CursorState.Base);
        }
    }

    private void Update()
    {
        FollowCursor();

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            UpdateCursorState(hit);
        }

    }

    private void FollowCursor()
    {
        cursor.position = Input.mousePosition;
    }

    private void UpdateCursorState(RaycastHit hit)
    {   
        if (currentState == CursorState.None)
        {
            SetCursorState(CursorState.Base);
            return;
        }

        if (SkillManager.Instance.CheckReservationSkill())
        {
            SetCursorState(CursorState.Target);
            return;
        }

        GameEntity entity = hit.collider.gameObject.GetComponent<GameEntity>();
        if (entity != null && playerTeam != entity.GetTeam())
        {
            SetCursorState(CursorState.Attack);
            return;
        }

        SetCursorState(CursorState.Base);
    }

    public void SetCursorState(CursorState nextState)
    {
        if(currentState == nextState)
        {
            return;
        }

        currentState = nextState;
        switch (currentState)
        {
            case CursorState.Base:
                cursorImage.sprite = baseCursorSprite;
                break;
            case CursorState.Attack:
                cursorImage.sprite = attackCursorSprite;
                break;
            case CursorState.Target:
                cursorImage.sprite = targetCursorSprite;
                break;
            default:
                break;
        }
    }

    
}
