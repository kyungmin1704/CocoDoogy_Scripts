using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.Data
{
    [CreateAssetMenu(fileName ="New StageInfo", menuName = "StageInfoData/Data")]
    public class StageInfoData : ScriptableObject
    {
        [Tooltip("이미지")]
        public Sprite infoImage;
        [Tooltip("이름")]
        public string infoName;
        [Tooltip("상세 정보")]
        [TextArea(3, 10)] public string infoDescription;

        private void Reset()
        {
            infoImage = null;
            infoName = string.Empty;
            infoDescription = string.Empty;
        }
    }
}
