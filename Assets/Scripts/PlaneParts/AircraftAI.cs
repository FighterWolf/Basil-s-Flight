using UnityEngine;
using System.Collections.Generic;

public class AircraftAI : MonoBehaviour
{
    Aircraft plane;
    Aircraft planeToFollow;
    VFormationSpot spotToFollow;
    private float distanceToSpotToFollow;
    
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
        this.planeToFollow = plane.planeToFollow;
        plane.whichSpotToFollow = FindFirstEmptyFormationSpot();
        spotToFollow = plane.whichSpotToFollow;
        if (spotToFollow != null)
        {
            EssentialFunctions.AimForTarget(transform, spotToFollow.transform, 5f);
        }
        HandleSpeed();
    }

    void HandleSpeed()
    {
        distanceToSpotToFollow = Vector3.Distance(transform.position,spotToFollow.transform.position);
        
        ChangeSpeed(planeToFollow.speed);
    }

    void ChangeSpeed(float desiredSpeed)
    {   
        if (plane.speed<desiredSpeed)
        {
            plane.Accelerate(1);
        }
    }

    public VFormationSpot FindFirstEmptyFormationSpot()
    {
        if (spotToFollow != null)
        {
            return spotToFollow;
        }

        //planeToFollow.AddAllLastTrailingAircraft(planeToFollow,planeToFollow.listOfLastTrailingPlanes);

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

        foreach (Aircraft a in planeToFollow.listOfLastTrailingPlanes)
        {
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
