using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }
    [SerializeField] private GameObject roomContainer;
    public GameObject RoomContainer => roomContainer;

    public readonly List<MapTileData> mapTiles = new List<MapTileData>();
    public readonly List<MapEntryData> mapEntries = new List<MapEntryData>();

    void Awake()
    {
        Instance = this;
        BuildMap();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void BuildMap()
    {
        mapTiles.Clear();
        mapEntries.Clear();

        RoomController[] rooms = roomContainer.GetComponentsInChildren<RoomController>();
        HashSet<ChunkController> addedEntries = new HashSet<ChunkController>();

        foreach (var room in rooms)
        {
            Vector2 worldCenter = room.GetWorldCenter2D();
            Vector2 worldSize = room.GetWorldSize2D();

            var tileData = new MapTileData(worldCenter, Vector2.zero, worldSize, room);
            mapTiles.Add(tileData);

            room.SetMapTileData(tileData);

            foreach (var entry in room.entries)
            {
                if (addedEntries.Contains(entry)) continue;
                addedEntries.Add(entry);

                Vector2 entryPos2D = new Vector2(entry.transform.position.x, entry.transform.position.y);
                var entryData = new MapEntryData(entryPos2D, entry);
                mapEntries.Add(entryData);
            }
        }
    }


    public MapTileData GetTileForPosition(Vector3 playerPos)
    {
        foreach (var tile in mapTiles)
        {
            Vector2 halfSize = tile.worldSize * 0.5f;

            // Chequeo de AABB (rectángulo)
            if (playerPos.x >= tile.worldPosition.x - halfSize.x &&
                playerPos.x <= tile.worldPosition.x + halfSize.x &&
                playerPos.y >= tile.worldPosition.y - halfSize.y &&
                playerPos.y <= tile.worldPosition.y + halfSize.y)
            {
                return tile;
            }
        }
        return null;
    }

    public Rect GetMapBounds(float scaleFactor)
    {
        if (mapTiles.Count == 0) return new Rect();

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var tile in mapTiles)
        {
            Vector2 half = tile.worldSize * 0.5f;
            minX = Mathf.Min(minX, tile.worldPosition.x - half.x);
            maxX = Mathf.Max(maxX, tile.worldPosition.x + half.x);
            minY = Mathf.Min(minY, tile.worldPosition.y - half.y);
            maxY = Mathf.Max(maxY, tile.worldPosition.y + half.y);
        }

        // Escalar a la escala de la UI
        minX *= scaleFactor;
        maxX *= scaleFactor;
        minY *= scaleFactor;
        maxY *= scaleFactor;

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }
    public string SectionName => SceneManager.GetActiveScene().name;
}

public class MapTileData
{
    public Vector2 worldPosition;
    public Vector2 normalizedPosition;
    public Vector2 worldSize;
    public bool discovered;
    public RoomController room; // 🔹 Referencia directa
    public GameObject tileObject;

    public MapTileData(Vector2 worldPos, Vector2 normalizedPos, Vector2 worldSize, RoomController room)
    {
        this.worldPosition = worldPos;
        this.normalizedPosition = normalizedPos;
        this.worldSize = worldSize;
        this.room = room;
        this.discovered = false;
    }
}


public class MapEntryData
{
    public Vector2 worldPosition;
    public ChunkController chunk;
    public bool discovered;
    public GameObject markerObject;
    public MapEntryData(Vector2 pos, ChunkController chunk)
    {
        this.worldPosition = pos;
        this.chunk = chunk;
        this.discovered = false;
    }
}
