using CocoDoogy.UI.Popup;
using Firebase.Firestore;
using Firebase.Functions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        /// <summary>
        /// 친구 관련으로 Firebase Functions 기능을 사용하는 메서드.<br/>
        /// 비슷한 내용으로 여러개 만들어지는 것을 방지하고자 Functions의 함수를 이름으로 받아 사용 
        /// </summary>
        /// <param name="functionName">Firebase Functions 이름</param>
        /// <param name="friendsUid">보내는 친구 uid(FindUserByNicknameAsync에서 찾아서 넣음)</param>
        /// <param name="errorMessage">Firebase Functions에 맞는 에러 문구</param>
        /// <returns></returns>
        public static async Task<IDictionary<string, object>> CallFriendFunctionAsync(string functionName,
            string friendsUid, string errorMessage)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                Dictionary<string, object> data = new() { { "friendsUid", friendsUid } };
                HttpsCallableResult result = await Instance.Functions.GetHttpsCallable(functionName).CallAsync(data);

                string json = JsonConvert.SerializeObject(result.Data);
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"{errorMessage}: {e.Message}");
                throw;
            }
            finally
            {
                loading.Hide();
            }
        }


        /// <summary>
        /// 입력받은 닉네임을 기준으로 파이어베이스 내에 입력받은 닉네임과 같은 닉네임이 있으면 해당 닉네임의 UID 를 반환 
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static async Task<string> FindUserByNicknameAsync(string nickname)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                DocumentReference docRef = Instance.Firestore.Collection("nicknames").Document(nickname);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    string uid = snapshot.GetValue<string>("uid");
                    return uid;
                }

                Debug.LogWarning($"'{nickname}' 닉네임을 찾을 수 없습니다.");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"사용자 검색 실패: {e.Message}");
                return null;
            }
            finally
            {
                loading.Hide();
            }
        }


        /// <summary>
        /// 현재 로그인한 유저가 받은 친구 추가 요청을 모두 Dictionary에 담아 반환 
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetFriendRequestsAsync(string request)
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                var userDoc = Instance.Firestore
                    .Collection("users").Document(Instance.Auth.CurrentUser.UserId)
                    .Collection("private").Document("data");

                DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();

                if (!snapshot.Exists || !snapshot.TryGetValue(request, out Dictionary<string, object> dictionary))
                {
                    return new Dictionary<string, string>();
                }

                Dictionary<string, string> result = new();
                foreach (KeyValuePair<string, object> key in dictionary)
                {
                    if (key.Value is Dictionary<string, object> friendData &&
                        friendData.TryGetValue("nickName", out object nickname))
                    {
                        // FirebaseStore에서 nickName 필드를 가져와서 Dictionary에 넣음 -> 친구 리스트에 넣을 닉네임을 가져오는 용도
                        result[key.Key] = nickname.ToString();
                    }
                    else if (key.Value is Dictionary<string, object> giftData &&
                             giftData.TryGetValue("giftList", out object gift))
                    {
                        // FirebaseStore에서 gift 필드를 가져와서 Dictionary에 넣음 -> 선물 리스트에 넣을 선물 목록을 가져오는 용도
                        result[key.Key] = gift.ToString();
                    }
                    else if (key.Value is string value)
                    {
                        // 그외 모든 필드의 내용을 넣음 -> 그외 다른 
                        result[key.Key] = value;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"친구 요청 목록 불러오기 실패: {e.Message}");
                return new Dictionary<string, string>();
            }
            finally
            {
                loading.Hide();
            }
        }

        public static async Task<int> SendGiftToAllFriendsAsync()
        {
            var loading = FirebaseLoading.ShowLoading();
            try
            {
                var result = await Instance.Functions.GetHttpsCallable("giftAllFriendsRequest").CallAsync();

                if (result.Data is long l) return (int)l;
                if (result.Data is int i) return i;
                if (result.Data is string s && int.TryParse(s, out var parsed)) return parsed;

                return 0;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return 0;
            }
            finally
            {
                loading.Hide();
            }
        }
    }
}