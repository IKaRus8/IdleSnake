using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI)),ExecuteInEditMode]
public class TextScaler : MonoBehaviour
{
    private TMP_Text text;
    public Vector2 padding;
    public Vector2 maxSize = new Vector2(1000, float.PositiveInfinity);
    public Vector2 minSize;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }


    public enum Mode
    {
        None = 0,
        Horizontal = 0x1,
        Vertical = 0x2,
        Both = Horizontal | Vertical
    }
    public Mode controlAxes = Mode.Both;

    protected string lastText = null;
    protected Vector2 lastSize;
    protected bool forceRefresh = false;

    protected virtual float MinX
    {
        get
        {
            if ((controlAxes & Mode.Horizontal) != 0) return minSize.x;
            return text.rectTransform.rect.width - padding.x;
        }
    }
    protected virtual float MinY
    {
        get
        {
            if ((controlAxes & Mode.Vertical) != 0) return minSize.y;
            return text.rectTransform.rect.height - padding.y;
        }
    }
    protected virtual float MaxX
    {
        get
        {
            if ((controlAxes & Mode.Horizontal) != 0) return maxSize.x;
            return text.rectTransform.rect.width - padding.x;
        }
    }
    protected virtual float MaxY
    {
        get
        {
            if ((controlAxes & Mode.Vertical) != 0) return maxSize.y;
            return text.rectTransform.rect.height - padding.y;
        }
    }

    protected virtual void Update()
    {
        if (text != null && (text.text != lastText || lastSize != text.rectTransform.rect.size || forceRefresh))
        {
            lastText = text.text;
            Vector2 preferredSize = text.GetPreferredValues(MaxX, MaxY);
            preferredSize.x = Mathf.Clamp(preferredSize.x, MinX, MaxX);
            preferredSize.y = Mathf.Clamp(preferredSize.y, MinY, MaxY);
            preferredSize += padding;

            if ((controlAxes & Mode.Horizontal) != 0)
            {
                text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
            }
            if ((controlAxes & Mode.Vertical) != 0)
            {
                text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
            }
            lastSize = text.rectTransform.rect.size;
            forceRefresh = false;
        }
    }

    // Forces a size recalculation on next Update
    public virtual void Refresh()
    {
        forceRefresh = true;
    }

}
