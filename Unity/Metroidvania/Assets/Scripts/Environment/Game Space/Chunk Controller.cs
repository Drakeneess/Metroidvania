using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    private Player player;
    private List<RoomController> roomControllers = new List<RoomController>();

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (other.TryGetComponent<Player>(out Player newPlayer))
        {
            player = newPlayer;
            CameraController.IsFollowingPlayer = false;

            // Marcar todos los cuartos relacionados como descubiertos
            foreach (var roomController in roomControllers)
            {
                if (!roomController.RoomDiscovered)
                {
                    roomController.SetRoomDiscovered();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (player != null)
        {
            CameraController.IsFollowingPlayer = true;
            player = null;
        }
    }

    public void SetRoomController(RoomController roomController)
    {
        this.roomControllers.Add(roomController);
    }
}
