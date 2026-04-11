using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlaneSelector : MonoBehaviour
{
    public static GameObject planeToSummon;
    public static bool setDefault;

    public TMP_Dropdown dropdown;
    int selectValue;
    public GameObject[] listOfPlanesToSelect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<string> aircraftNames = new List<string>();

        foreach (GameObject o in listOfPlanesToSelect)
        {
            string name = o.name;
            if (!aircraftNames.Contains(o.name))
            {
                aircraftNames.Add(name);
            }
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(aircraftNames);

        if (!setDefault)
        {
            planeToSummon = listOfPlanesToSelect[0];
            setDefault = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(planeToSummon);
    }

    public void ChangeAircraftModel()
    {
        selectValue = dropdown.value;
        planeToSummon = listOfPlanesToSelect[selectValue];
    }
}
