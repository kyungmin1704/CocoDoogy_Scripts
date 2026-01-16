using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect
{
    /// <summary>
    /// Stage 선택 버튼
    /// </summary>
    public class StageSelectButton : MonoBehaviour
    {
        private int Star { get; set; }

        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI stageNumberText;

        [SerializeField] private CommonButton startButton;

        [SerializeField] private GameObject starGroup = null;
        [SerializeField] private GameObject[] clearStars = null;

        [Header("UI Components")] 
        [SerializeField] private Sprite defaultSprite;

        [SerializeField] private Sprite lockedSprite;


        private StageData stageData = null;
        private Action<StageData, int> callback = null;
        private bool isLocked = false;

#if UNITY_EDITOR
        private void Reset()
        {
            stageNumberText = GetComponentInChildren<TextMeshProUGUI>();
            startButton = GetComponentInChildren<CommonButton>();

            List<GameObject> starObjects = new();
            foreach (Transform child in transform.Find("Stars"))
            {
                starObjects.Add(child.gameObject);
            }

            clearStars = starObjects.ToArray();
        }
#endif
        private void OnEnable()
        {
            startButton.onClick.AddListener(OnClickStageSelectButton);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(OnClickStageSelectButton);
        }
        
        
        
        /// <summary>
        /// 스테이지 데이터 입력 및 초기화
        /// </summary>
        /// <param name="data">스테이지 데이터</param>
        /// <param name="starSize">스테이지 사이즈</param>
        /// <param name="actionCallback"></param>
        public void Init(StageData data, int starSize, Action<StageData, int> actionCallback)
        {
            stageData = data;
            callback = actionCallback;
            
            Star = starSize;

            foreach (GameObject star in clearStars)
            {
                star.SetActive(starSize-- > 0);
            }
        
            stageNumberText.text = $"{data.stageName}";
        
            var last = StageSelectManager.LastClearedStage;
            StageData lastClearStage = null;
            
            if (last != null)
            {
                lastClearStage = DataManager.GetStageData((Theme)(1 << (last.theme.Hex2Int() - 1)), last.level.Hex2Int());
            }
            
            isLocked = true;
            foreach (var check in IsStageOpen)
            {
                if (check(data, lastClearStage))
                {
                    isLocked = false;
                    break;
                }
            }
            
            ApplyLockedState();
        }
        
        // TODO: 스테이지 테스트 할때 위에 Init 비활성화 하고 이거 활성화 해서 사용하면 됨 
        
        // public void Init(StageData data, int starSize, Action<StageData, int> actionCallback)
        // {
        //     // if (StageSelectManager.LastClearedStage.theme.Hex2Int() < (int)data.theme ||
        //     //     StageSelectManager.LastClearedStage.level.Hex2Int() < data.index - 1)
        //     // {
        //     //     startButton.interactable = false;
        //     //     startButton.GetComponentInChildren<Image>().sprite = lockedSprite;
        //     //     starGroup.gameObject.SetActive(false);
        //     //     stageNumberText.text = $"{data.stageName}";
        //     //     return;
        //     // }
        //
        //     startButton.interactable = true;
        //     startButton.GetComponentInChildren<Image>().sprite = defaultSprite;
        //     starGroup.gameObject.SetActive(true);
        //
        //     stageData = data;
        //     callback = actionCallback;
        //
        //     foreach (GameObject star in clearStars)
        //     {
        //         star.SetActive(starSize-- > 0);
        //     }
        //
        //     stageNumberText.text = $"{data.stageName}";
        //     startButton.onClick.AddListener(OnButtonClicked);
        // }
        //
        private void ApplyLockedState()
        {
            Image img = startButton.image;
            if (img)
                img.sprite = isLocked ? lockedSprite : defaultSprite;

            if (starGroup)
                starGroup.gameObject.SetActive(!isLocked);

            if (stageNumberText && stageData && !isLocked) stageNumberText.text = $"{stageData.stageName}";
            else stageNumberText.text = "";
        }

        private void OnClickStageSelectButton()
        {
            if (isLocked)
                OnLockedButtonClicked();
            else
                OnButtonClicked();
        }
        
        private void OnButtonClicked()
        {
            callback?.Invoke(stageData, Star);
        }

        private void OnLockedButtonClicked()
        {
            MessageDialog.ShowMessage($"{stageData.stageName}", "해당 스테이지에 입장할 수 없습니다.", DialogMode.Confirm, null);
        }

        #region < 스테이지 활성화 책임 연쇄 패턴>
        private static readonly Func<StageData, StageData, bool>[] IsStageOpen =
        {
            IsFirstStage, IsClearedTheme, IsClearedStageOrNextIndex, IsLastClearedNextStage
        };

        /// <summary>
        /// 첫 스테이지인지
        /// </summary>
        private static bool IsFirstStage(StageData nowStage, StageData clearData) =>
            nowStage.theme <= Theme.Forest && nowStage.index <= 1;
        /// <summary>
        /// 첫 스테이지인지
        /// </summary>
        private static bool IsClearedTheme(StageData nowStage, StageData clearData) =>
            clearData && nowStage.theme < clearData.theme;
        /// <summary>
        /// 이미 클리어한 스테이지거나,<br/>
        /// 같은 테마 내 다음 스테이지인지
        /// </summary>
        private static bool IsClearedStageOrNextIndex(StageData nowStage, StageData clearData) =>
            clearData && nowStage.theme <= clearData.theme && nowStage.index <= clearData.index + 1;
        /// <summary>
        /// 마지막으로 클리어한 스테이지의 다음 Theme 중 첫 스테이지인지
        /// </summary>
        private static bool IsLastClearedNextStage(StageData nowStage, StageData clearData)
        {
            if(!clearData) return false;
            if(nowStage.theme.ToIndex() > clearData.theme.ToIndex() + 1) return false;

            int lastIndex = DataManager.GetThemeMaxIndex(clearData.theme);
            return clearData && nowStage.index <= 1 && clearData.index >= lastIndex;
        }
        #endregion
    }
}