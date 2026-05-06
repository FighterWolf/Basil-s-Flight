using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlaneSelector : MonoBehaviour
{
    public static GameObject planeToSummon;
    public static bool setDefault;

    public TMP_Dropdown dropdown;
    static int selectValue;
    public PlaneStats[] listOfPlanesToSelect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<string> aircraftNames = new List<string>();

        foreach (PlaneStats o in listOfPlanesToSelect)
        {
            string name = o.planeClass;
            if (!aircraftNames.Contains(o.planeClass))
            {
                aircraftNames.Add(name);
            }
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(aircraftNames);

        if (!setDefault)
        {
            planeToSummon = listOfPlanesToSelect[0].gameObject;
            setDefault = true;
        }
        dropdown.value = selectValue;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeAircraftModel()
    {
        selectValue = dropdown.value;
        planeToSummon = listOfPlanesToSelect[selectValue].gameObject;
    }
}
