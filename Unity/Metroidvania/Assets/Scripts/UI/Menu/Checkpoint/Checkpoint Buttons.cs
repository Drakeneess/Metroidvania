using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenuButtons : MenuButtons
{
    [Header("Refs")]
    public CheckpointMenu checkpointMenu;

    protected override void Start()
    {
        base.Start();
        // Suponiendo orden: 0=Rest, 1=Travel, 2=Exit
        buttons[0].onClick.AddListener(OnRest);
        buttons[1].onClick.AddListener(OnTravel);
        buttons[2].onClick.AddListener(OnExit);
    }

    private void OnRest()
    {
        CheckpointMenuController.Instance.DoRest();
        // Si quieres cerrar al descansar, descomenta
        CheckpointMenuController.Instance.Close();
    }

    private void OnTravel()
    {
        CheckpointMenuController.Instance.OpenFastTravel();
        // Mantener menú abierto o cambiar a mapa según tu UX
    }
    private void OnExit()
    {
        CheckpointMenuController.Instance.Close();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void Select(string actionName)
    {
        base.Select(actionName);
        if (actionName == "Back")
        {
            checkpointMenu.Close();
            
        }
    }
}
