// using System;
// using System.Collections.Generic;
// using UnityEngine;
// // using UnityEngine.XR.Interaction.Toolkit;
// // using UnityEngine.XR.Interaction.Toolkit.Interactables;

// // [RequireComponent(typeof(XRGrabInteractable))]
// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(AudioSource))]
// public class ObjectPlacer : MonoBehaviour
// {
//     [Serializable]
//     public class SnapPointData
//     {
//         [Header("Primary Snap Point")]
//         public Transform snapWaypoint;
//         public GameObject shadowPrefab;
//         public AudioClip shadowSpawnClip;
//         public AudioClip snapCompleteClip;

//         [Header("Secondary Snap Point")]
//         public Transform secondarySnapWaypoint;
//         public GameObject secondaryShadowPrefab;
//         public AudioClip secondaryShadowSpawnClip;
//         public AudioClip secondarySnapCompleteClip;

//         [Header("Identity Prefabs")]
//         public GameObject primaryOriginalPrefab;
//         public GameObject secondaryOriginalPrefab;
//     }

//     public List<SnapPointData> snapPoints = new List<SnapPointData>();

//     [Header("Settings")]
//     public float snapDistance = 0.3f;

//     private AudioSource audioSource;
//     private XRGrabInteractable grabInteractable;
//     private Rigidbody rb;

//     private bool isGrabbed = false;
//     private GameObject currentShadow;
//     private SnapPointData currentSnapPoint;
//     private bool usingSecondaryPoint = false;

//     private static HashSet<Transform> occupiedSnapPoints = new HashSet<Transform>(); // Prevent double placement
//     private WaypointFloatingSpawner spawner;

//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody>();
//         grabInteractable = GetComponent<XRGrabInteractable>();
//         audioSource = GetComponent<AudioSource>();

//         grabInteractable.selectEntered.AddListener(OnGrabbed);
//         grabInteractable.selectExited.AddListener(OnReleased);
//     }

//     public void SetSpawner(WaypointFloatingSpawner s) => spawner = s;

//     private void Update()
//     {
//         if (!isGrabbed || snapPoints.Count == 0) return;

//         SnapPointData nearestSnap = null;
//         float nearestDist = float.MaxValue;
//         bool nearestIsSecondary = false;

//         foreach (var snap in snapPoints)
//         {
//             // Primary snap
//             if (snap.snapWaypoint != null && !occupiedSnapPoints.Contains(snap.snapWaypoint))
//             {
//                 float dist = Vector3.Distance(transform.position, snap.snapWaypoint.position);
//                 if (dist < nearestDist)
//                 {
//                     nearestDist = dist;
//                     nearestSnap = snap;
//                     nearestIsSecondary = false;
//                 }
//             }

//             // Secondary snap
//             if (snap.secondarySnapWaypoint != null && !occupiedSnapPoints.Contains(snap.secondarySnapWaypoint))
//             {
//                 float dist = Vector3.Distance(transform.position, snap.secondarySnapWaypoint.position);
//                 if (dist < nearestDist)
//                 {
//                     nearestDist = dist;
//                     nearestSnap = snap;
//                     nearestIsSecondary = true;
//                 }
//             }
//         }

//         if (nearestSnap != null && nearestDist <= snapDistance)
//         {
//             bool shadowNeedsChange = (currentSnapPoint != nearestSnap) || (usingSecondaryPoint != nearestIsSecondary);

//             if (shadowNeedsChange)
//             {
//                 if (currentShadow != null)
//                     Destroy(currentShadow);

//                 if (!nearestIsSecondary && nearestSnap.shadowPrefab != null)
//                 {
//                     currentShadow = Instantiate(nearestSnap.shadowPrefab,
//                                                 nearestSnap.snapWaypoint.position,
//                                                 nearestSnap.snapWaypoint.rotation);
//                     if (nearestSnap.shadowSpawnClip != null)
//                         audioSource.PlayOneShot(nearestSnap.shadowSpawnClip);
//                 }
//                 else if (nearestIsSecondary && nearestSnap.secondaryShadowPrefab != null)
//                 {
//                     currentShadow = Instantiate(nearestSnap.secondaryShadowPrefab,
//                                                 nearestSnap.secondarySnapWaypoint.position,
//                                                 nearestSnap.secondarySnapWaypoint.rotation);
//                     if (nearestSnap.secondaryShadowSpawnClip != null)
//                         audioSource.PlayOneShot(nearestSnap.secondaryShadowSpawnClip);
//                 }
//             }

//             if (currentShadow != null)
//             {
//                 Transform target = nearestIsSecondary ? nearestSnap.secondarySnapWaypoint : nearestSnap.snapWaypoint;
//                 currentShadow.transform.position = target.position;
//                 currentShadow.transform.rotation = target.rotation;
//             }

//             currentSnapPoint = nearestSnap;
//             usingSecondaryPoint = nearestIsSecondary;
//         }
//         else
//         {
//             if (currentShadow != null)
//                 Destroy(currentShadow);

//             currentSnapPoint = null;
//             usingSecondaryPoint = false;
//         }
//     }

//     private void OnGrabbed(SelectEnterEventArgs args) => isGrabbed = true;

//     private void OnReleased(SelectExitEventArgs args)
//     {
//         isGrabbed = false;

//         if (currentShadow != null)
//         {
//             Destroy(currentShadow);
//         }

//         if (currentSnapPoint != null)
//         {
//             Transform targetWaypoint = usingSecondaryPoint ? currentSnapPoint.secondarySnapWaypoint : currentSnapPoint.snapWaypoint;

//             if (targetWaypoint != null &&
//                 Vector3.Distance(transform.position, targetWaypoint.position) <= snapDistance &&
//                 !occupiedSnapPoints.Contains(targetWaypoint))
//             {
//                 rb.velocity = Vector3.zero;
//                 rb.angularVelocity = Vector3.zero;
//                 rb.isKinematic = true;

//                 transform.position = targetWaypoint.position;
//                 transform.rotation = targetWaypoint.rotation;

//                 grabInteractable.enabled = false;
//                 occupiedSnapPoints.Add(targetWaypoint);

//                 // Play correct snap sound
//                 if (!usingSecondaryPoint && currentSnapPoint.snapCompleteClip != null)
//                     audioSource.PlayOneShot(currentSnapPoint.snapCompleteClip);
//                 else if (usingSecondaryPoint && currentSnapPoint.secondarySnapCompleteClip != null)
//                     audioSource.PlayOneShot(currentSnapPoint.secondarySnapCompleteClip);

//                 // Restore original prefab (identity)
//                 if (!usingSecondaryPoint && currentSnapPoint.primaryOriginalPrefab != null)
//                 {
//                     Instantiate(currentSnapPoint.primaryOriginalPrefab, targetWaypoint.position, targetWaypoint.rotation);
//                 }
//                 else if (usingSecondaryPoint && currentSnapPoint.secondaryOriginalPrefab != null)
//                 {
//                     Instantiate(currentSnapPoint.secondaryOriginalPrefab, targetWaypoint.position, targetWaypoint.rotation);
//                 }

//                 // Stop floating from spawner
//                 if (spawner != null)
//                     spawner.StopFloating();

//                 Destroy(gameObject); // Remove placed temp object
//             }
//         }
//     }

//     internal void SetSnapTarget(Transform snapTarget)
//     {
//         throw new NotImplementedException();
//     }
// }
