using CocoDoogy.Core;
using UnityEngine;
using CocoDoogy.UI.StageSelect;
using CocoDoogy.Data;

namespace CocoDoogy.LobbyObject
{
    /// <summary>
    /// Lobby Scene의 각 Theme별 장식의 Active를 ClearData를 기준으로 관리하는 class
    /// </summary>
    public class LobbyThemeObjectManager : MonoBehaviour
    {
        public GameObject[] forestObjects;
        public GameObject[] waterObjects;
        public GameObject[] snowObjects;
        public GameObject[] sandObjects;
        

        void Awake()
        {
            OnLastClearStageChanged(null);
            StageSelectManager.OnLastClearedStageChanged += OnLastClearStageChanged;
        }

        void OnDestroy()
        {
            StageSelectManager.OnLastClearedStageChanged -= OnLastClearStageChanged;
        }


        private void OnLastClearStageChanged(StageInfo data)
        {
            StageData lastClearStage = null;
            if (data != null)
            {
                lastClearStage = DataManager.GetStageData((Theme)(1 << (data.theme.Hex2Int() - 1)), data.level.Hex2Int());
            }

            ChangeActive(forestObjects, Theme.Forest, lastClearStage);
            ChangeActive(waterObjects, Theme.Water, lastClearStage);
            ChangeActive(snowObjects, Theme.Snow, lastClearStage);
            ChangeActive(sandObjects, Theme.Sand, lastClearStage);
        }
        private void ChangeActive(GameObject[] objs, Theme theme, StageData clearData)
        {
            for(int i = 0;i < objs.Length;i++)
            {
                bool isActive =
                    clearData && // 일단 data가 존재해야 함
                    (
                        clearData.theme > theme || // 클리어 Theme가 더 크면 하이패스
                        (
                            clearData.theme >= theme && // 클리어 Theme가 Theme 이상에
                            (
                                clearData.index >= DataManager.GetThemeMaxIndex(clearData.theme) || // 모든 Stage를 클리어했거나
                                clearData.index >= i + 1 // index도 이상이면
                            )
                        )
                    );
                objs[i].SetActive(isActive);
            }
        }
    }
}
