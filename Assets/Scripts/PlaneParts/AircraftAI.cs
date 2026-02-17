using UnityEngine;
using System.Collections;
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
    private int planeLayer;

    private List<Entity> listOfPotentialEnemies;
    [SerializeField] private Entity enemy;
    [SerializeField] private float enemyDistance;

    private Coroutine manuverCoroutine;
    private Coroutine gunBurstCoroutine;

    private PlaneWeaponSystem pws;

    public enum State
    {
        Patroling,
        Following,
        Attacking,
        Evading,
        AvoidingGround,
        AvoidingCollision,
        Disabled,
        Aimless
    }

    public State state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planeLayer = LayerMask.GetMask("Plane Parts");
        plane = GetComponent<Aircraft>();
        pws = GetComponent<PlaneWeaponSystem>();
        pws.weaponSystem = PlaneWeaponSystem.WeaponSystem.Missile;
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
                SearchForClosestEnemy();
                break;
            case State.Following:
                FollowAlliedAircraft();
                SearchForClosestEnemy();
                break;
            case State.Attacking:
                Pursue();
                ClearEnemyOnTakedown();
                break;
            case State.Evading:
                Evade();
                break;
            case State.AvoidingGround:
                AvoidGround();
                break;
            case State.AvoidingCollision:
                AvoidMidAirCollision();
                break;
            case State.Disabled:
                DisableSelf();
                break;
            case State.Aimless:
                ResetRotation();
                break;
        }
    }

    void FixedUpdate()
    {
        
    }
    State ChangeState()
    {
        if (plane.IsDisabled())
        {
            return State.Disabled;
        }
        else if (IsTooCloseToGround(150))
        {
            return State.AvoidingGround;
        }
        else if(IsTooCloseToAircraft())
        {
           return State.AvoidingCollision;
        }
        else if (isHit)
        {
            return State.Evading;
        }
        else if (manuverCoroutine == null && enemy && enemyDistance < 500)
        {
            return State.Attacking;
        }
        else if (planeToFollow && !planeToFollow.IsDisabled())
        {
            return State.Following;
        }
        else if (waypoints.Length > 0)
        {
            return State.Patroling;
        }

        return State.Aimless;
    }

    void Pursue()
    {
        EssentialFunctions.AimForTarget(transform, enemy.transform, 0.5f);
        ResetRotation();
        if (EntityInFront(10,500) is Entity e)
        {
            //Debug.Log(GetComponent<Entity>().killCreditName + " detected " + e.killCreditName);

            if (pws.MissileOperational() && enemy && pws.isReadyToBomb && enemyDistance > 200)
            {
                pws.FireMissile(e);
            }
            else if (pws.MachineGunOperational() && e != GetComponent<Entity>() && listOfPotentialEnemies.Contains(e))
            {
                if (gunBurstCoroutine == null) gunBurstCoroutine = StartCoroutine(GunBurst());
            }
        }
    }

    private IEnumerator GunBurst()
    {
        Debug.Log(GetComponent<Entity>().killCreditName + " is firing gun");
        float timer = 0;
        float duration = 0.5f;

        while (timer < duration)
        {
            pws.FireGun();
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log(GetComponent<Entity>().killCreditName + " is on cooldown");
        yield return new WaitForSeconds(3f);
        gunBurstCoroutine = null;
    }

    void ClearEnemyOnTakedown()
    {
        if (enemy.health <= 0)
        {
            enemy = null;
            enemyDistance = Mathf.Infinity;
        }
    }

    void Evade()
    {
        if (manuverCoroutine==null) manuverCoroutine = StartCoroutine(Manuver());
    }

    Entity EntityInFront(float radius,float distance)
    {
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, distance))
        {
            if (hit.collider.transform.root.TryGetComponent<Entity>(out Entity e))
            {
                return e;
            }
        }
        return null;
    }

    bool IsTooCloseToAircraft()
    {
        if (EntityInFront(50,100)) return true;
        return false;
    }

    void AvoidMidAirCollision()
    {
        if (transform.position.z < 67)
        {
            plane.roll = 1;
        }
        else
        {
            plane.roll = 0;
        }

        if (transform.position.x < -45)
        {
            plane.pitch = 1;
        }
        else
        {
            plane.pitch = 0;
        }
    }

    void ResetRotation()
    {
        plane.pitch = 0;
        plane.yaw = 0;
        plane.roll = 0;
    }

    bool IsTooCloseToGround(float threshold)
    {
        Vector3 diagonal = (transform.forward + (Vector3.down * 0.5f)).normalized;

        if (Physics.Raycast(transform.position, diagonal, out RaycastHit hit, Mathf.Infinity, ~planeLayer))
        {
            //Debug.Log(hit.distance);
            if (hit.distance < threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void AvoidGround()
    {
        if (plane.transform.rotation.x > -25)
        {
            plane.pitch=1;
        }
        else
        {
            plane.pitch = 0;
        }
    }

    private IEnumerator Manuver()
    {
        plane.pitch = 1;

        int random = Random.Range(0, 1);

        if (plane.altitude > 25)
        {
            float whichTurn = 0;

            whichTurn = random > 0.5 ? 1 : -1;

            plane.roll = whichTurn;
        }

        yield return new WaitForSeconds(1f);
        isHit = false;
        plane.pitch = 0;
        plane.roll = 0;
        manuverCoroutine = null;
    }

    void Patrol()
    {
        ResetRotation();
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
    
    void FollowAlliedAircraft()
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
        if (transform.rotation.x < 10)
        {
            plane.pitch = -1;
        }
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

            foreach (Entity e in listOfPotentialEnemies)
            {
                if (!e || e.health <= 0) continue;
                
                float distance = Vector3.Distance(e.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = e;
                }
            }
            enemyDistance = closestDistance;
            enemy = closestEnemy;
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
