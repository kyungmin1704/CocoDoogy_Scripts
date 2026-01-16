using Firebase.Firestore;

namespace CocoDoogy.Data
{
    [FirestoreData]
    public class StageInfo
    {
        [FirestoreProperty] public int remainAP { get; set; }
        [FirestoreProperty] public int refillPoints { get; set; }
        [FirestoreProperty] public float clearTime { get; set; }
        [FirestoreProperty] public string theme { get; set; }
        [FirestoreProperty] public string level { get; set; }
        [FirestoreProperty] public string replayId { get; set; }
    }
}