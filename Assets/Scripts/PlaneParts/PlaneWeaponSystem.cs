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
    private PlaneHUD pHUD;

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

        if (TryGetComponent<PlaneHUD>(out PlaneHUD pHUD)) this.pHUD = pHUD;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isGameOver)
        {
            pilotInput = null;
        }
        
        if (pilotInput != null)
        {
            fire = pilotInput.fire;
            switchWeapon = pilotInput.switchWeapon;
            flare = pilotInput.flare;
        }

        if (!PauseMenu.isPaused&&!plane.IsDisabled())
        {
            Fire();
            SwitchWeapon();
        }

        EssentialFunctions.HandleSound(gunPod.GetComponent<AudioSource>(), bullet.bulletSound, (!fire || weaponSystem!=WeaponSystem.Gun) || PauseMenu.isPaused);
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
                    if (isReadyToFireGun /*&& !pilotInput.allowLook*/) FireGun();
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
        GameObject o = Instantiate(this.bullet.gameObject, gunPod.transform.position, transform.rotation);
        Bullet bullet = o.GetComponent<Bullet>();
        bullet.gunDamage = gunDamage;
        bullet.owner = plane;
        bullet.speed = plane.speed;
        isReadyToFireGun = false;
        StartCoroutine(ResetGunShot());
    }

    public void FireMissile(Entity target = null)
    {
        //Debug.Log(plane.name + ": Firing Missile");
        GameObject o = Instantiate(this.missile.gameObject, missilePod.transform.position, transform.rotation, missilePod.transform);
        Missile missile = o.GetComponent<Missile>();
        if (pHUD)
        {
            if(pHUD.confirmedTarget != null) missile.targetToStrike = pHUD.confirmedTarget.transform;
        }
        else if (target)
        {
            missile.targetToStrike = target.transform;
        }
        AudioSource.PlayClipAtPoint(missile.missileSound, missilePod.transform.position);
        missile.AssignStats(missileDamage, plane, missile.speed, plane.speed);
        missile.tag = plane.tag;
        isReadyToBomb = false;
        if(pilotInput) pilotInput.fire = false;
        fire = false;
        StartCoroutine(ResetMissileShot());
    }

    public void DeployFlare()
    {
        GameObject flare = Instantiate(this.flareObject.gameObject, flareDeployer.transform.position, flareDeployer.transform.rotation);
        flare.GetComponent<Flare>().AssignStats(0,plane);
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
        if (pHUD) pHUD.missileCooldown = 0;
        yield return new WaitForSeconds(missileReloadRate);
        if(pilotInput) yield return new WaitUntil(() => !pilotInput.fire);
        isReadyToBomb =true;
    }

    public IEnumerator ResetFlare()
    {
        if (pHUD) pHUD.flareCooldown = 0;
        yield return new WaitForSeconds(flareReloadRate);
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
