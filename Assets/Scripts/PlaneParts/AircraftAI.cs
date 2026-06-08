using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AircraftAI : Aircraft
{
    //VFormationSpot spotToFollow;

    public string enemyTag;

    public bool isHit;
    private bool awaitingMissileClearance;
    //private float distanceToSpotToFollow;

    private List<Entity> listOfPotentialEnemies;
    [SerializeField] private Entity enemy;
    [SerializeField] private float enemyDistance;

    private Coroutine manuverCoroutine;
    private Coroutine gunBurstCoroutine;
    private Coroutine missileClearanceCoroutine;

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

        if(planeToFollow)
        {
            glideSpeed = planeToFollow.speed>maxSpeed ? maxSpeed-10 : planeToFollow.glideSpeed;
            speed = planeToFollow.speed > maxSpeed ? maxSpeed - 10 : planeToFollow.glideSpeed;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        state = ChangeState();

        if (missilesHeadingTowardsSelf.Count > 0 && missileAwareness>0.75f && pws.isFlareReady)
        {
            pws.DeployFlare();
        }

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
        else if (IsTooCloseToGround(actualSpeed*5))
        {
            return State.AvoidingGround;
        }
        else if(IsTooCloseToAircraft())
        {
           return State.AvoidingCollision;
        }
        else if (isHit||IsBeingLockedOn())
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
        ResetRotation();
        if(enemy)NavigateToTarget(enemy.transform);
        if (EssentialFunctions.EntityInFront(transform, 10,500) is Entity e)
        {
            if (pws.MissileOperational() && enemy && pws.isReadyToBomb && enemyDistance > 150&& listOfPotentialEnemies.Contains(e))
            {
                pws.FireMissile(e);
                if(missileClearanceCoroutine!=null) missileClearanceCoroutine = StartCoroutine(WaitForMissileClearance());
            }
            else if (pws.MachineGunOperational() && e != GetComponent<Entity>() && listOfPotentialEnemies.Contains(e) && !awaitingMissileClearance)
            {
                if (gunBurstCoroutine == null) gunBurstCoroutine = StartCoroutine(GunBurst());
            }
        }
    }

    private IEnumerator GunBurst()
    {
        float timer = 0;
        float duration = 2f;

        while (timer < duration)
        {
            pws.FireGun();
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        gunBurstCoroutine = null;
    }

    private IEnumerator WaitForMissileClearance()
    {
        awaitingMissileClearance = true;
        yield return new WaitForSeconds(1f);
        awaitingMissileClearance = false;
        missileClearanceCoroutine = null;
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

    bool IsTooCloseToAircraft()
    {
        if (EssentialFunctions.EntityInFront(transform,50,75)) return true;
        return false;
    }

    void AvoidMidAirCollision()
    {
        if (transform.position.z < 45)
        {
            roll = 1;
        }
        else
        {
            roll = 0;
        }

        if (transform.position.x < -33)
        {
            pitch = 1;
        }
        else
        {
            pitch = 0;
        }
    }

    bool IsTooCloseToGround(float threshold)
    {
        Vector3 diagonal = (transform.forward + (Vector3.down * 0.5f)).normalized;

        if (Physics.Raycast(transform.position, diagonal, out RaycastHit hit, Mathf.Infinity, ~planeLayer))
        {
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
        float pitchAngle = transform.eulerAngles.x;

        if (pitchAngle > 180)
        {
            pitchAngle -= 360;
        }

        if (pitchAngle > -45)
        {
            pitch=1;
            throttle = 1;
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
    
    void FollowAlliedAircraft()
    {
        ResetRotation();

        if (planeToFollow != null)
        {
            NavigateToTarget(planeToFollow.transform);
            ChangeSpeed(planeToFollow.actualSpeed);
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
                if (!e || e.GetComponent<Projectile>() != null || e.health <= 0) continue;
                
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

    void ChangeSpeed(float desiredSpeed)
    {   
        if (actualSpeed<desiredSpeed)
        {
            throttle=1;
        }
        else if (actualSpeed > desiredSpeed)
        {
            throttle = -1;
        }
    }
}
