using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Animated Light Tile")]
public class AnimatedLightTile : AnimatedTile
{
    public GameObject lightPrefab;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (Application.isPlaying && lightPrefab != null)
        {
            Tilemap map = tilemap.GetComponent<Tilemap>();

            Vector3 worldPos = map.CellToWorld(position) + new Vector3(0.5f, 0.5f, 0);

            GameObject lightObj = GameObject.Instantiate(lightPrefab, worldPos, Quaternion.identity);
            lightObj.name = $"Light_{position.x}_{position.y}";
        }

        return base.StartUp(position, tilemap, go);
    }
}
