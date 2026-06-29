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
    [SerializeField] private List<GameObject> puzzlePieces;
    [SerializeField] private GameObject staticPuzzlePieces;
    [SerializeField] private GameObject snapZoneParent;
    [SerializeField] private PlayableDirector JosePuzzleWaitPD;
    [SerializeField] private PlayableDirector eleanorPD;
    [SerializeField] private GameObject outlines;
    [SerializeField] private AudioClip soundConfirm;
    [SerializeField] private GameObject completedPuzzle;
    private OutlineDraw outlineDraw;

    private bool eleanorPDpaused = false;
    private bool josePuzzlePDPaused = true;
    private bool puzzleCompleted = false;

    void OnEnable()
    {
        SubscribeToPiecePlacedEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromPiecePlacedEvents();
    }

    public void TogglePauseLocal()
    {
        if (!eleanorPDpaused && JosePuzzleWaitPD)
        {
            // Play the Puzzle Game Timeline on jose GO & pause Eleanor's
            eleanorPD.Pause();
            JosePuzzleWaitPD.Play();
            eleanorPDpaused = true;
            josePuzzlePDPaused = false;
            foreach (GameObject piece in puzzlePieces)
            {
                MovedPiecesUp(.5f);
                // Debug.Log("Moved piece: " + piece.name + "to new position: " + piece.transform.localPosition);
            }

            outlineDraw = snapZoneParent.GetComponent<OutlineDraw>();
            if (outlineDraw != null)
            {
                outlineDraw.enabled = true;
            }

            outlines.SetActive(true);

            if (AreAllPiecesPlaced())
                ResumeEleanorAfterPuzzleComplete();
        }
    }

    void SubscribeToPiecePlacedEvents()
    {
        foreach (GameObject piece in puzzlePieces)
        {
            if (piece == null)
                continue;

            PlaceableObject placeable = piece.GetComponent<PlaceableObject>();
            if (placeable != null)
                placeable.onPlaced.AddListener(OnPiecePlaced);
        }
    }

    void UnsubscribeFromPiecePlacedEvents()
    {
        foreach (GameObject piece in puzzlePieces)
        {
            if (piece == null)
                continue;

            PlaceableObject placeable = piece.GetComponent<PlaceableObject>();
            if (placeable != null)
                placeable.onPlaced.RemoveListener(OnPiecePlaced);
        }
    }

    void OnPiecePlaced()
    {
        if (!eleanorPDpaused || puzzleCompleted)
            return;

        if (AreAllPiecesPlaced())
            ResumeEleanorAfterPuzzleComplete();
    }

    bool AreAllPiecesPlaced()
    {
        if (puzzlePieces == null || puzzlePieces.Count == 0)
            return false;

        foreach (GameObject piece in puzzlePieces)
        {
            if (piece == null)
                return false;

            PlaceableObject placeable = piece.GetComponent<PlaceableObject>();
            if (placeable == null || !placeable.IsPlaced)
                return false;
        }

        return true;
    }

    void ResumeEleanorAfterPuzzleComplete()
    {
        if (puzzleCompleted || !eleanorPDpaused)
            return;

        puzzleCompleted = true;

        if (JosePuzzleWaitPD != null)
            JosePuzzleWaitPD.Pause();

        if (eleanorPD != null)
            eleanorPD.Resume();

        eleanorPDpaused = false;
        josePuzzlePDPaused = true;

        if (outlineDraw != null)
            outlineDraw.enabled = false;

        if (outlines != null)
            outlines.SetActive(false);

        if (completedPuzzle != null)
            completedPuzzle.SetActive(true);

        foreach (GameObject piece in puzzlePieces)
        {
            piece.SetActive(false);
        }

        if (staticPuzzlePieces != null)
            staticPuzzlePieces.SetActive(false);

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

    public void RemovePuzzleGame()
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
