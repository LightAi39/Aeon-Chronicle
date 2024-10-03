using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject skillPanel;
    public GameObject itemPanel;

    public void ShowSkillPanel()
    {
        skillPanel.SetActive(true);
        itemPanel.SetActive(false);
    }

    public void ShowItemPanel()
    {
        skillPanel.SetActive(false);
        itemPanel.SetActive(true);
    }
}
