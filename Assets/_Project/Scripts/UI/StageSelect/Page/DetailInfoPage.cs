using CocoDoogy.Data;
using CocoDoogy.Tile;
using CocoDoogy.UI.StageSelect.DetailInfo;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect.Page
{
    /// <summary>
    /// 스테이지 상세보기
    /// </summary>
    public class DetailInfoPage : StageInfoPage
    {
        [SerializeField] private DetailGroup tileGroup;
        [SerializeField] private DetailGroup pieceGroup;
        [SerializeField] private DetailGroup weatherGroup;
        
        
        [SerializeField] private GameObject detailInfoContainer;
        
        
        protected override void OnShowPage()
        {
            tileGroup.Clear();
            pieceGroup.Clear();
            weatherGroup.Clear();

            foreach (var data in StageData.tileData)
            {
                tileGroup.AddItem(data);
            }
            foreach (var data in StageData.pieceData)
            {
                pieceGroup.AddItem(data);
            }
            foreach (var data in StageData.weatherData)
            {
                weatherGroup.AddItem(data);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(tileGroup.transform.parent as RectTransform);
            tileGroup.Check();
            pieceGroup.Check();
            weatherGroup.Check();
        }

        private void GetTileInfo()
        {
            
        }
    }
}