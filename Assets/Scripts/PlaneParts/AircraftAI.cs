using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AircraftAI : Aircraft
{
    VFormationSpot spotToFollow;

    public string enemyTag;

    public bool isHit;
    [SerializeField] private Transform[] waypoints;
    private Transform currentWaypoint;
    public int waypointsIterator = 0;
    private float waypointDistanceThreshhold=10;
    private float distanceToSpotToFollow;

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
    public override void Start()
    {
        base.Start();
        pws = GetComponent<PlaneWeaponSystem>();
        pws.weaponSystem = PlaneWeaponSystem.WeaponSystem.Missile;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
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

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    State ChangeState()
    {
        if (IsDisabled())
        {
            return State.Disabled;
        }
        else if (IsTooCloseToGround(250))
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
        //EssentialFunctions.AimForTarget(transform, enemy.transform, 0.5f);
        ResetRotation();
        if(enemy)NavigateToTarget(enemy.transform);
        if (EntityInFront(10,500) is Entity e)
        {
            if (pws.MissileOperational() && enemy && pws.isReadyToBomb && enemyDistance > 150&& listOfPotentialEnemies.Contains(e))
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
        Debug.Log(killCreditName + " is firing gun");
        float timer = 0;
        float duration = 1f;

        while (timer < duration)
        {
            pws.FireGun();
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log(killCreditName + " is on cooldown");
        yield return new WaitForSeconds(1f);
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
            roll = 1;
        }
        else
        {
            roll = 0;
        }

        if (transform.position.x < -45)
        {
            pitch = 1;
        }
        else
        {
            pitch = 0;
        }
    }

    void ResetRotation()
    {
        pitch = 0;
        yaw = 0;
        roll = 0;
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
        if (transform.rotation.x > -25)
        {
            pitch=1;
        }
        else
        {
            pitch = 0;
        }
    }

    private IEnumerator Manuver()
    {
        pitch = 1;

        int random = Random.Range(0, 1);

        if (altitude > 25)
        {
            float whichTurn = 0;

            whichTurn = random > 0.5 ? 1 : -1;

            roll = whichTurn;
        }

        yield return new WaitForSeconds(1f);
        isHit = false;
        pitch = 0;
        roll = 0;
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
        ResetRotation();
        whichSpotToFollow = FindFirstEmptyFormationSpot();
        spotToFollow = whichSpotToFollow;
        if (spotToFollow != null)
        {
            float percentageOfWayToTarget = Mathf.InverseLerp(1, 1000, Vector3.Distance(transform.position, spotToFollow.transform.position));
            //EssentialFunctions.AimForTarget(transform, spotToFollow.transform, 1.5f);
            NavigateToTarget(spotToFollow.transform);
            HandleSpeed();
        }
    }

    void NavigateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 localPosition = transform.InverseTransformDirection(direction);

        //Debug.Log(localPosition);

        float threshold = 0.05f;

        pitch = Mathf.Clamp(localPosition.y, -1, 1);
        yaw = Mathf.Clamp(localPosition.x, -1, 1);
        if (Mathf.Abs(localPosition.x) > threshold)
        {
            roll = Mathf.Clamp(localPosition.x, -1, 1);
        }
        else
        {
            float bank = Vector3.Dot(transform.right, Vector3.up);
            roll = Mathf.Clamp(bank, -1, 1);
        }
    }

    void DisableSelf()
    {
        ResetRotation();
        speed = 0;
        if (transform.rotation.x < 10)
        {
            pitch = -1;
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
        if (glideSpeed<desiredSpeed)
        {
            Accelerate(1);
        }
        else if (glideSpeed >desiredSpeed)
        {
            Accelerate(-1);
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
                    formationPosition = Aircraft.FormationPosition.Left;
                    break;
                case VFormationSpot.LeftORRight.Right:
                    formationPosition = Aircraft.FormationPosition.Right;
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
                    v.whoTakesTheSpot = this;
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
                        v.whoTakesTheSpot = this;
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
