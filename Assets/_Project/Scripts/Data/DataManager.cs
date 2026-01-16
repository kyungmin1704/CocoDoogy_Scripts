using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Network;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CocoDoogy.Data
{
    public partial class DataManager : Singleton<DataManager>
    { 
        [SerializeField] private ItemData[] itemData;
        [SerializeField] private ItemData[] cashData;
        [SerializeField] private ItemData[] stampData;
        
        public UserData UserData { get; private set; }
        /// <summary>
        /// itemData에 있는 정보를 다른 클래스에서 사용하기 위한 변수 <br/>
        /// 지금은 ItemToggleHandler에서 아이템 버튼에 아이템 정보를 찾아넣기 위해 사용 중.
        /// </summary>
        public IReadOnlyList<ItemData> ItemData => itemData;
        
        /// <summary>
        /// 현재 로그인한 계정의 아이템 보유 수량을 저장하기 위한 딕셔너리 
        /// </summary>
        public readonly Dictionary<ItemData, int> CurrentItem = new();
        
        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화가 생기면 발생하는 이벤트. public Doc 하위 필드 변경시 발생
        /// </summary>
        public event Action OnPublicUserDataLoaded;
        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화가 생기면 발생하는 이벤트. private Doc 하위 필드 변경시 발생
        /// </summary>
        public event Action OnPrivateUserDataLoaded;

        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화를 감지하는 리스너. public Doc 하위 필드 변경시 발생
        /// </summary>
        private ListenerRegistration publicListener;
        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화를 감지하는 리스너. private Doc 하위 필드 변경시 발생
        /// </summary>
        private ListenerRegistration privateListener;
        
        /// <summary>
        /// Firebase의 변화를 알리는 Listener가 중복으로 생성되는것을 방지하기 위한 변수
        /// </summary>
        private PrivateUserData lastPrivateData;
        
        private static readonly Dictionary<ItemEffect, ItemData> ReplayItem = new();
        
#if UNITY_EDITOR
        void Reset()
        {
            string[] guids = AssetDatabase.FindAssets("t:HexTileData");
            tileData = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<HexTileData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
            guids = AssetDatabase.FindAssets("t:PieceData");
            pieceData = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<PieceData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
            guids = AssetDatabase.FindAssets("t:WeatherData");
            weatherData = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<WeatherData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
            guids = AssetDatabase.FindAssets("t:StageData");
            stageData = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<StageData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                InitTileData();
                InitReplayItem(itemData);
                DontDestroyOnLoad(gameObject);
            }
        }
        
        //실시간 리스너 구독
        public void StartListeningForUserData(string userId)
        {
            StopListening();
            publicListener = CreateListener(userId, "public", "profile", true);
            privateListener = CreateListener(userId, "private", "data", false);
        }

        private ListenerRegistration CreateListener(string userId, string collection, string document, bool isPublic)
        {
            var docRef = FirebaseManager.Instance.Firestore
                .Collection("users").Document(userId)
                .Collection(collection).Document(document);
            
            var listener = docRef.Listen(snapshot =>
            {
                try
                {
                    if (!snapshot.Exists || snapshot.Metadata.HasPendingWrites) return;

                    if (UserData == null)
                    {
                        UserData = new UserData();
                        UserData.SetUid(userId);
                    }

                    if (isPublic)
                    {
                        var newPublicData = snapshot.ConvertTo<PublicUserData>();
                        if (UserData.PublicUserData != null && UserData.PublicUserData.Equals(newPublicData)) return;

                        UserData.PublicUserData = newPublicData;
                        Debug.Log("PublicUserData 업데이트됨");
                        OnPublicUserDataLoaded?.Invoke();
                    }
                    else
                    {
                        var newPrivateData = snapshot.ConvertTo<PrivateUserData>();
                        
                        if (lastPrivateData != null && lastPrivateData.Equals(newPrivateData)) return;

                        lastPrivateData = newPrivateData;
                        UserData.PrivateUserData = newPrivateData;
                        Debug.Log("PrivateUserData 업데이트됨");
                        OnPrivateUserDataLoaded?.Invoke();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"실시간 데이터 동기화 중 오류 발생: {e.Message}");
                }
            });
            
            return listener;
        }

        private void StopListening()
        {
            publicListener?.Stop();
            publicListener = null;
            privateListener?.Stop();
            privateListener = null;
        }
    
        /// <summary>
        /// Intro -> Lobby 씬 전환 시 이벤트 구독 전에 이벤트가 실행되서 이벤트를 실행시키기 위한 메서드 
        /// </summary>
        public void InvokePrivateUserData()
        {
            OnPrivateUserDataLoaded?.Invoke();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopListening();
        }
        
        private void InitReplayItem(ItemData[] arr)
        {
            foreach (var item in arr)
            {
                if (item == null) continue;

                if (!ReplayItem.TryAdd(item.effect, item))
                {
                    Debug.LogWarning($"중복 itemId 발견: {item.itemId}");
                }
            }
        }

        public static ItemData GetReplayItem(ItemEffect effect)
        {
            return ReplayItem.GetValueOrDefault(effect);
        }
    }
}