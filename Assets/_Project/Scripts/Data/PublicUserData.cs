using Firebase.Firestore;

namespace CocoDoogy.Data
{
    [FirestoreData]
    public class PublicUserData
    {
        [FirestoreProperty] public string nickName { get; set; }
        [FirestoreProperty] public int clearedStageCount { get; set; }
    }
}
