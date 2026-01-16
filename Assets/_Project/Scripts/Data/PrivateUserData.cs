using Firebase.Firestore;
using System.Collections.Generic;

namespace CocoDoogy.Data
{
    [FirestoreData]
    public class PrivateUserData
    {
        [FirestoreProperty] public int inGameMoney { get; set; }
        [FirestoreProperty] public int cashMoney { get; set; }
        [FirestoreProperty] public int gameTicket { get; set; }
        [FirestoreProperty] public int bonusTicket { get; set; }
        [FirestoreProperty] public long lastTicketTime { get; set; }
        [FirestoreProperty] public Dictionary<string, object> itemDic { get; set; } 
        [FirestoreProperty] public Dictionary<string, object> friendsList { get; set; }
        [FirestoreProperty] public Dictionary<string, object> friendReceivedList { get; set; }
        [FirestoreProperty] public Dictionary<string, object> friendSentList { get; set; }
        [FirestoreProperty] public List<GiftData> giftList {get; set;}
        public PrivateUserData()
        {
            itemDic = new Dictionary<string, object>();
            friendsList = new Dictionary<string, object>();
            friendReceivedList = new Dictionary<string, object>();
            friendSentList = new Dictionary<string, object>();
        }
    }
}