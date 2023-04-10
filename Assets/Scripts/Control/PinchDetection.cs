using System.Collections;
using UnityEngine;

public class PinchDetection : MonoBehaviour
{
    private float speed = 1f;
    private TouchControls _controls;

    private Coroutine _zoomCoroutine;

    [SerializeField]
    private RectTransform _content;
    private void Awake()
    {
        _controls = new TouchControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        _controls.Touch.SecondaryTouchContact.started += _ => ZoomStart();
        _controls.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();
        _controls.Touch.PrimaryFingerPosition.canceled += _ => ZoomEnd();
    }

    private void ZoomStart()
    {
        _zoomCoroutine = StartCoroutine(ZoomDetection());
    }

    private void ZoomEnd()
    {
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);
    }

    private IEnumerator ZoomDetection()
    {
        float previoysDistance = Vector2.Distance(_controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(), _controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
        float distance;
        while (true)
        {
            distance = Vector2.Distance(_controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(), _controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());


            if (distance > previoysDistance && _content.localScale.x < 5)
            {
                Vector3 targetScale = _content.localScale;
                targetScale += Vector3.one;
                _content.localScale = Vector3.Slerp(_content.localScale, targetScale, Time.deltaTime * speed);
            }
            else if (distance < previoysDistance && _content.localScale.x >= 0.3f)
            {
                Vector3 targetScale = _content.localScale;
                targetScale -= Vector3.one;
                _content.localScale = Vector3.Slerp(_content.localScale, targetScale, Time.deltaTime * speed);
            }
            previoysDistance = distance;
            yield return null;
        }
    }
}
