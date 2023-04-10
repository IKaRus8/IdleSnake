using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    public GameObject cellObject;
    public SpriteRenderer cellSprite;



    public void InitDefault()
    {
        cellObject = gameObject;
        cellSprite = cellObject.GetComponent<SpriteRenderer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Managers.BoostManager.isFullControl) return;
        Debug.Log("Move snake");
        Managers.FieldManager.Instance.StartMoveDirection(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
