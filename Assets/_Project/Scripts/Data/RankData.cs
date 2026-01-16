using Firebase.Firestore;

namespace CocoDoogy.Data
{
    [FirestoreData]
    public class RankData
    {
        [FirestoreProperty] public int rank { get; set; }
        [FirestoreProperty] public string nickname { get; set; }
        [FirestoreProperty] public double clearTime { get; set; }
        [FirestoreProperty] public string remainAP { get; set; }
        [FirestoreProperty] public string replayId { get; set; }
        [FirestoreProperty] public int refillPoints { get; set; }
    }
}