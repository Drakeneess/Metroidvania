using UnityEngine;
using System.Collections.Generic;

public class MapUIController : Menu
{
    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject entryMarkerPrefab;

    [SerializeField] private float scaleFactor = 0.1f;
    public float ScaleFactor => scaleFactor;
    [SerializeField] private float markerSize = 8f;

    public Player player;

    void Start()
    {
        var map = MapController.Instance;
        DrawMap(map);
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        GameMenuController.CurrentMode = GameMode.MapMenu;
        RefreshMap();
        CenterOnPlayerTile(); // ⬅️ Centrar el mapa al cuarto actual
    }

    private void DrawMap(MapController map)
    {
        if (map == null || map.mapTiles.Count == 0) return;

        foreach (Transform child in mapContainer)
            Destroy(child.gameObject); // Solo se hace la primera vez

        MapTileData playerTile = (player != null) ? map.GetTileForPosition(player.transform.position) : null;

        foreach (var tile in map.mapTiles)
        {
            GameObject tileObj = Instantiate(tilePrefab, mapContainer);
            RectTransform rt = tileObj.GetComponent<RectTransform>();
            rt.sizeDelta = tile.worldSize * scaleFactor;
            rt.anchoredPosition = tile.worldPosition * scaleFactor;

            var img = tileObj.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                img.color = (tile == playerTile) ? Color.green : Color.white;

            tile.tileObject = tileObj;
            tileObj.SetActive(tile.discovered); // Solo mostrar si fue descubierto
        }

        foreach (var entry in map.mapEntries)
        {
            GameObject marker = Instantiate(entryMarkerPrefab, mapContainer);
            RectTransform rt = marker.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;
            rt.sizeDelta = Vector2.one * markerSize;
            rt.anchoredPosition = entry.worldPosition * scaleFactor;

            entry.markerObject = marker;
            marker.SetActive(entry.discovered); // Solo mostrar si fue descubierto
        }
    }

    private void RefreshMap()
    {
        var map = MapController.Instance;
        if (map == null || map.mapTiles.Count == 0) return;

        MapTileData playerTile = (player != null) ? map.GetTileForPosition(player.transform.position) : null;

        foreach (var tile in map.mapTiles)
        {
            if (tile.tileObject != null)
            {
                tile.tileObject.SetActive(tile.discovered);

                var img = tile.tileObject.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    img.color = (tile == playerTile) ? Color.green : Color.white;
            }
        }

        foreach (var entry in map.mapEntries)
        {
            if (entry.markerObject != null)
                entry.markerObject.SetActive(entry.discovered);
        }
    }

    public Vector2 GetPlayerTilePosition()
    {
        if (player == null || MapController.Instance == null)
            return Vector2.zero;

        var playerTile = MapController.Instance.GetTileForPosition(player.transform.position);
        if (playerTile == null)
            return Vector2.zero;

        return playerTile.worldPosition * scaleFactor;
    }

    private void CenterOnPlayerTile()
    {
        if (mapContainer == null) return;

        Vector2 playerTilePos = GetPlayerTilePosition();

        // Como el pivot del mapContainer normalmente está en el centro (0.5, 0.5),
        // lo movemos para que el playerTile quede centrado
        mapContainer.anchoredPosition = -playerTilePos;
    }

    void OnDisable()
    {
        GameMenuController.CurrentMode = GameMode.Game;
    }
}
