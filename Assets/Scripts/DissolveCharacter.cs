using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DissolveCharacter : MonoBehaviour
{
    [SerializeField] List<GameObject> characterParents = new List<GameObject>();
    [SerializeField] float dissolveDuration = 1f;

    const string DissolveProperty = "_DissolveAmount";

    readonly List<Material> dissolveMaterials = new List<Material>();
    readonly List<float> originalDissolveValues = new List<float>();

    Coroutine dissolveCoroutine;

    void Start()
    {
        CollectDissolveMaterials();
    }

    public void StartDissolve()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        dissolveCoroutine = StartCoroutine(DissolveRoutine());
    }

    void OnDestroy()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        ResetMaterials();
    }

    void CollectDissolveMaterials()
    {
        dissolveMaterials.Clear();
        originalDissolveValues.Clear();

        if (characterParents == null || characterParents.Count == 0)
            return;

        HashSet<Material> seenMaterials = new HashSet<Material>();

        foreach (GameObject root in GetUniqueRoots(characterParents))
        {
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    if (material != null &&
                        material.HasProperty(DissolveProperty) &&
                        seenMaterials.Add(material))
                    {
                        dissolveMaterials.Add(material);
                        originalDissolveValues.Add(material.GetFloat(DissolveProperty));
                    }
                }
            }
        }
    }

    static List<GameObject> GetUniqueRoots(List<GameObject> parents)
    {
        List<GameObject> uniqueRoots = new List<GameObject>();

        for (int i = 0; i < parents.Count; i++)
        {
            GameObject candidate = parents[i];
            if (candidate == null)
                continue;

            bool isDescendantOfAnotherEntry = false;
            for (int j = 0; j < parents.Count; j++)
            {
                if (i == j)
                    continue;

                GameObject other = parents[j];
                if (other == null)
                    continue;

                if (candidate.transform.IsChildOf(other.transform))
                {
                    isDescendantOfAnotherEntry = true;
                    break;
                }
            }

            if (!isDescendantOfAnotherEntry)
                uniqueRoots.Add(candidate);
        }

        return uniqueRoots;
    }

    IEnumerator DissolveRoutine()
    {
        if (dissolveMaterials.Count == 0)
            yield break;

        float elapsed = 0f;

        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dissolveDuration);

            for (int i = 0; i < dissolveMaterials.Count; i++)
            {
                float value = Mathf.Lerp(originalDissolveValues[i], 1f, t);
                dissolveMaterials[i].SetFloat(DissolveProperty, value);
            }

            yield return null;
        }

        for (int i = 0; i < dissolveMaterials.Count; i++)
            dissolveMaterials[i].SetFloat(DissolveProperty, 1f);

        dissolveCoroutine = null;
    }

    void ResetMaterials()
    {
        for (int i = 0; i < dissolveMaterials.Count; i++)
            dissolveMaterials[i].SetFloat(DissolveProperty, originalDissolveValues[i]);
    }
}
