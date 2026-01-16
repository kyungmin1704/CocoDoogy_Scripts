using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.MapEditor
{
    public class HexGridRenderer : MonoBehaviour
    {
        [Header("Guide Size")]
        [SerializeField] private int hexWidth;
        [SerializeField] private int hexHeight;
        [SerializeField] private float hexSize;
        
        [Header("Guide Style")]
        [SerializeField] private Material hexMaterial;
        [SerializeField] private Color hexMaterialColor;
        
        
        private List<GameObject> Hexes { get; } = new();
        
        
        private void Start()
        {
            DrawGrid();
        }
        
        private void DrawGrid()
        {
            for (int y = -hexHeight; y < hexHeight; y++)
            {
                for (int x = -hexWidth; x < hexWidth; x++)
                {
                    Vector3 pos = new Vector2Int(x, y).ToWorldPos();
                    DrawHex(pos);
                }
            }
        }

        private void DrawHex(Vector3 center)
        {
            GameObject hexObj = new("HexLine", typeof(LineRenderer));
            hexObj.layer = LayerMask.NameToLayer("Guide");
            hexObj.transform.SetParent(transform);
            var line = hexObj.GetComponent<LineRenderer>();
            
            line.positionCount = 6;
            line.loop = true;
            line.widthMultiplier = 0.05f;
            line.material = hexMaterial;
            // TODO : 나중에 색상 변경 기능도 추가할 거면 추가
            line.material.color = hexMaterialColor;
            line.useWorldSpace = false;
            
            float sizeRate = hexSize / Constants.SQRT_3;

            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * (60 * i + 30);
                line.SetPosition(i, new Vector3( sizeRate * Mathf.Cos(angle), 0, sizeRate * Mathf.Sin(angle)));
            }

            hexObj.transform.position = center;
            Hexes.Add(hexObj);
        }
    }
}
