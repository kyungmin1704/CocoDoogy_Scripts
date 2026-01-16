using UnityEngine;

namespace CocoDoogy.MiniGame
{
    [CreateAssetMenu(fileName = "TutorialExplainData", menuName = "Scriptable Objects/MiniGame/ExplainData")]
    public class TutorialExplainData : ScriptableObject
    {
        [TextArea(3,3)]
        public string description;
    }
}