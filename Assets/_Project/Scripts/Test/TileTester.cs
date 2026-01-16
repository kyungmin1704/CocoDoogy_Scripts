using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.Test
{
    public class TileTester: MonoBehaviour
    {
        [SerializeField] private int x;
        [SerializeField] private int y;


        void Start()
        {
            x = 5;
            y = 5;
            
            foreach (Transform child in gameObject.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Tile.HexTile hexTile = Tile.HexTile.Create(null, new Vector2Int(i, j), transform);
                }
            }
        }
    }
}