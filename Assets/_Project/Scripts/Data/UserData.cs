using CocoDoogy.Network;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Data
{
    [FirestoreData]
    public class UserData
    {
        private string uid;
        
        public PublicUserData PublicUserData { get; set; }
        public PrivateUserData PrivateUserData { get; set; }
        
        //읽기 전용 프로퍼티
        public string NickName => PublicUserData.nickName;
        public int InGameMoney => PrivateUserData.inGameMoney;
        public int CashMoney => PrivateUserData.cashMoney;
        public int GameTicket => PrivateUserData.gameTicket;
        public int BonusTicket => PrivateUserData.bonusTicket;
        public int ClearedStageCount => PublicUserData.clearedStageCount;
        public IReadOnlyDictionary<string, object> ItemDic => PrivateUserData.itemDic;
        public IReadOnlyDictionary<string, object> FriendsList => PrivateUserData.friendsList;
        public IReadOnlyDictionary<string, object> FriendReceivedList => PrivateUserData.friendReceivedList;
        public IReadOnlyDictionary<string, object> FriendSentList => PrivateUserData.friendSentList;
        public UserData() //파이어베이스에서 데이터를 불러올 때 null 오류 방지
        {
            PublicUserData = new PublicUserData();
            PrivateUserData = new PrivateUserData();
        }

        public UserData(string uid, string nickName)
        {
            this.uid = uid;
            PublicUserData = new PublicUserData { nickName = nickName };
            PrivateUserData = new PrivateUserData
            {
                gameTicket = 10,
                // UserData를 생성할 때 아이템 인벤토리에 모든 아이템을 0개로 초기화 후 저장
                itemDic = new Dictionary<string, object>() {
                    { "item001", 0 },
                    { "item002", 0 },
                    { "item003", 0 },
                }
            };
        }

        public void SetUid(string id)
        {
            this.uid = id;
        }
        
        //게임을 "처음" 시작시 서버에 데이터 생성 (전역 메서드)
        public static async Task<UserData> CreateOnServerAsync(string uid, string nickName)
        {
            var userData = new UserData(uid, nickName);
            
            var publicRef = FirebaseManager.Instance.Firestore
                .Collection("users").Document(uid)
                .Collection("public").Document("profile");
            await publicRef.SetAsync(userData.PublicUserData);
            
            var privateRef = FirebaseManager.Instance.Firestore
                .Collection("users").Document(uid)
                .Collection("private").Document("data");
            await privateRef.SetAsync(userData.PrivateUserData);
            
            return userData;
        }
        
        /// <summary>
        /// 로그인시 신규 유저인지 신규유저가 아닌지 확인하는 메서드
        /// </summary>
        /// <param name="userId">로그인한 유저의 UID</param>
        /// <returns>
        /// 로그인 한 유저가 신규 유저다 -> false
        /// 료그인 한 유저가 기존 유저다 -> true
        /// </returns>
        public static async Task<bool> CheckUserProfileExistsAsync(string userId)
        {
            var userRef = FirebaseManager.Instance.Firestore
                .Collection("users").Document(userId)
                .Collection("public").Document("profile");
            var snapshot = await userRef.GetSnapshotAsync();
            return snapshot.Exists;
        }
        
        public static async Task<bool> TrySetNewNickNameAsync(string uid, string nickname)
        {
            try
            {
                await FirebaseManager.Instance.Firestore.RunTransactionAsync(async transaction =>
                {
                    DocumentReference nickRef =  FirebaseManager.Instance.Firestore.Collection("nicknames").Document(nickname);

                    DocumentSnapshot nickSnapshot = await transaction.GetSnapshotAsync(nickRef);

                    if (nickSnapshot.Exists)
                    {

                        throw new Exception("닉네임이 이미 존재하고 있습니다."); 
                    }

                    transaction.Set(nickRef, new { uid = uid });
                });

                return true;
            }
            catch (Exception ex) when (ex.Message.Contains("닉네임이 이미 존재하고 있습니다."))
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"닉네임 선점 트랜잭션 중 오류 발생: {ex.Message}");
                throw; 
            }
        }
    }
}
