using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName = "";
    public float physicalHealth = 100f;
    public float mentalHealth = 50f;
    public float emotionalHealth = 20f;

    public int currentWeapon = 0;
    public List<int> toolUnlocked = new List<int>();
    public List<IntIntPair> toolLevel = new List<IntIntPair>();

    public int currentSkill = -1;
    public List<int> skillUnlocked = new List<int>();

    public float timePlayed = 0f;
    public int lastScene = 0;
    public int lastCheckpointIndex = 0;

    public List<string> checkpointsUnlocked = new List<string>();
    public List<string> roomVisited = new List<string>();

    public List<IntIntPair> conversationsCompleted = new List<IntIntPair>();

    public int studentID = -1;
    public int playthroughID = -1;
}


[System.Serializable]
public struct IntIntPair
{
    public int key;
    public int value;
}
