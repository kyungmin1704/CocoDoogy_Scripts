using CocoDoogy.UI.Popup;
using Firebase.Firestore;
using Firebase.Functions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        /// <summary>
        /// 아이템 ID를 기준으로 아이템을 구매하는 메서드 <br/>
        /// 아이템 ID가 이상하면 에러 메세지 출력 <br/>
        /// 아이템 ID가 정상이고 돈이 부족하면 return dict { success: false, reason: "돈부족" } <br/>
        /// 아이템 ID가 정상이고 돈이 부족하지 않으면 return dict{ success: true, ... }
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<IDictionary<string, object>> PurchaseWithCashMoneyAsync(string itemId, int quantity)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                Dictionary<string, object> data = new()
                {
                    { "itemId", itemId }, { "itemQuantity", Convert.ToInt32(quantity) }
                };
                HttpsCallableResult result =
                    await Instance.Functions.GetHttpsCallable("purchaseWithCashMoney").CallAsync(data);
                string json = JsonConvert.SerializeObject(result.Data);
                IDictionary<string, object> dict = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
                return dict;
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase Function 호출 실패: {e.Message}");
                throw;
            }
            finally
            {
                loading.Hide();
            }
        }

        public static async Task<IDictionary<string, object>> TakeGiftRequestAsync(string giftId)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                Dictionary<string, object> data = new() { { "giftId", giftId } };
                HttpsCallableResult result =
                    await Instance.Functions.GetHttpsCallable("takePresentRequest").CallAsync(data);

                string json = JsonConvert.SerializeObject(result.Data);
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"선물 받기 실패: {e.Message}");
                throw;
            }
            finally
            {
                loading.Hide();
            }
        }

        public static async Task<string> TakeAllGiftRequestAsync()
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                HttpsCallableResult result = await Instance.Functions
                    .GetHttpsCallable("takeAllPresentRequest")
                    .CallAsync();

                string json = JsonConvert.SerializeObject(result.Data);
                IDictionary<string, object> dict = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

                return dict["message"].ToString();
            }
            catch (Exception e)
            {
                Debug.LogError($"TakeAllGiftRequestAsync Error: {e}");
                return string.Empty;
            }
            finally
            {
                loading.Hide();
            }
        }

        public static async Task<IDictionary<string, object>> UseItemAsync(string itemId)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                Dictionary<string, object> data = new() { { "itemId", itemId } };
                HttpsCallableResult result = await Instance.Functions.GetHttpsCallable("useItem").CallAsync(data);

                string json = JsonConvert.SerializeObject(result.Data);
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"아이템 사용 실패: {e.Message}");
                throw;
            }
            finally
            {
                loading.Hide();
            }
        }

        public static async Task<List<IDictionary<string, object>>> GetGiftListAsync()
        {
            string userId = Instance.Auth.CurrentUser.UserId;
            DocumentSnapshot userDoc = await Instance.Firestore
                .Collection("users")
                .Document(userId)
                .Collection("private")
                .Document("data")
                .GetSnapshotAsync();

            if (!userDoc.Exists) return new List<IDictionary<string, object>>();

            if (userDoc.TryGetValue("giftList", out List<object> giftListRaw) &&
                giftListRaw != null)
            {
                var giftList = new List<IDictionary<string, object>>();
                foreach (var item in giftListRaw)
                {
                    giftList.Add(item as IDictionary<string, object>);
                }

                return giftList;
            }

            return new List<IDictionary<string, object>>();
        }

        /// <summary>
        /// Firebase Firestore에서 현재 로그인한 유저의 itemDic을 읽어와 반환하는 메서드
        /// </summary>
        public static async Task<IDictionary<string, object>> GetItemListAsync()
        {
            string userId = Instance.Auth.CurrentUser.UserId;
            DocumentSnapshot userDoc = await Instance.Firestore
                .Collection("users")
                .Document(userId)
                .Collection("private")
                .Document("data")
                .GetSnapshotAsync();
            if (!userDoc.Exists) return new Dictionary<string, object>();
            if (userDoc.TryGetValue("itemDic", out Dictionary<string, object> dictionary))
            {
                var itemDic = new Dictionary<string, object>();
                foreach (var item in dictionary)
                {
                    itemDic[item.Key] = item.Value;
                }

                return itemDic;
            }

            return new Dictionary<string, object>();
        }
    }
}