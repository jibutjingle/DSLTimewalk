// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;

// public class DMVFade : NetworkBehaviour
// {
//     [Header("Set Active Objects")]
//     public List<GameObject> dmvObjects = new List<GameObject>();

//     [Header("Dissolve Settings")]
//     public List<Material> dissolveMaterials = new List<Material>();

//     public float dissolveSpeed = 0.5f;

//     private const string DISSOLVE_PROPERTY = "_DissolveAmount";

//     private Coroutine currentDissolveCoroutine;

//     [ServerRpc(RequireOwnership = false)]
//     public void DisableDMVServerRpc()
//     {
//         DisableDMVClientRpc();
//     }

//     [ClientRpc]
//     void DisableDMVClientRpc()
//     {
//         DisableDMV();
//     }

//     public void DisableDMV()
//     {
//         if (currentDissolveCoroutine != null)
//         {
//             StopCoroutine(currentDissolveCoroutine);
//         }

//         foreach (var obj in dmvObjects)
//         {
//             if (obj != null)
//                 obj.SetActive(false);
//         }

//         currentDissolveCoroutine = StartCoroutine(AnimateDissolve(1f));
//     }

//     [ServerRpc(RequireOwnership = false)]
//     public void EnableDMVServerRpc()
//     {
//         EnableDMVClientRpc();
//     }

//     [ClientRpc]
//     void EnableDMVClientRpc()
//     {
//         EnableDMV();
//     }
//     public void EnableDMV()
//     {
//         if (currentDissolveCoroutine != null)
//         {
//             StopCoroutine(currentDissolveCoroutine);
//         }

//         currentDissolveCoroutine = StartCoroutine(AnimateDissolve(0f));

//         foreach (var obj in dmvObjects)
//         {
//             if (obj != null)
//                 obj.SetActive(true);
//         }
//     }

//     private IEnumerator AnimateDissolve(float targetValue)
//     {
//         bool allComplete = false;

//         while (!allComplete)
//         {
//             allComplete = true;

//             foreach (var mat in dissolveMaterials)
//             {
//                 if (mat == null || !mat.HasProperty(DISSOLVE_PROPERTY))
//                     continue;

//                 float currentValue = mat.GetFloat(DISSOLVE_PROPERTY);

//                 float direction = targetValue > currentValue ? 1f : -1f;
//                 float newValue = currentValue + (direction * dissolveSpeed * Time.deltaTime);

//                 if (direction > 0)
//                 {
//                     newValue = Mathf.Min(newValue, targetValue);
//                 }
//                 else
//                 {
//                     newValue = Mathf.Max(newValue, targetValue);
//                 }

//                 mat.SetFloat(DISSOLVE_PROPERTY, newValue);

//                 if (Mathf.Abs(newValue - targetValue) > 0.001f)
//                 {
//                     allComplete = false;
//                 }
//             }

//             yield return null;
//         }

//         foreach (var mat in dissolveMaterials)
//         {
//             if (mat != null && mat.HasProperty(DISSOLVE_PROPERTY))
//                 mat.SetFloat(DISSOLVE_PROPERTY, targetValue);
//         }
//     }

//     void OnDisable()
//     {
//         foreach (var mat in dissolveMaterials)
//         {
//             if (mat != null)
//                 mat.SetFloat(DISSOLVE_PROPERTY, 0f);
//         }
//     }
// }