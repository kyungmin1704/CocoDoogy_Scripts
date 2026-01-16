using CocoDoogy.Data;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CocoDoogy.Utility.Loading
{
    public class Loading : MonoBehaviour
    {
        private static string nextScene;
        
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private Image loadingBar;
        
        [SerializeField] private LoadingTextData loadingTextData;
        [SerializeField] private TextMeshProUGUI tips;

        public static void LoadScene(string sceneName)
        {
            nextScene = sceneName;
            SceneManager.LoadScene("Loading");
        }

        void Start()
        {
            loadingBar.fillAmount = 0;
            StartCoroutine(LoadingProcess());
            
            string randomtips = loadingTextData.texts[Random.Range(0, loadingTextData.texts.Length)];
            tips.text = $"Tips.{randomtips}";
        }

        IEnumerator LoadingProcess()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
            op.allowSceneActivation = false;

            float loadingTimer = 0f;
            float additionalLoadingTime = 2f;
            
            while (!op.isDone)
            {
                yield return null;
                
                int loadingPercent = Mathf.RoundToInt(loadingBar.fillAmount * 100);
                loadingText.text = $"{loadingPercent}%";

                if (op.progress < 0.9f)
                {
                    loadingBar.fillAmount = op.progress;
                }
                else
                {
                    loadingTimer += Time.unscaledDeltaTime / additionalLoadingTime;
                    loadingBar.fillAmount = Mathf.Lerp(0.9f, 1, loadingTimer);
                    if (loadingBar.fillAmount >= 1)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
    }
}
