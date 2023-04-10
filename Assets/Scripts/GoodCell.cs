using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoodCell : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    public GameObject goodObject;
    public int hp=1;
    private GameObject _illumination;
    [SerializeField]
    private int point =1;

    private static readonly int IsDestroy = Animator.StringToHash("isDestroy");


    public void InitDefault()
    {
        goodObject = gameObject;
        _illumination = transform.GetComponentsInChildren<Transform>(true)[1].gameObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Hp: "+hp);
        hp=hp-1-(AncestorsManager.grades.isOpen[1]?1:0);
        if (hp > 0) return;
        _illumination.SetActive(false);
        FieldManager.Instance.RemoveGood(gameObject);
        GetComponent<Animator>().SetBool(IsDestroy, true);
    }

    public void RemoveObject()
    {
        UpgradesManager.AllCoins += point;
        var text = Instantiate(LevelGrowManager.pointPrefab, transform.position, new Quaternion());
        text._text.text = point.ToString();
        text._text.color = Color.blue;
        UIManager.Instance.UpdateCoinValue();
        Destroy(gameObject);
    }

    public void StartStan()
    {
        _illumination.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
