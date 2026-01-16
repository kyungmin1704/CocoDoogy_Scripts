using CocoDoogy.Audio;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.StageSelect.Page;
using CocoDoogy.Utility.Loading;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageInfoPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI title;
        
        [Header("Stage Information")]
        [SerializeField] private StageInfoPage commonInfoPage;
        [SerializeField] private StageInfoPage detailInfoPage;
        [SerializeField] private StageRankingPage rankingPage;
        
        [Header("Buttons")]
        [SerializeField] private CommonButton pageChangeButton;
        [SerializeField] private CommonButton startButton;
        
        
        [SerializeField] private StageSelectStar stageSelectStar;
        public bool IsOpened { get; private set; } = false;


        private StageData stageData = null;

        void Awake()
        {
            pageChangeButton.onClick.AddListener(OnPageChangeButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        void OnDisable()
        {
            IsOpened = false;
        }
        

        public void Show(StageData data)
        {
            if (!data) return;

            IsOpened = true;
            gameObject.SetActive(true);

            title.text = (stageData = data).stageName;
            
            BgmManager.PrepareStageBgm(data.theme);
            
            commonInfoPage.Show(stageData);
            rankingPage.Show(stageData);
            detailInfoPage.Close();
        }

        public void BrightStar(int count)
        {
            rankingPage.CurrentStageStar = count;
            stageSelectStar.BrightStar(count);
        }
        
        private void OnPageChangeButtonClicked()
        {
            if (commonInfoPage.gameObject.activeSelf)
            {
                commonInfoPage.Close(() => detailInfoPage.Show(stageData));
            }
            else
            {
                detailInfoPage.Close(() =>
                {
                    commonInfoPage.Show(stageData);
                    stageSelectStar.BrightStar(rankingPage.CurrentStageStar);
                });
            }
        }
        
        
        private async void OnStartButtonClicked()
        {
            startButton.interactable = false;
            bool isReady = await FirebaseManager.UseTicketAsync();
            if (isReady)
            {
                InGameManager.Stage = stageData;
                PlayerHandler.IsReplay = false;
                Loading.LoadScene("InGame");
            }
            else
            {
               
                MessageDialog.ShowMessage(
                    "티켓 부족", 
                    "티켓이 부족하여 게임을 진행할 수 없습니다.",
                    DialogMode.Confirm,
                    null);
                startButton.interactable = true;
            }
        }
    }
}
