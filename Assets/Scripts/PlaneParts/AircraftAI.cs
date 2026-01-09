using UnityEngine;
using System.Collections.Generic;

public class AircraftAI : MonoBehaviour
{
    Aircraft plane;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<Aircraft>();
    }

    // Update is called once per frame
    void Update()
    {
        FollowAircraft();
    }

    void FollowAircraft()
    {
        plane.whichSpotToFollow = FindFirstEmptyFormationSpot();
        if (plane.whichSpotToFollow!=null)
        {
            EssentialFunctions.AimForTarget(transform, plane.whichSpotToFollow.transform, 5f);
        }
    }
    public VFormationSpot FindFirstEmptyFormationSpot()
    {
        if (plane.whichSpotToFollow != null)
        {
            return plane.whichSpotToFollow;
        }

        plane.planeToFollow.AddAllLastTrailingAircraft(plane.planeToFollow);

        void ChooseFormationPosition(VFormationSpot.LeftORRight position)
        {
            switch (position)
            {
                case VFormationSpot.LeftORRight.Left:
                    plane.formationPosition = Aircraft.FormationPosition.Left;
                    break;
                case VFormationSpot.LeftORRight.Right:
                    plane.formationPosition = Aircraft.FormationPosition.Right;
                    break;
            }
        }

        VFormationSpot FindCorrectSpot(VFormationSpot[] slots, VFormationSpot.LeftORRight direction)
        {
            ChooseFormationPosition(direction);
            foreach (VFormationSpot v in slots)
            {
                if (v.spot==direction)
                {
                    v.whoTakesTheSpot = plane;
                    return v;
                }
            }
            return null;
        }

        foreach (Aircraft a in plane.planeToFollow.listOfLastTrailingPlanes)
        {
            if (plane.whichSpotToFollow!=null)
            {
                return plane.whichSpotToFollow;
            } 

            //If one of the trailing planes is leading.
            if (a.isLeadPlane)
            {
                //Since both spots can be valid, check for first empty.
                foreach(VFormationSpot v in a.vFormations)
                {
                    if (v.whoTakesTheSpot == null)
                    {
                        //Is the plane on the lead plane's left or right?
                        ChooseFormationPosition(v.spot);
                        v.whoTakesTheSpot = plane;
                        return v;
                    }
                }
            }
            else
            {
                //Check if the trailer is on the right or left of the V shape
                switch (a.formationPosition)
                {
                    case Aircraft.FormationPosition.Left:
                        return FindCorrectSpot(a.vFormations, VFormationSpot.LeftORRight.Left);
                    case Aircraft.FormationPosition.Right:
                        return FindCorrectSpot(a.vFormations, VFormationSpot.LeftORRight.Right);
                }
            }
        }
        return null;
    }
}
