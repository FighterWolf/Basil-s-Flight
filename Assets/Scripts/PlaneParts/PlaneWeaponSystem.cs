using UnityEngine;
using StarterAssets;
using System.Collections;

public class PlaneWeaponSystem : MonoBehaviour
{
    public float gunDamage;
    public float missileDamage;

    public float gunFireRate;
    public float missileReloadRate;

    public float flareReloadRate;

    public bool isReadyToFireGun=true;
    public bool isReadyToBomb=true;
    public bool isFlareReady = true;

    public enum WeaponSystem {
        Gun,
        Missile
    }

    private int weaponSystemSize;
    private int weaponSystemIterator = 0;
    public WeaponSystem weaponSystem;

    private Aircraft plane;
    private Camera planeCam;
    private ThirdPersonController player;
    private AircraftControls pilotInput;

    public GameObject gunPod;
    public Bullet bullet;

    public GameObject missilePod;
    public Missile missile;

    public GameObject flareDeployer;
    public Flare flareObject;

    public bool fire;
    public bool switchWeapon;
    public bool flare;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<Aircraft>();
        planeCam = EssentialFunctions.FindDescendants(transform,"Camera").GetComponent<Camera>();
        gunPod = EssentialFunctions.FindDescendants(transform, "GunPod").gameObject;
        missilePod = EssentialFunctions.FindDescendants(transform, "MissileLauncher").gameObject;
        flareDeployer = EssentialFunctions.FindDescendants(transform, "FlareLauncher").gameObject;
        weaponSystemSize = System.Enum.GetNames(typeof(WeaponSystem)).Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (pilotInput != null)
        {
            fire = pilotInput.fire;
            switchWeapon = pilotInput.switchWeapon;
            flare = pilotInput.flare;
        }

        if (!plane.IsDisabled())
        {
            Fire();
            SwitchWeapon();
        }
    }

    public bool MissileOperational()
    {
        if (missilePod && missile) return true;
        return false;
    }

    public bool MachineGunOperational()
    {
        if (gunPod && bullet) return true;
        return false;
    }

    public void Fire(Entity target = null)
    {
        if (fire)
        {
            switch (weaponSystem)
            {
                case WeaponSystem.Gun:
                    if(isReadyToFireGun /*&& !pilotInput.allowLook*/) FireGun();
                    break;
                case WeaponSystem.Missile:
                    if(isReadyToBomb) FireMissile();
                    break;
            }
        }

        if (flare && isFlareReady)
        {
            DeployFlare();
        }
    }

    public void FireGun()
    {
        //Debug.Log(GetComponent<Entity>().killCreditName + ": Firing Gun");
        GameObject shot = Instantiate(this.bullet.gameObject, gunPod.transform.position, transform.rotation);
        shot.GetComponent<Bullet>().owner=plane;
        shot.GetComponent<Bullet>().speed = plane.speed;
        shot.GetComponent<Bullet>().gunDamage = gunDamage;

        isReadyToFireGun = false;
        StartCoroutine(ResetGunShot());
    }

    public void FireMissile(Entity target = null)
    {
        //Debug.Log(plane.name + ": Firing Missile");
        GameObject missile = Instantiate(this.missile.gameObject, missilePod.transform.position, transform.rotation, missilePod.transform);
        if (TryGetComponent<PlaneHUD>(out PlaneHUD pHUD))
        {
            if(pHUD.confirmedTarget != null) missile.GetComponent<Missile>().targetToStrike = pHUD.confirmedTarget.transform;
        }
        else if (target)
        {
            missile.GetComponent<Missile>().targetToStrike = target.transform;
        }
        missile.GetComponent<Missile>().owner = plane;
        missile.GetComponent<Missile>().speedModifier = plane.speed;
        missile.GetComponent<Missile>().missileDamage = missileDamage;
        missile.tag = plane.tag;
        isReadyToBomb = false;
        if(pilotInput) pilotInput.fire = false;
        fire = false;
        StartCoroutine(ResetMissileShot());
    }

    public void DeployFlare()
    {
        GameObject flare = Instantiate(this.flareObject.gameObject, flareDeployer.transform.position, flareDeployer.transform.rotation);
        flare.GetComponent<Flare>().owner=plane;
        flare.tag = plane.tag;
        plane.deployedFlares.Add(flare.GetComponent<Flare>());

        isFlareReady = false;
        if (pilotInput) pilotInput.flare = false;
        StartCoroutine(ResetFlare());
    }

    public IEnumerator ResetGunShot()
    {
        yield return new WaitForSeconds(60 / gunFireRate);
        isReadyToFireGun = true;
    }

    public IEnumerator ResetMissileShot()
    {
        yield return new WaitForSeconds(60 / missileReloadRate);
        if(pilotInput) yield return new WaitUntil(() => !pilotInput.fire);
        isReadyToBomb =true;
    }

    public IEnumerator ResetFlare()
    {
        yield return new WaitForSeconds(60 / flareReloadRate);
        if (pilotInput) yield return new WaitUntil(() => !pilotInput.flare);
        isFlareReady = true;
    }
    public void SwitchWeapon()
    {
        if (switchWeapon)
        {
            if (weaponSystemIterator+1== weaponSystemSize)
            {
                weaponSystemIterator = 0;
            }
            else
            {
                weaponSystemIterator++;
            }

            weaponSystem = (WeaponSystem)weaponSystemIterator;
            pilotInput.switchWeapon = false;
           
        }
    }

    public void SetPlayer(ThirdPersonController player = null)
    {
        this.player = player;
        if (player != null)
        {
            pilotInput = player.GetComponent<AircraftControls>();
        }
        else
        {
            pilotInput = null;
        }
    }
}
