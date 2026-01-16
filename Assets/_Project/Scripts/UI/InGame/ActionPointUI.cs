using CocoDoogy._Project.Scripts.UI.CocoDoogys;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Tile;
using System;
using UnityEngine;
using TMPro;

namespace CocoDoogy.UI.InGame
{
    public class ActionPointUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private int oldActionPoints = int.MaxValue;
        private bool initialized = false;
        
        //이전 ActionPoint에서 현재 ActionPoint를 빼서 그만큼을 추가하는수밖에...
        private int previousAP;
        
        void OnEnable()
        {
            initialized = false;
            oldActionPoints = int.MaxValue;
            InGameManager.OnActionPointChanged += OnActionPointChanged;
        }
        void OnDisable()
        {
            InGameManager.OnActionPointChanged -= OnActionPointChanged;
        }


        private bool Initialize(int point)
        {
            if (!initialized)
            {
                // MaxAP 초기 세팅 한번만 실행
                if (point == HexTileMap.ActionPoint)
                {
                    initialized = true;
                    SetValue(point);
                }
                return true;
            }
            return false;
        }
        
        private void OnActionPointChanged(int point)
        {
            if (Initialize(point)) return;
            //Refill할때도 ActionPoint를 출력하는 문제를 방지
            bool isRefill = (oldActionPoints < HexTileMap.ActionPoint) && (point == HexTileMap.ActionPoint);
            int AddAP = point - previousAP;
            //기존 ActionPoint보다 높아지면 GetItem출력 그게 아니면 값만 변경
            if (!isRefill && point >= oldActionPoints && point > 0&&AddAP > 0)
            {
                GetItemPanel.GetItem(ItemUIType.Movement, AddAP, () => SetValue(point));
            }
            else
            {
                SetValue(point);
            }
        }

        private void SetValue(int point)
        {
            text.SetText($"{point} / {HexTileMap.ActionPoint}");
            oldActionPoints = point;
            previousAP = point;
        }
    }
}