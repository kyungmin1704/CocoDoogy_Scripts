using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.UI.Highlight;
using UnityEngine;

namespace CocoDoogy.Tutorial
{
    public class TutorialController: MonoBehaviour
    {
        [Header("For UI Trace")]
        [SerializeField] private RectTransform actionPointsUI;
        [SerializeField] private RectTransform refillButton;
        [SerializeField] private RectTransform weatherIcon;


        private int index = 0;


        void Awake()
        {
            InGameManager.OnMapDrawn += OnTutorialCheck;
            TutorialUI.OnTouched += OnTouched;
        }
        void OnDestroy()
        {
            InGameManager.OnMapDrawn -= OnTutorialCheck;
            TutorialUI.OnTouched -= OnTouched;
        }


        private void OnTouched()
        {
            index++;
            OnTutorialCheck(InGameManager.Stage);
        }
        private void OnTutorialCheck(StageData stage)
        {
            if(!stage) return;

            if(IsEqual(stage, Theme.Forest, 1))
            {
                Tutorial1_1(index);
            }
            else if(IsEqual(stage, Theme.Forest, 3))
            {
                Tutorial1_3(index);
            }
            else if(IsEqual(stage, Theme.Forest, 4))
            {
                Tutorial1_4(index);
            }
            else if(IsEqual(stage, Theme.Water, 15))
            {
                Tutorial2_15(index);
            }
            else
            {
                TutorialLocker.CameraLock = false;
                TutorialLocker.WhiteListPoses.Clear();
            }
        }
        private bool IsEqual(StageData stage, Theme theme, int index)
        {
            return stage.theme == theme && stage.index == index;
        }

        private void Tutorial1_1(int index = 0)
        {
            switch(index)
            {
                case 0:
                    TutorialLocker.CameraLock = true;
                    TutorialUI.Show("주인이 어디갔지?");
                    TutorialUI.OnBackground();
                    return;
                case 1:
                    targetGridPos = new Vector2Int(-1, 0);
                    targetEventType = PlayerEventType.Move;

                    PlayerHandler.OnEvent += OnPlayerActioned;
                    TutorialLocker.WhiteListPoses.Add(new Vector2Int(-1, 0));
                    Highlighter.FocusTile(new Vector2Int(-1, 0));
                    TutorialUI.Show("앞으로 한 칸 이동해보자");
                    TutorialUI.OffRaycast();
                    return;
                case 2:
                    TutorialLocker.WhiteListPoses.Clear();
                    Highlighter.FocusUI(actionPointsUI.position);
                    TutorialUI.Show("한 칸 이동하니까\n행동력이 하나 소모되네?");
                    TutorialUI.OnRaycast();
                    return;
                case 3:
                    targetGridPos = new Vector2Int(0, 0);
                    TutorialLocker.WhiteListPoses.Add(new Vector2Int(0, 0));
                    Highlighter.FocusTile(Vector2Int.zero);
                    TutorialUI.Show("조심해서 집으로 돌아가보자!");
                    TutorialUI.OffRaycast();
                    return;
            }
            PlayerHandler.OnEvent -= OnPlayerActioned;
            TutorialLocker.WhiteListPoses.Clear();
            TutorialLocker.CameraLock = false;
            Highlighter.Invisible();
            TutorialUI.Close();
        }
        private void Tutorial1_3(int index = 0)
        {
            switch(index)
            {
                case 0:
                    TutorialLocker.CameraLock = true;
                    TutorialUI.Show("헤에~?");
                    TutorialUI.OnRaycast(); 
                    return;
                case 1:
                    Highlighter.FocusTile(new Vector2Int(0, -1));
                    TutorialUI.Show("저기 먹을 게 있어!");
                    return;
                case 2:
                    TutorialUI.Show("한 번 가볼까?");
                    return;
                case 3:
                    targetGridPos = new Vector2Int(-2, -1);
                    targetEventType = PlayerEventType.Move;
                    TutorialLocker.WhiteListPoses.Add(new Vector2Int(-2, -1));
                    
                    PlayerHandler.OnEvent += OnPlayerActioned;
                    Highlighter.FocusTile(new Vector2Int(-2, -1));
                    TutorialUI.OffRaycast();
                    return;
                case 4:
                    targetGridPos = new Vector2Int(-1, -1);
                    targetEventType = PlayerEventType.Move;
                    TutorialLocker.WhiteListPoses.Clear();
                    TutorialLocker.WhiteListPoses.Add(new Vector2Int(-1, -1));
                    
                    Highlighter.FocusTile(new Vector2Int(-1, -1));
                    return;
                case 5:
                    targetGridPos = new Vector2Int(0, -1);
                    targetEventType = PlayerEventType.Move;
                    TutorialLocker.WhiteListPoses.Clear();
                    TutorialLocker.WhiteListPoses.Add(new Vector2Int(0, -1));
                    
                    Highlighter.FocusTile(new Vector2Int(0, -1));
                    return;
                case 6:
                    TutorialLocker.WhiteListPoses.Clear();

                    PlayerHandler.OnEvent -= OnPlayerActioned;
                    Highlighter.FocusUI(actionPointsUI.position);
                    TutorialUI.Show("맛있는 걸 먹으니 행동력이 오르네?");
                    TutorialUI.OnRaycast();
                    return;
                case 7:
                    Highlighter.Invisible();
                    TutorialUI.Show("이제 집으로 돌아가보자!");
                    return;
            }
            TutorialLocker.CameraLock = false;
            Highlighter.Invisible();
            TutorialUI.Close();
        }
        private void Tutorial1_4(int index = 0)
        {
            switch(index)
            {
                case 0:
                    TutorialLocker.CameraLock = true;
                    TutorialUI.Show("헥.. 헥..");
                    TutorialUI.OnRaycast();
                    return;
                case 1:
                    Highlighter.FocusTile(new Vector2Int(2, 0));
                    TutorialUI.Show("집까지는 너무 먼 것 같아...");
                    return;
                case 2:
                    PlayerHandler.OnEvent += OnPlayerActioned;
                    targetGridPos = new Vector2Int(1, -2);
                    targetEventType = PlayerEventType.Move;
                    Highlighter.FocusTile(new Vector2Int(1, -2));
                    TutorialUI.Show("일단 맛있는 걸 먹고 조금만 더 놀아야지");
                    return;
                case 4:
                    TutorialLocker.CameraLock = true;
                    targetEventType = PlayerEventType.Refill;

                    Highlighter.FocusUI(refillButton.position);
                    TutorialLocker.WhiteListPoses.Add(new Vector2Int(-10, -10));
                    TutorialUI.Show("이제 다시 돌아가면 집으로 갈 수 있을 거 같아!");
                    return;
                case 5:
                    TutorialUI.OffRaycast();
                    return;
                case 6:
                    PlayerHandler.OnEvent -= OnPlayerActioned;
                    break;
            }
            TutorialLocker.CameraLock = false;
            TutorialLocker.WhiteListPoses.Clear();
            Highlighter.Invisible();
            TutorialUI.Close();
        }
        private void Tutorial2_15(int index = 0)
        {
            switch(index)
            {
                case 0:
                    TutorialLocker.CameraLock = true;
                    TutorialUI.Show("날씨가 너무 흐려");
                    TutorialUI.OnRaycast();
                    return;
                case 1:
                    TutorialUI.Show("금방이라도 비가 올 것 같아!");
                    return;
                case 2:
                    targetWeather = WeatherType.Rain;
                    WeatherManager.OnWeatherChanged += OnWeatherChagned;
                    TutorialUI.Close();
                    return;
                case 3:
                    print("박정희");
                    WeatherManager.OnWeatherChanged -= OnWeatherChagned;
                    TutorialUI.Show("비가 오니까 흙이 진흙이 됐어!");
                    Highlighter.FocusUI(weatherIcon.position);
                    return;
                case 4:
                    Highlighter.FocusTile(new Vector2Int(-2, 2));
                    TutorialUI.Show("조심해서 마저 가보자!");
                    return;
            }
            TutorialLocker.CameraLock = false;
            Highlighter.Invisible();
            TutorialUI.Close();
        }

        
        private Vector2Int targetGridPos = Vector2Int.zero;
        private PlayerEventType targetEventType = PlayerEventType.None;
        private void OnPlayerActioned(Vector2Int gridPos, PlayerEventType eventType)
        {
            if ( targetEventType != eventType ) return;
            if ( targetGridPos != gridPos && targetEventType == PlayerEventType.Move ) return;

            OnTouched();
        }

        private WeatherType targetWeather = WeatherType.None;
        private void OnWeatherChagned(WeatherType weather)
        {
            print($"야스가 좋은 7세:{weather}");
            if(weather != targetWeather) return;

            OnTouched();
        }
    }
}