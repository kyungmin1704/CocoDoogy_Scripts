using CocoDoogy.Core;
using CocoDoogy.Network;
using Firebase.Functions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Data
{
    public class ServerRequestManager : Singleton<ServerRequestManager>
    {
        private FirebaseFunctions Functions;

        protected override void Awake()
        {
            base.Awake();

            if (Instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            FirebaseManager.Instance.OnFirebaseInitialized += InitializeFunctions;
        }

        private void InitializeFunctions()
        {
            Functions = FirebaseManager.Instance.Functions;
            Debug.Log("Cloud Functions 준비완료 및 SererRequestManager 메서드 이용 가능");
        }

        /// <summary>
        /// 인게임 머니를 변경합니다.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="amount">변경할 값 (양,음수 둘다 가능)</param>
        /// <returns></returns>
        public async Task<bool> ChangeInGameMoneyAsync(int amount)
        {
            if (amount == 0) return false;

            try
            {
                //functions에서 호출 가능한 함수 객체를 가져옵니다.
                var callable = Functions.GetHttpsCallable("getInGameMoney");

                var data = new Dictionary<string, object> { { "amount", amount } };

                //data를 인풋으로 해당 함수 객체를 실행합니다.
                var result = await callable.CallAsync(data);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"getInGameMoney 호출 중 오류 발생: {e.Message}");
                return false;
            }
        }

        public async Task<bool> ChangeCashMoneyAsync(string uid, int amount)
        {
            if (amount == 0) return false;

            try
            {
                //functions에서 호출 가능한 함수 객체를 가져옵니다.
                var callable = Functions.GetHttpsCallable("changeCashMoney");
                
                var data = new Dictionary<string, object>
                {
                    { "uid", uid },
                    { "cashMoney", amount }
                };
                
                //data를 인풋으로 해당 함수 객체를 실행합니다.
                var result = await callable.CallAsync(data);
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"changeCashMoney 호출 중 오류 발생: {e.Message}");
                return false;
            }
        }
        
        //닉네임으로 friend찾고 Uid 찾아서 같이 저장하는 로직도 있어야함
        //애초에 친구 추가할때 Uid도 저장하는 게 맞음?
        public async Task<bool> AddFriendAsync(string uid, string friendUid, string friendNickName)
        {
            try
            {
                var callable = Functions.GetHttpsCallable("addFriend");
                var data = new Dictionary<string, object>
                {
                    { "uid", uid },
                    { "friendUid", friendUid },
                    { "friendNickName", friendNickName }
                };
                var result = await callable.CallAsync(data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"AddFriend 호출 중 오류 발생: {e.Message}");
                return false;
            }
        }

        public async Task<bool> PurchaseItemAsync(string uid, int amount)
        {
            if (amount == 0) return false;
            
            var data = new Dictionary<string, object>
            {
                { "uid", uid },
                { "price", amount }
            };
            
            try
            {
                var callable = Functions.GetHttpsCallable("purchaseItem");
                
                var result = await callable.CallAsync(data);
                
                var dict = result.Data as Dictionary<string, object>;
                
                if (dict != null)
                {
                    bool success = dict.ContainsKey("success") && (bool)dict["success"];
                    string returnedUid = dict.ContainsKey("uid") ? dict["uid"].ToString() : "";
                    int returnedPrice = dict.ContainsKey("price") ? System.Convert.ToInt32(dict["price"]) : 0;
                    string reason = dict.ContainsKey("reason") ? dict["reason"].ToString() : "";

                    Debug.Log($"Success: {success}, UID: {returnedUid}, Price: {returnedPrice}, Reason: {reason}");
                }
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"호출 중 오류 발생: {e.Message}");
                return false;
            }
        }
    }
}
