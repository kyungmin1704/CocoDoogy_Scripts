using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CocoDoogy.MiniGame
{
    public class MiniGameManager : Singleton<MiniGameManager>
    {
        [SerializeField] private MiniGameBase[] miniGames;
        [SerializeField] private GameObject backGround;
         public GameObject BackGround => backGround;

        [SerializeField] private TutorialMessage tutorialMessage;
        [SerializeField] private TextMeshProUGUI miniGameExplainText;
        [SerializeField] private GameObject letterBoxd;
        public GameObject LetterBoxd => letterBoxd;
        
        protected override void Awake()
        {
            base.Awake();
            foreach (var miniGame in miniGames)
            {
                miniGame.gameObject.SetActive(false);
            }
        }
        
        private void ExplainText(string tutorialExplainData)
        {
            miniGameExplainText.gameObject.SetActive(true);
            miniGameExplainText.text = tutorialExplainData;
        }
        /// <summary>
        /// MiniGameManger가 갖고있는 모든 미니게임중 테마를 받으면서 호출
        /// </summary>
        public static void OpenRandomGame(System.Action callback)
        {
            Instance.gameObject.SetActive(true);
            Instance.backGround.SetActive(true);
            
            Theme nowTheme = InGameManager.Stage.theme;

            //nowTheme를 갖고 있는 게임을 가져옴//즉, 테마가 없는 게임은 모든 테마를 갖고있도록 설정해야 랜덤게임에 선택될수있음
            MiniGameBase[] possibleGames = Instance.miniGames.Where(x => x.HasWithTheme(nowTheme)).ToArray();
            int randomIdx = Random.Range(0, possibleGames.Length);

            MiniGameBase selectedMiniGame = possibleGames[randomIdx];
            selectedMiniGame.gameObject.SetActive(true);
            //PlayerPrefs로 튜토리얼 설명여부저장
             string key = $"TutorialShown_{selectedMiniGame.MiniGameID}";
             if (PlayerPrefs.GetInt(key, 0) == 0)
             {
              Instance.tutorialMessage.ShowTutorialExplain(selectedMiniGame.tutorialExplainData.description);
             }
             PlayerPrefs.SetInt(key, 1);
             PlayerPrefs.Save();
            Instance.letterBoxd.SetActive(true);
            Instance.ExplainText(selectedMiniGame.tutorialExplainData.description);
            selectedMiniGame.Open(callback);
        }

        /// <summary>
        /// 테스트용 임시 실행 코드
        /// </summary>
         public void OpenRandomGame()
         {
             Instance.gameObject.SetActive(true);
             Instance.backGround.SetActive(true);
        
             Theme nowTheme = Theme.Sand; // TODO: 나중에 맵 데이터에서 호출하게 변경
        
             //nowTheme를 갖고 있는 게임을 가져옴//즉, 테마가 없는 게임은 모든 테마를 갖고있도록 설정해야 랜덤게임에 선택될수있음
             MiniGameBase[] possibleGames = Instance.miniGames.Where(x => x.HasWithTheme(nowTheme)).ToArray();
             int randomIdx = Random.Range(0, possibleGames.Length);
        
             MiniGameBase selectedMiniGame = possibleGames[randomIdx];
             selectedMiniGame.gameObject.SetActive(true);
             //PlayerPrefs로 튜토리얼 설명여부저장
              string key = $"TutorialShown_{selectedMiniGame.MiniGameID}";
              if (PlayerPrefs.GetInt(key, 0) == 0)
              { 
                  Instance.tutorialMessage.ShowTutorialExplain(selectedMiniGame.tutorialExplainData.description);
              }
              PlayerPrefs.SetInt(key, 1);
              PlayerPrefs.Save();
             Instance.ExplainText(selectedMiniGame.tutorialExplainData.description);
             Instance.letterBoxd.SetActive(true);
             
             selectedMiniGame.Open(()=> { });
         }
    }
}
