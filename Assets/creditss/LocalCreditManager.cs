using UnityEngine;
using System.Collections.Generic;

public class LocalCreditManager : MonoBehaviour
{
    public List<LocalCreditHover> allCredits = new List<LocalCreditHover>();

    private void Awake()
    {
        allCredits.Clear();

        LocalCreditHover[] credits = FindObjectsByType<LocalCreditHover>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        allCredits.AddRange(credits);
    }

    public void SelectCredit(LocalCreditHover selectedCredit)
    {
        foreach (LocalCreditHover credit in allCredits)
        {
            if (credit == null) continue;

            if (credit == selectedCredit)
                credit.SetSelected();
            else
                credit.SetDim();
        }
    }

    public void ClearSelection()
    {
        foreach (LocalCreditHover credit in allCredits)
        {
            if (credit != null)
                credit.SetNormal();
        }
    }
}