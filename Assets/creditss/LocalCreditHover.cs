using UnityEngine;
using TMPro;

public class LocalCreditHover : MonoBehaviour
{
    [Header("Text")]
    public TextMeshPro creditText;

    [Header("Optional Role/Title Text")]
    public GameObject titleText;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1f, 0.72f, 0.25f);
    public Color dimColor = new Color(0.25f, 0.25f, 0.25f);

    private void Awake()
    {
        if (creditText == null)
            creditText = GetComponent<TextMeshPro>();

        if (creditText == null)
            creditText = GetComponentInChildren<TextMeshPro>(true);

        if (titleText == null)
        {
            Transform child = transform.Find("TitleText");
            if (child != null)
                titleText = child.gameObject;
        }
    }

    private void Start()
    {
        SetNormal();
    }

    public void SetSelected()
    {
        if (creditText != null)
            creditText.color = selectedColor;

        if (titleText != null)
            titleText.SetActive(true);
    }

    public void SetDim()
    {
        if (creditText != null)
            creditText.color = dimColor;

        if (titleText != null)
            titleText.SetActive(false);
    }

    public void SetNormal()
    {
        if (creditText != null)
            creditText.color = normalColor;

        if (titleText != null)
            titleText.SetActive(false);
    }
}