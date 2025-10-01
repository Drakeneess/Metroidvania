using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class RoomController : MonoBehaviour
{
    [Range(0f, 5f)]
    public float detectionMargin = 0.5f;

    public BoxCollider roomBounds;
    public List<ChunkController> entries = new List<ChunkController>();

    [SerializeField, HideInInspector] private string roomHash;
    public string RoomHash => roomHash;

    [SerializeField] private bool roomDiscovered = false;
    public bool RoomDiscovered => roomDiscovered;

    private MapTileData mapTileData;

    void Awake()
    {
        if (roomBounds == null)
            roomBounds = GetComponent<BoxCollider>();

        roomBounds.isTrigger = true;
        DetectEntriesByBounds();

        GenerateRoomHash();
    }

    private void Start()
    {
        if (SaveDataController.Instance.saveData.roomVisited.Contains(roomHash))
        {
            StartCoroutine(Initialize());
        }
        
    }
    private IEnumerator Initialize()
    {
        yield return null;
        SetRoomDiscovered();
    }
    private void DetectEntriesByBounds()
    {
        entries.Clear();
        ChunkController[] allChunks = FindObjectsOfType<ChunkController>();

        // 🔹 Crear bounds expandido
        Bounds expandedBounds = roomBounds.bounds;
        expandedBounds.Expand(detectionMargin * 2f);


        foreach (var chunk in allChunks)
        {
            Vector3 pos = chunk.transform.position;

            // ✅ Detecta con margen
            if (expandedBounds.Contains(pos))
            {
                entries.Add(chunk);
                chunk.SetRoomController(this);
            }
        }
    }

    private void GenerateRoomHash()
    {
        // 🔹 Datos base para el hash
        string sceneName = SceneManager.GetActiveScene().name;
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

        // 🔹 Combinar en un string
        string raw = $"{sceneName}_{gameObject.name}_{pos2D.x:F2}_{pos2D.y:F2}";

        // 🔹 Crear hash MD5 para guardarlo compacto
        roomHash = Hash128.Compute(raw).ToString();
    }

    public Vector2 GetWorldSize2D()
    {
        return new Vector2(
            roomBounds.size.x * transform.lossyScale.x,
            roomBounds.size.y * transform.lossyScale.y
        );
    }

    public Vector2 GetWorldCenter2D()
    {
        Vector3 worldCenter = roomBounds.bounds.center;
        return new Vector2(worldCenter.x, worldCenter.y);
    }

    public void SetMapTileData(MapTileData data)
    {
        mapTileData = data;
    }

    public void SetRoomDiscovered()
    {
        if (roomDiscovered) return;
        roomDiscovered = true;

        var save = SaveDataController.Instance.saveData;
        if (!save.roomVisited.Contains(roomHash))
            save.roomVisited.Add(roomHash);

        // 🔹 Activar el tile directamente
        if (mapTileData != null && mapTileData.tileObject != null)
        {
            mapTileData.tileObject.SetActive(true);
            mapTileData.discovered = true;
        }

        // 🔹 Activar todas las entradas de este cuarto
        foreach (var entry in entries)
        {
            var entryData = MapController.Instance.mapEntries.Find(e => e.chunk == entry);
            if (entryData != null && entryData.markerObject != null)
            {
                entryData.markerObject.SetActive(true);
                entryData.discovered = true;
            }
        }
    }
}
