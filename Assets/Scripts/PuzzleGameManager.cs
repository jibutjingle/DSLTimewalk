using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
// using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class PuzzleGameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> puzzlePieces;
    [SerializeField]
    private List<GameObject> staticPuzzlePieces;
    [SerializeField]
    private GameObject snapZoneParent;
    // [SerializeField]
    // private GameObject puzzleButton;
    [SerializeField]
    private PlayableDirector JosePuzzleWaitPD;
    [SerializeField]
    private PlayableDirector eleanorPD;
    [SerializeField]
    private GameObject outlines;
    [SerializeField]
    private AudioClip soundConfirm;

    private bool eleanorPDpaused = false;
    private bool josePuzzlePDPaused = true;

    public void TogglePauseLocal()
    {
        if (!eleanorPDpaused && JosePuzzleWaitPD)
        {
            // Play the Puzzle Game Timeline on jose GO & pause Eleanor's
            eleanorPD.Pause();
            JosePuzzleWaitPD.Play();
            eleanorPDpaused = true;
            josePuzzlePDPaused = false;
            LayerMask defaultLayer = LayerMask.NameToLayer("KeepOriginal");
            foreach (GameObject piece in puzzlePieces)
            {
                // if (IsServer)
                // {
                //     piece.layer = defaultLayer;
                //     piece.GetComponent<Collider>().enabled = true;
                // }
                // piece.layer = defaultLayer;
                // piece.GetComponent<Collider>().enabled = true;
                MovePiecesUpServerRpc(.5f);
                // Debug.Log("Moved piece: " + piece.name + "to new position: " + piece.transform.localPosition);
            }

            foreach (GameObject piece in staticPuzzlePieces)
            {
                piece.layer = defaultLayer;
                piece.GetComponent<Collider>().enabled = true;
            }

            // GuideFromZonesDraw guideFromZonesDraw = snapZoneParent.GetComponent<GuideFromZonesDraw>();
            // if (guideFromZonesDraw != null)
            // {
            //     guideFromZonesDraw.enabled = true;
            // }

            // puzzleButton.SetActive(true);
            outlines.SetActive(true);
        }
    }

    // [ServerRpc(RequireOwnership = false)]
    public void MovePiecesUpServerRpc(float newY)
    {
        MovePiecesUpClientRpc(newY);
    }

    // [ClientRpc]
    private void MovePiecesUpClientRpc(float newY)
    {
        MovedPiecesUp(newY);
    }

    private void MovedPiecesUp(float newY)
    {
        foreach (GameObject piece in puzzlePieces)
        {
            // piece.transform.position = new Vector3(piece.transform.position.x, newY, piece.transform.position.z);
            piece.transform.localPosition = new Vector3(piece.transform.localPosition.x, newY, piece.transform.localPosition.z);
            // Debug.Log("Moved piece: " + piece.name + "to new position: " + piece.transform.position);
        }
    }

    public void TogglePause()
    {
        // if (IsServer)
        // {
        if (eleanorPD == null)
        {
            Debug.LogWarning("PlayableDirector is not assigned!");
            return;
        }
        TogglePauseServerRpc();
        // }
    }

    // [ServerRpc(RequireOwnership = false)]
    private void TogglePauseServerRpc()
    {
        TogglePauseClientRpc();
    }

    // [ClientRpc]
    private void TogglePauseClientRpc()
    {
        if (!eleanorPDpaused && JosePuzzleWaitPD)
        {
            // Play the Puzzle Game Timeline on jose GO & pause Eleanor's
            eleanorPD.Pause();
            JosePuzzleWaitPD.Play();
            eleanorPDpaused = true;
            josePuzzlePDPaused = false;
            LayerMask defaultLayer = LayerMask.NameToLayer("KeepOriginal");
            foreach (GameObject piece in puzzlePieces)
            {
                piece.layer = defaultLayer;
                piece.GetComponent<Collider>().enabled = true;
            }

            // GuideFromZonesDraw guideFromZonesDraw = snapZoneParent.GetComponent<GuideFromZonesDraw>();
            // if (guideFromZonesDraw != null)
            // {
            //     guideFromZonesDraw.enabled = true;
            // }

            // puzzleButton.SetActive(true);
            outlines.SetActive(true);
        }
        else if (eleanorPDpaused && !josePuzzlePDPaused)
        {
            // Pause the Timeline
            JosePuzzleWaitPD.Pause();
            eleanorPD.Play();
            josePuzzlePDPaused = true;
            eleanorPDpaused = false;
        }
    }

    public void RemovePuzzleGame()
    {
        // if (IsServer)
        // {
        RemovePuzzleGameServerRpc();
        // }
    }

    // [ServerRpc(RequireOwnership = false)]
    public void RemovePuzzleGameServerRpc()
    {
        RemovePuzzleGameClientRpc();
    }

    // [ClientRpc]
    public void RemovePuzzleGameClientRpc()
    {
        if (snapZoneParent != null)
        {
            Destroy(snapZoneParent);
        }

        var completePuzzle = GameObject.Find("PuzzleFixedOriginsComplete");
        if (completePuzzle != null)
        {
            Destroy(completePuzzle);
        }
    }

    public AudioClip GetConfirmationSound()
    {
        return soundConfirm;
    }

}
