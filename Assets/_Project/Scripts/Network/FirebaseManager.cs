using CocoDoogy.Core;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Functions;
using Firebase.Storage;
using System;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager : Singleton<FirebaseManager>
    {
        public FirebaseAuth Auth { get; private set; }
        public FirebaseUser User { get; set; }
        public FirebaseDatabase DB { get; set; }
        public FirebaseStorage Storage { get; private set; }
        public FirebaseFirestore Firestore { get; private set; }
        public FirebaseFunctions Functions { get; private set; }

        public bool IsFirebaseReady { get; private set; } = false;

        /// <summary>
        /// 파이어베이스 초기화가 완료 후에 작동하는 이벤트
        /// </summary>
        public event Action OnFirebaseInitialized;

        public event Action OnDisconnectInternet;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            InitFirebase();
        }

        /// <summary>
        /// 각 파이어베이스 초기화시키는 메서드
        /// </summary>
        private void InitFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    Debug.Log("Firebase dependency check available");

                    // 각 파이어베이스 초기화
                    Auth = FirebaseAuth.DefaultInstance;
                    DB = FirebaseDatabase.DefaultInstance;
                    Storage = FirebaseStorage.DefaultInstance;
                    Firestore = FirebaseFirestore.DefaultInstance;
                    Functions = FirebaseFunctions.GetInstance("asia-northeast3");
                    
                    IsFirebaseReady = true;
                    OnFirebaseInitialized?.Invoke();
                }
                else
                {
                    IsFirebaseReady = false;
                    Debug.LogError($"Could not initialize FirebaseAuth {task.Result}");
                }
            });
        }
        
        /// <summary>
        /// LoginUI 에서 OnFirebaseInitialized에 이벤트를 넣어야하는데 이벤트를 넣기 전, <br/>
        /// OnFirebaseInitialized?.Invoke() 부분이 실행될 수 있기 때문에 파악 후 이벤트 실행 or 넣기 
        /// </summary>
        /// <param name="callback"></param>
        public static void SubscribeOnFirebaseInitialized(Action callback)
        {
            if (Instance.IsFirebaseReady) callback?.Invoke();
            else Instance.OnFirebaseInitialized += callback;
        }
    }
}