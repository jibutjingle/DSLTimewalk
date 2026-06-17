using System.Collections;
using System.Collections.Generic;
// using Unity.Netcode;
using UnityEngine;

public class MoveMusicNotesIntoScene : MonoBehaviour
{
    public List<GameObject> musicNotes = new List<GameObject>();
    //public float newYLocation = 1.5f; // new y location set in the signal emitter "GameStartStudyRoom"

    // [ServerRpc]
    public void MoveNotesServerRpc(float newY)
    {
        // if (IsServer)
        // {
        MoveNotesClientRpc(newY);
        // }
    }

    // [ClientRpc]
    private void MoveNotesClientRpc(float n)
    {
        MoveNotesIntoGamePosition(n);
    }
    private void MoveNotesIntoGamePosition(float newYLocation)
    {
        foreach (GameObject obj in musicNotes)
        {
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, newYLocation, obj.transform.localPosition.z);
        }
    }
}
