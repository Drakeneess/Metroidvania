using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    protected bool isDeployed=false;
    protected bool areOptionsDeployed=false;

    public bool IsDeployed { get { return isDeployed; } }
    public bool AreOptionsDeployed { get { return areOptionsDeployed; } }
}
