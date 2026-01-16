using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Phase;
using CocoDoogy.GameFlow.InGame.Phase.Passage;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Test;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class InGameManager : Singleton<InGameManager>
    {
        public static event Action<int> OnActionPointChanged = null;
        public static event Action<int> OnRefillCountChanged = null;
        public static event Action<Sprite, bool, Action> OnInteractChanged = null;
        public static event Action<StageData> OnMapDrawn = null;

        /// <summary>
        /// 현재 인게임이 정상적인(= 플레이 가능) 상태인지 체크
        /// </summary>
        public static bool IsValid
        {
            get
            {
                if (!Instance) return false;
                if (!PlayerHandler.Instance) return false;


                return true;
            }
        }

        /// <summary>
        /// 마지막으로 소모된 ActionPoints
        /// </summary>
        public static int LastConsumeActionPoints { get; private set; } = 0;
        /// <summary>
        /// 초기화 후, 소모된 ActionPoints
        /// </summary>
        public static int ConsumedActionPoints
        {
            get;
            private set;
        }

        /// <summary>
        /// 남은 RefillPoints
        /// </summary>
        public static int RefillPoints
        {
            get => Instance?.refillPoints ?? 0;
            private set
            {
                if (!IsValid) return;

                Instance.refillPoints = value;
                OnRefillCountChanged?.Invoke(Instance.refillPoints);
            }
        }

        public static int UseActionPoints
        {
            get => _UseActionPoints;
            set => print(_UseActionPoints = value);
        }
        private static int _UseActionPoints = 0;
        public static int UseRefillCounts = 0;
        /// <summary>
        /// Refill전까지 남은 ActionPoints
        /// </summary>
        public static int ActionPoints
        {
            get => Instance?.actionPoints ?? 0;
            private set
            {
                if (!IsValid) return;

                Instance.actionPoints = value;
                OnActionPointChanged?.Invoke(Instance.actionPoints);
            }
        }

        public static List<PassageBase> Passages { get; } = new();
        /// <summary>
        /// InGame에서 사용되는 StageData
        /// </summary>
        public static StageData Stage
        {
            get => stageData;
            set
            {
                stageData = value;
                MapData = stageData ? stageData.mapData.text : null;
            }
        }
        /// <summary>
        /// InGame에서 사용되는 MapData
        /// </summary>
        public static string MapData { get; private set; } = null;

        public static Stopwatch Timer { get; } = new();
        /// <summary>
        /// 플레이 하고 있는 맵의 최대 행동력
        /// </summary>
        public static int CurrentMapMaxActionPoints { get; private set; } = 0;


        private static StageData stageData = null;


        private int refillPoints = 0;
        private int actionPoints = 0;

        private readonly IPhase[] turnPhases =
        {
            new ClearCheckPhase(),
            new PreGravityButtonPhase(),
            new TornadoCheckPhase(),
            new SlideCheckPhase(),
            new PassageCheckPhase(),
            new CrateMovePhase(),
            new CrateProcessPhase(),
            new RegenCheckPhase(),
            new ActionPointCheckPhase(),
            new InteractableCheckPhase(),
            new LockCheckPhase(),
        };


        void Start()
        {
            DrawMap(MapData);
        }


        /// <summary>
        /// 맵 그리기 및 캐릭터 배치
        /// </summary>
        /// <param name="mapJson"></param>
        public static void DrawMap(string mapJson)
        {
            // TODO:
            //  1. DrawMap을 다른 곳으로 옮기는 게 좋아 보임
            //  2. Map 그리는 클래스와 게임 진행 클리스를 분리해야 함
            if (!IsValid) return;

            Instance.Clear();
            CommandManager.Clear();

            if (mapJson is null)
            {
                // MapData가 없이 InGame에 들어가면, Test데이터 생성
                mapJson = Resources.Load<TextAsset>($"MapData/Test").text;
            }
            MapSaveLoader.Apply(mapJson);

            RefillPoints = HexTileMap.RefillPoint;
            UseRefillCounts = 0;
            UseActionPoints = 0;
            ActionPoints = HexTileMap.ActionPoint;
            CurrentMapMaxActionPoints = HexTileMap.ActionPoint;
            CommandManager.Deploy(HexTileMap.StartPos, HexDirection.NorthEast);
            CommandManager.Weather(HexTileMap.DefaultWeather);

            foreach (var weather in HexTileMap.Weathers)
            {
                Passages.Add(new WeatherPassage(weather.Key, weather.Value));
            }

            foreach (var gimmick in HexTileMap.Gimmicks.Values)
            {
                foreach (var trigger in gimmick.Triggers)
                {
                    GimmickExecutor.ExecuteFromTrigger(trigger.GridPos);
                }
            }

            OnMapDrawn?.Invoke(Stage);
            Timer.Start();

            ProcessPhase();
        }

        private void Clear()
        {
            TileOutlineDrawer.Clear();
            WeatherManager.NowWeather = WeatherType.None;
            Passages.Clear();
            LastConsumeActionPoints = 0;
            ConsumedActionPoints = 0;
            RefillPoints = 0;
            ActionPoints = 0;
            Timer.Stop();

            ChangeInteract(null, false, null);

            foreach (IPhase phase in turnPhases)
            {
                if (phase is IClearable clearable)
                {
                    clearable.OnClear();
                }
            }
        }

        /// <summary>
        /// 초기화 동작
        /// </summary>
        public static void RefillActionPoint()
        {
            ConsumedActionPoints = 0;
            RefillPoints--;
            UseRefillCounts++;
            ActionPoints = HexTileMap.ActionPoint;
        }
        /// <summary>
        /// 역초기화 동작
        /// </summary>
        public static void ClearActionPoint()
        {
            RefillPoints++;
            UseRefillCounts--;
            ActionPoints = 0;
        }

        public static void RegenActionPoint(int regen, bool containConsume = true, bool notify = true)
        {
            if (containConsume)
            {
                ConsumedActionPoints -= regen;
            }
            if(notify)
            {
                ActionPoints += regen;
            }
            else
            {
                Instance.actionPoints += regen;
            }
        }
        public static void ConsumeActionPoint(int consume, bool containConsume = true, bool notify = true)
        {
            if (containConsume)
            {
                LastConsumeActionPoints = consume;
                ConsumedActionPoints += consume;
            }
            if(notify)
            {
                ActionPoints -= consume;
            }
            else
            {
                Instance.actionPoints -= consume;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ProcessPhase()
        {
            if (!IsValid) return;

            foreach (var phase in Instance.turnPhases)
            {
                if (!phase.OnPhase()) break;
            }
            // TODO: 추후 삭제 필요
            TileOutlineDrawer.Draw();
        }

        public static void ChangeInteract(Sprite icon, bool interactable, Action callback)
        {
            OnInteractChanged?.Invoke(icon, interactable, callback);
        }
    }
}