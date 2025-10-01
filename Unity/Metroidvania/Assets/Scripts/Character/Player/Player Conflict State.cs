using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConflictStateController : MonoBehaviour
{
    public static PlayerConflictStateController Instance;
    private bool isInConflict = false;
    public bool IsInConflict { get => isInConflict; }
    void Awake()
    {
        Instance = this;
    }

    public void BeginCombat()
    {
        isInConflict = true;
     }
    public void EndCombat()
    {
        isInConflict = false;
     }
}
