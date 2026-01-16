using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.Replay;
using CocoDoogy.Network;
using CocoDoogy.UI.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect.Item
{
    public class RankItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI resetCountText;
        [SerializeField] private TextMeshProUGUI remainAPText;
        [SerializeField] private TextMeshProUGUI clearTimeText;

        [SerializeField] private Button replayButton;

        private string replayId;
        private double replayTime;
        private StageData stageData;
        /// <summary>
        /// 스테이지 선택창에서 스테이지를 선택하면 해당 스테이지의 랭킹을 띄우는 아이템의 초기화
        /// </summary>
        public void Init(string rank, string nickname, string resetCount, string remainAP, float clearTime, string replay, int starCount, StageData stage)
        {
            rankText.text = rank;
            nicknameText.text = nickname;
            resetCountText.text = resetCount;
            remainAPText.text = remainAP;
            OnTimeChanged(clearTime);

            ReplayUIManager.consumeAP = remainAP;
            ReplayUIManager.refillCount = resetCount;
            
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnClickReplayStart);
            
            replayId = replay;
            stageData = stage;
            replayButton.interactable = starCount == 3;
        }

        public void Clear()
        {
            rankText.text = "순위외";
            nicknameText.text = DataManager.Instance.UserData.NickName;
            replayButton.interactable = false;
            resetCountText.text = "0";
            remainAPText.text = "0";
            OnTimeChanged(0);
        }
        
        /// <summary>
        /// 버튼을 눌렀을 때 리플레이를 시작하는 메서드
        /// </summary>
        private async void OnClickReplayStart()
        {
            ReplayHandler.ReplayData = await FirebaseManager.GetReplayData(replayId);
            PlayerHandler.IsReplay = true;
            ReplayUIManager.timer = replayTime;
            InGameManager.Stage = stageData;
            SceneManager.LoadScene("Replay");
        }
        private void OnTimeChanged(float time)
        {
            replayTime = time;
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            clearTimeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}