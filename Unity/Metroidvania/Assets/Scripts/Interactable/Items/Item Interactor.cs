using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractor : Interactable
{
    [SerializeField] private Item item;

    protected override void Start()
    {
        base.Start();
        if (item == null)
        {
            item = GetComponentInChildren<Item>();
        }
    }
    protected override void Action()
    {
        base.Action(); // desactiva el botón y loguea

        var extras = new List<string>
        {
            $"Object: {item?.name ?? "Unknown"}",
            $"InteractionType: {interactionType}"
        };
        LogBegin(extras);

        if (item == null)
        {
            item = GetComponentInChildren<Item>(true);
            if (item == null)
            {
                Debug.LogError($"[{name}] ItemInteractor: 'item' es NULL, no puedo PickUpItem().");
                return;
            }
        }

        try
        {
            item.PickUpItem();
            isInteractable = false;
            LogEnd(extras);
            Debug.Log($"[{name}] Item recogido: {item.name}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{name}] Error en PickUpItem(): {ex.Message}\n{ex.StackTrace}");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
