using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI;
using TMPro;
using UnityEngine;

namespace CocoDoogy.Test
{
    public class MapLoaderPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform buttonGroup;
        [SerializeField] private CommonButton buttonPrefab;


        void Awake()
        {
            TextAsset[] mapData = Resources.LoadAll<TextAsset>("MapData");

            foreach (var data in mapData)
            {
                var button = Instantiate(buttonPrefab, buttonGroup);
                var text = button.GetComponentInChildren<TextMeshProUGUI>();
                text.text = data.name;

                button.onClick.AddListener(() =>
                {
                    string json = data.text;
                    InGameManager.DrawMap(json);
                });
            }
        }
    }
}