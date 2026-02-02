using UnityEngine;
using System.Collections.Generic;

public class AircraftAI : MonoBehaviour
{
    Aircraft plane;
    Aircraft planeToFollow;
    VFormationSpot spotToFollow;

    public string enemyTag;

    public bool isHit;
    [SerializeField] private Transform[] waypoints;
    private Transform currentWaypoint;
    public int waypointsIterator = 0;
    private float waypointDistanceThreshhold=10;
    private float distanceToSpotToFollow;

    private List<Entity> listOfPotentialEnemies;
    private Entity enemy;
    private float enemyDistance;
    
    public enum State
    {
        Patroling,
        Following,
        Attacking,
        Evading,
        Disabled
    }

    public State state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<Aircraft>();
        this.planeToFollow = plane.planeToFollow;
    }

    // Update is called once per frame
    void Update()
    {
        state = ChangeState();

        switch (state)
        {
            case State.Patroling:
                Patrol();
                //SearchForClosestEnemy();
                break;
            case State.Following:
                FollowAircraft();
                SearchForClosestEnemy();
                break;
            case State.Attacking:
                break;
            case State.Evading:
                //Evade();
                break;
            case State.Disabled:
                DisableSelf();
                break;
        }
    }

    void FixedUpdate()
    {
        
    }
    State ChangeState()
    {
        if (GetComponent<Entity>().health <= 0)
        {
            return State.Disabled;
        }
        else if (isHit)
        {
            return State.Evading;
        }
        else if (enemy&&enemyDistance<500)
        {
            return State.Attacking;
        }
        else if (planeToFollow||spotToFollow)
        {
            return State.Following;
        }
        else if (waypoints.Length > 0)
        {
            return State.Patroling;
        }

        return State.Disabled;
    }

    void Evade()
    {
        //Bank like far left or far right for a few seconds
        //As in really sharp fucking turn
        isHit = false;
    }

    void Patrol()
    {
        if (!currentWaypoint||ReachedWaypoint())
        {
            SearchNextWaypoint();
        }

        if (currentWaypoint)
        {
            EssentialFunctions.AimForTarget(transform, currentWaypoint, 1.5f);
        }

        bool ReachedWaypoint()
        {
            if (Vector3.Distance(transform.position, currentWaypoint.position) < waypointDistanceThreshhold)
            {
                if (waypointsIterator == waypoints.Length - 1)
                {
                    waypointsIterator = 0;
                }
                else
                {
                    waypointsIterator++;
                }
                return true;
            }
            return false;
        }

        void SearchNextWaypoint()
        {
            if (waypoints.Length > 0)
            {
                currentWaypoint = waypoints[waypointsIterator];
            }
        }
    }
    
    void FollowAircraft()
    {
        plane.whichSpotToFollow = FindFirstEmptyFormationSpot();
        spotToFollow = plane.whichSpotToFollow;
        if (spotToFollow != null)
        {
            float percentageOfWayToTarget = Mathf.InverseLerp(1, 1000, Vector3.Distance(transform.position, spotToFollow.transform.position));
            EssentialFunctions.AimForTarget(transform, spotToFollow.transform, 1.5f);
            HandleSpeed();
        }
    }

    void DisableSelf()
    {
        plane.speed = 0;
    }

    void SearchForClosestEnemy()
    {
        if (gameObject.CompareTag("BluFor"))
        {
            listOfPotentialEnemies = Entity.opForEntity;
        }
        else if (gameObject.CompareTag("OpFor"))
        {
            listOfPotentialEnemies = Entity.bluForEntity;
        }

        if (enemy&&enemy.health <= 0)
        {
            enemy = null;
            enemyDistance = Mathf.Infinity;
        }

        if (listOfPotentialEnemies.Count>0)
        {
            float closestDistance = Mathf.Infinity;
            Entity closestEnemy=null;
            foreach(Entity e in listOfPotentialEnemies)
            {
                float distance = Vector3.Distance(e.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }
            enemy = closestEnemy;
            enemyDistance = closestDistance;
        }
    }

    void HandleSpeed()
    {
        distanceToSpotToFollow = Vector3.Distance(transform.position,spotToFollow.transform.position);
        
        ChangeSpeed(planeToFollow.glideSpeed);
    }

    void ChangeSpeed(float desiredSpeed)
    {   
        if (plane.glideSpeed<desiredSpeed)
        {
            plane.Accelerate(1);
        }
        else if (plane.glideSpeed >desiredSpeed)
        {
            plane.Accelerate(-1);
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
