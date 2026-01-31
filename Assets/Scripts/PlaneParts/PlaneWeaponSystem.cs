using UnityEngine;
using StarterAssets;
using System.Collections;

public class PlaneWeaponSystem : MonoBehaviour
{
    public float gunDamage;
    public float missileDamage;

    public float gunFireRate;
    public float missileReloadRate;

    private bool isReadyToFireGun=true;
    private bool isReadyToBomb=true;

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

    public GameObject missilePod;
    public Missile missile;

    public bool fire;
    public bool switchWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<Aircraft>();
        planeCam = EssentialFunctions.FindDescendants(transform,"Camera").GetComponent<Camera>();
        weaponSystemSize = System.Enum.GetNames(typeof(WeaponSystem)).Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (pilotInput != null)
        {
            fire = pilotInput.fire;
            switchWeapon = pilotInput.switchWeapon;
        }

        Fire();
        SwitchWeapon();
    }

    public void Fire()
    {
        if (fire)
        {
            switch (weaponSystem)
            {
                case WeaponSystem.Gun:
                    if(isReadyToFireGun && !pilotInput.allowLook) FireGun();
                    break;
                case WeaponSystem.Missile:
                    if(isReadyToBomb) FireMissile();
                    break;
            }
        }

        void FireGun()
        {
            Debug.Log("Firing Gun");
            Ray r = new Ray(planeCam.transform.position, planeCam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(r,out hit,Mathf.Infinity))
            {
                if(hit.collider.TryGetComponent<Entity>(out Entity hitTarget))
                {
                    Debug.Log("Hit Target: " + hitTarget + "!");
                    hitTarget.DecreaseHealth(gunDamage);
                    if(hit.collider.TryGetComponent<AircraftAI>(out AircraftAI planeAI))
                    {
                        planeAI.isHit = true;
                    }
                }
            }
            isReadyToFireGun = false;
            StartCoroutine(ResetGunShot());
        }

        void FireMissile()
        {
            Debug.Log("Firing Missile");
            GameObject missile = Instantiate(this.missile.gameObject,missilePod.transform.position,transform.rotation,missilePod.transform);
            if(GetComponent<PlaneHUD>().confirmedTarget != null) missile.GetComponent<Missile>().targetToStrike = GetComponent<PlaneHUD>().confirmedTarget.transform;
            missile.GetComponent<Missile>().speedModifier = plane.speed;
            isReadyToBomb = false;
            pilotInput.fire = false;
            StartCoroutine(ResetMissileShot());
        }
    }

    public IEnumerator ResetGunShot()
    {
        yield return new WaitForSeconds(60 / gunFireRate);
        isReadyToFireGun = true;
    }

    public IEnumerator ResetMissileShot()
    {
        yield return new WaitForSeconds(60 / missileReloadRate);
        yield return new WaitUntil(() => !pilotInput.fire);
        isReadyToBomb=true;
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
