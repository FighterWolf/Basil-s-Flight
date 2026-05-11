using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlaneSelector : MonoBehaviour
{
    public static GameObject planeToSummon;
    public static bool setDefaultPlane;

    public static GameObject pilotToSummon;
    public static bool setDefaultPilot;

    public TMP_Dropdown dropdownPlane;
    static int selectValuePlane;
    public PlaneStats[] listOfPlanesToSelect;

    public TMP_Dropdown dropdownPilot;
    static int selectValuePilot;
    public PilotModel[] listOfPilotsToSelect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<string> aircraftNames = new List<string>();
        List<string> pilotNames = new List<string>();

        foreach (PlaneStats o in listOfPlanesToSelect)
        {
            string name = o.planeClass;
            if (!aircraftNames.Contains(o.planeClass))
            {
                aircraftNames.Add(name);
            }
        }

        foreach (PilotModel m in listOfPilotsToSelect)
        {
            string name = m.characterName;
            if (!pilotNames.Contains(m.characterName))
            {
                pilotNames.Add(name);
            }
        }

        if (!setDefaultPlane)
        {
            planeToSummon = listOfPlanesToSelect[0].gameObject;
            setDefaultPlane = true;
        }

        if (!setDefaultPilot)
        {
            pilotToSummon = listOfPilotsToSelect[0].gameObject;
            setDefaultPilot = true;
        }

        SummonModel(ref dropdownPlane, ref selectValuePlane, aircraftNames);
        SummonModel(ref dropdownPilot, ref selectValuePilot, pilotNames);
    }

    void SummonModel(ref TMP_Dropdown drop, ref int value, List<string> names)
    {
        drop.ClearOptions();
        drop.AddOptions(names);
        drop.value = value;
    }

    public void ChangeAircraftModel()
    {
        selectValuePlane = dropdownPlane.value;
        planeToSummon = listOfPlanesToSelect[selectValuePlane].gameObject;
    }

    public void ChangeAllyModel()
    {
        selectValuePilot = dropdownPilot.value;
        pilotToSummon = listOfPilotsToSelect[selectValuePilot].gameObject;
    }
}
