using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for multiple tabbed pages of flat content
/// </summary>
public class TabHolder : MonoBehaviour
{
    public int DesiredActivePage;
    private int CurrentlyActivePage;

    public List<GameObject> pages;

    public void NextPage()
    {
        DesiredActivePage = (DesiredActivePage + 1) % pages.Count;
        SetActivePage();
    }
    public void PreviousPage()
    {
        DesiredActivePage = (DesiredActivePage - 1) % pages.Count;
        SetActivePage();
    }

    public void SetActivePage()
    {
        foreach (var page in pages)
        {
            page.SetActive(false);
        }

        pages[DesiredActivePage].SetActive(true);
        CurrentlyActivePage = DesiredActivePage;
    }

    private void Update()
    {
        if(CurrentlyActivePage != DesiredActivePage)
        {
            SetActivePage();
        }
    }

}
