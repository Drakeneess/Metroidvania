using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cure : MonoBehaviour
{
    [SerializeField] private float physicalHealthRegen = 40;
    public float PhysicalHealthRegen => physicalHealthRegen;
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ActivateCure()
    {
        gameObject.SetActive(true);
    }
    public void DeactivateCure()
    {
        gameObject.SetActive(false);
    }
}
