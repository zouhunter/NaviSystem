using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace NaviSystem
{
    /// <summary>
    /// 新手引导动画
    /// </summary>
    public class NaviMask : MonoBehaviour
    {
        public Material material { get; set; }
        public RectTransform rootRect { get; set; }

        private RectTransform target;
        private Vector4 center;
        private float diameter; // 直径
        private float current = 0f;
        float yVelocity = 0f;
        Vector3[] corners = new Vector3[4];
        private float startDia;
        private bool delyMove;
        public void MoveToNode(RectTransform target)
        {
            Debug.Assert(target != null,"target is null!");
            this.target = target;
            delyMove = true;
        }

        public void WarningCurrentNode()
        {
            rootRect.GetWorldCorners(corners);
            for (int i = 0; i < corners.Length; i++){
                current = Mathf.Max(Vector3.Distance(WordToCanvasPos(rootRect, corners[i]), center), current);
            }
            current *= 0.2f;
            startDia = current;
        }


        void Update()
        {
            if (target == null) return;

            UpdateCenter();

            if (delyMove)
            {
                delyMove = false;
                MoveInternal();
            }

            float value = Mathf.SmoothDamp(current, diameter, ref yVelocity, 0.3f);
            if (!Mathf.Approximately(value, current))
            {
                current = value;
                material.SetFloat("_Silder", current);
            }
        }
        void MoveInternal()
        {
            rootRect.GetWorldCorners(corners);
            for (int i = 0; i < corners.Length; i++){
                current = Mathf.Max(Vector3.Distance(WordToCanvasPos(rootRect, corners[i]), center), current);
            }
            current *= 0.5f;
        }

        void UpdateCenter()
        {
            Debug.Assert(corners != null, "coners is Null");
            Debug.Assert(target != null, "target is Null");
            target.GetWorldCorners(corners);
            diameter = Vector2.Distance(WordToCanvasPos(rootRect, corners[0]), WordToCanvasPos(rootRect, corners[2])) / 2f;
            float x = corners[0].x + ((corners[3].x - corners[0].x) / 2f);
            float y = corners[0].y + ((corners[1].y - corners[0].y) / 2f);
            center = new Vector3(x, y, 0f);
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, center, rootRect.GetComponent<Camera>(), out position);

            center = new Vector4(position.x, position.y, 0f, 0f);
            material.SetVector("_Center", center);
        }
        static Vector2 WordToCanvasPos(RectTransform canvas, Vector3 world)
        {
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, world, canvas.GetComponent<Camera>(), out position);
            return position;
        }

        private static float spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

    }
}
