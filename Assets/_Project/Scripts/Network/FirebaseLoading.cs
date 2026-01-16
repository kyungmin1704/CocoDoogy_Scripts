using DG.Tweening;
using Lean.Pool;
using TMPro;
using UnityEngine;

namespace CocoDoogy.Network
{
    public class FirebaseLoading : MonoBehaviour
    {
        private static FirebaseLoading prefab;
        private static Transform canvas;

        [SerializeField] private TextMeshProUGUI loadingText;

        private Tweener dotTween;

        private void Awake()
        {
            dotTween?.Kill();
        }

        private void Show()
        {
            PlayAnimation();
        }

        public void Hide()
        {
            dotTween?.Kill();
            LeanPool.Despawn(this);
        }

        private void PlayAnimation()
        {
            dotTween = DOTween.To(() => 0, x =>
                {
                    loadingText.text = "로딩중" + new string('.', x);
                }, 3, 1f)
                .SetLoops(-1, LoopType.Restart);
        }

        private static FirebaseLoading Create()
        {
            if (!canvas)
                canvas = GameObject.Find("PopupCanvas").transform;

            FirebaseLoading result = LeanPool.Spawn(prefab, canvas);
            return result;
        }

        public static FirebaseLoading ShowLoading()
        {
            var loading = Create();
            loading.Show();
            return loading;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            prefab = Resources.Load<FirebaseLoading>("UI/LoadingUI");
        }
    }
}
