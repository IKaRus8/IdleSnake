using System.Collections;
using UnityEngine;

namespace Extensions.Core
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Sets transform's local position with lerp.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="initial"></param>
        /// <param name="final"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static IEnumerator SetPositionWithLerp(this Transform transform, Vector3 initial, Vector3 final, float duration)
        {
            float percentage = 0;
            float timeElapsed = 0;
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                percentage = timeElapsed / duration;
                transform.position = Vector3.Lerp(initial, final, percentage);
                yield return null;
            }
        }

        /// <summary>
        /// Sets transform's local scale with lerp.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="initialScale"></param>
        /// <param name="finalScale"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static IEnumerator ScaleWithLerp(this Transform transform ,Vector3 initialScale, Vector3 finalScale, float duration)
        {
            float timeElapsed = 0;
            float percentage = 0;
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                percentage = timeElapsed / duration;
                transform.localScale = Vector3.Lerp(initialScale, finalScale, percentage);
                yield return null;
            }
        }


        /// <summary>
        /// Sets rotation with lerp.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="initialRotation"></param>
        /// <param name="finalRotation"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static IEnumerator RotateWithLerp(this Transform transform, float initialRotation, float finalRotation, float duration)
        {
            float timeElapsed = 0;
            float percentage = 0;
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                percentage = timeElapsed / duration;
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, initialRotation), Quaternion.Euler(0, 0, finalRotation), percentage);
                yield return null;
            }
        }

        public static IEnumerator SetPositionWithLerpRect(this RectTransform transform, Vector3 initial, Vector3 final, float duration)
        {
            float percentage = 0;
            float timeElapsed = 0;
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                percentage = timeElapsed / duration;
                transform.localPosition = Vector3.Lerp(initial, final, percentage);
                yield return null;
            }
        }
        public static IEnumerator SetWidthHeighWithLerpRect(this RectTransform transform, Vector2 initial, Vector2 final, float duration)
        {
            float percentage = 0;
            float timeElapsed = 0;
            Rect _rect = transform.rect;
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                percentage = timeElapsed / duration;
                transform.sizeDelta = Vector2.Lerp(initial, final, percentage);
                //(_rect.x,_rect.y, Vector2.Lerp(initial, final, percentage).x, Vector2.Lerp(initial, final, percentage).y);
                yield return null;
            }
        }

        public static void LookAt2D(this Transform transform, Vector2 target)
        {
            Vector2 diff = target - (Vector2)transform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
    }
}
