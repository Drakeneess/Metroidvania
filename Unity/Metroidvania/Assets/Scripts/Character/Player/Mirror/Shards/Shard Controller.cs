using System.Collections.Generic;
using UnityEngine;

public class ShardController : MonoBehaviour
{
    public List<ShardTool> shardTools;
    public List<Shard> shards;

    private Dictionary<ShardTool, Shard> weaponShardDictionary;

    void Start()
    {
        weaponShardDictionary = new Dictionary<ShardTool, Shard>();

        for (int i = 0; i < shardTools.Count; i++)
        {
            ShardTool tool = shardTools[i];
            Shard shard = shards[i];

            // Vincular herramienta con su fragmento
            shard.ShardTool = tool;
            // Registrar en diccionario
            weaponShardDictionary.Add(tool, shard);
        }
    }
}
