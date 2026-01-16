using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Lean.Pool;

namespace CocoDoogy.EffectPooling
{
    public class TouchEffect : MonoBehaviour
    {
        [Header("Touch Size Settings")]
        public float initialSize = 50f;
        public float finalSize = 75f;
        public float growTime = 1f;
        public bool fadeEffect = true;

        [Header("Color Options")]
        public bool useCustomColor = false;
        public Color customColor = Color.white;

        [Header("Sparkles")]
        public bool emitSparkles = true;
        public float sparkleInitialSize = 50f;
        public float sparkleRandomizeSizeMultiplier = 0.5f;
        public float sparkleFinalSize = 0f;

        public int amountMin = 5;
        public int amountMax = 10;

        public float speedMin = 1f;
        public float speedMax = 3f;

        public bool spinSparkles = true;
        public float spinSpeedMin = -5f;
        public float spinSpeedMax = 5f;

        [Header("Color Options Sparkles")]
        public bool useCustomSparkleColor = false;
        public List<Color> customSparkleColor = new() { Color.white };

        [Header("Prefabs")]
        public GameObject sparklePrefab; // 반드시 할당!

        [Header("Sprites")]
        public Sprite circleSprite;
        public List<Sprite> sparkles;

        [Header("Vibration Settings")]
        public bool useVibration = false;
        public int vibrationStrength = 64;
        public int vibrationDuration = 125;

        [Header("Performance helper")]
        public Image circleImage;
        public CanvasGroup canvasGroup;
        public RectTransform rect;

        private float growStartTime;
        private readonly List<Sparkle> liveSparkles = new();
        private float t;


        private void OnEnable()
        {
            if (circleImage == null) circleImage = GetComponent<Image>();
            if (rect == null) rect = circleImage.rectTransform;
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            rect.sizeDelta = new Vector2(initialSize, initialSize);
            canvasGroup.alpha = 1f;

            circleImage.sprite = circleSprite;
            if (useCustomColor) circleImage.color = customColor;

            // 기존 Sparkle 제거
            DespawnAllSparkles();

            // 새로운 Sparkle 생성
            if (sparkles.Count > 0 && emitSparkles)
                EmitSparkles();

            StartCoroutine(DelayGrowStart());
        }

        private IEnumerator<WaitForEndOfFrame> DelayGrowStart()
        {
            yield return new WaitForEndOfFrame();
            growStartTime = Time.unscaledTime;
        }

        private void Update()
        {
            t = Mathf.Clamp01((Time.unscaledTime - growStartTime) / growTime);

            rect.sizeDelta = Vector2.Lerp(
                new Vector2(initialSize, initialSize),
                new Vector2(finalSize, finalSize),
                t
            );

            if (fadeEffect)
                canvasGroup.alpha = 1f - t;

            if (t >= 1f)
            {
                DespawnAllSparkles();
                LeanPool.Despawn(gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (liveSparkles.Count > 0)
                MoveSparkles();
        }


        private void EmitSparkles()
        {
            int amount = Random.Range(amountMin, amountMax + 1);

            for (int i = 0; i < amount; i++)
            {
                GameObject sGO = LeanPool.Spawn(sparklePrefab, transform);
                RectTransform r = sGO.GetComponent<RectTransform>();
                Image img = sGO.GetComponent<Image>();
                Sparkle s = sGO.GetComponent<Sparkle>();

                float rad = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float dx = Mathf.Cos(rad);
                float dy = Mathf.Sin(rad);

                r.anchoredPosition = new Vector2(dx * initialSize / 2f, dy * initialSize / 2f);

                img.sprite = sparkles[Random.Range(0, sparkles.Count)];

                if (useCustomSparkleColor)
                    img.color = customSparkleColor[Random.Range(0, customSparkleColor.Count)];

                float randomSize = Random.Range(
                    sparkleInitialSize,
                    sparkleInitialSize * sparkleRandomizeSizeMultiplier
                );

                s.initialSize = new Vector2(randomSize, randomSize);
                s.finalSize = new Vector2(sparkleFinalSize, sparkleFinalSize);
                r.sizeDelta = s.initialSize;

                s.direction = new Vector2(dx, dy);
                s.speed = Random.Range(speedMin, speedMax);
                s.rect = r;
                s.image = img;
                s.spinSpeed = new Vector3(0, 0, Random.Range(spinSpeedMin, spinSpeedMax));

                liveSparkles.Add(s);
            }
        }


        private void MoveSparkles()
        {
            foreach (Sparkle s in liveSparkles)
            {
                s.rect.anchoredPosition += s.direction * s.speed;
                s.rect.sizeDelta = Vector2.Lerp(s.initialSize, s.finalSize, t);

                if (spinSparkles)
                    s.rect.Rotate(s.spinSpeed);
            }
        }


        private void DespawnAllSparkles()
        {
            foreach (Sparkle s in liveSparkles)
            {
                if (s != null)
                    LeanPool.Despawn(s.gameObject);
            }
            liveSparkles.Clear();
        }
    }
}
