using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.EffectPooling
{
    public class Sparkle : MonoBehaviour
    {
        public Vector2 direction;
        public float speed;
        public RectTransform rect;
        public Image image;
        public Vector2 initialSize;
        public Vector2 finalSize;
        public Vector3 spinSpeed;

        public void ResetSparkle()
        {
            direction = Vector2.zero;
            speed = 0;
        }
    }
}
