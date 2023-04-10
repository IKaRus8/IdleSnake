using TMPro;
using UnityEngine;

public class TextEatFruit : MonoBehaviour
{
    public TextMeshPro _text;
   

    // Update is called once per frame
    private void Update()
    {
        transform.position += Vector3.up*0.01f;
        _text.color -= new Color(0, 0, 0, 0.01f);
        if (_text.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
