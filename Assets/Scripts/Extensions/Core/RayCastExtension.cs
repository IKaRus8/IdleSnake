using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Core
{
    public static class RayCastExtension 
    {
        public static void SetBlockingMask(this GraphicRaycaster gRaycaster, int maskLayer)
        {
            if (gRaycaster != null)
            {
                var fieldInfo = gRaycaster.GetType().GetField("m_BlockingMask", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    LayerMask layerMask = new LayerMask();
                    layerMask.value = maskLayer;
                    fieldInfo.SetValue(gRaycaster, layerMask);
                }
            }
        }
    }
}
