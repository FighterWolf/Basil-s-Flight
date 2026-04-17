using UnityEngine;

public class EssentialFunctions : MonoBehaviour
{
    public static Transform FindDescendants(Transform t, string name)
    {
        foreach(Transform child in t)
        {
            if (child.name == name)
            {
                return child;
            }
            else
            {
                if (FindDescendants(child, name)!=null)
                {
                    return FindDescendants(child, name);
                }
            }
        }
        return null;
    }

    public static void AimForTarget(Transform ownerTransform, Transform targetTransform, float rotationSpeed)
    {
        Vector3 desiredDirection = (targetTransform.position - ownerTransform.position).normalized;
        Vector3 newDirection = Vector3.RotateTowards(ownerTransform.forward,desiredDirection,rotationSpeed*Time.deltaTime,0f);
        ownerTransform.rotation = Quaternion.LookRotation(newDirection);
    }

    public static Vector3 TransformWorldCoordsToScreen(Vector3 objectPosition,Camera camera)
    {
        Vector3 screenCoords = camera.WorldToScreenPoint(objectPosition);

        RectTransform rt = GameObject.Find("Canvas").GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt,screenCoords,null,out Vector2 localPoint);

        return new Vector3(localPoint.x,localPoint.y,screenCoords.z);
    }

    public static void OnSuccessfulHit(Entity killer,Entity target, bool ignoreCooldown, float decreaseHealth, string weaponName)
    {
        if (!target.isDisabled)
        {
            target.DecreaseHealth(ignoreCooldown,decreaseHealth);

            if (target.health <= 0)
            {
                if(!(target is Projectile))
                {
                    Debug.Log(killer.killCreditName + " eliminated " + target.killCreditName + " using: " + weaponName);
                    if(killer.TryGetComponent<HandlePlayer>(out HandlePlayer player))
                    {
                        player.currentKillPoints++;
                    }
                }

                //reward killer

                //play sound for all, if player, display on screen.
            }
        }
    }

    public static Entity EntityInFront(Transform owner,float radius, float distance)
    {
        if (Physics.SphereCast(owner.position, radius, owner.forward, out RaycastHit hit, distance))
        {
            if (hit.collider.transform.root.TryGetComponent<Entity>(out Entity e))
            {
                return e;
            }
        }
        return null;
    }

    public static void HandleSound(AudioSource source,bool stopLoop)
    {
        if (!source.loop&&!stopLoop)
        {
            source.loop = true;
            source.Play();
        }else if (stopLoop)
        {
            source.loop = false;
            source.Stop();
        }
    }

    public static void HandleSound(AudioSource source, AudioClip clip,bool stopLoop)
    {
        source.clip = clip;
        if (!source.loop && !stopLoop)
        {
            source.loop = true;
            source.Play();
        }
        else if (stopLoop)
        {
            source.loop = false;
            source.Stop();
        }
    }

    public static void CreateSound(AudioClip clip, Vector3 position)
    {
        AudioSource source = new GameObject("Sound").AddComponent<AudioSource>();
        source.transform.position = position;
        source.clip = clip;
        source.spatialBlend = 1;
        source.maxDistance = 1500f;
        source.minDistance = 50f;
        source.Play();
        Destroy(source.gameObject,clip.length);
    }

    public static void CreateParticle(GameObject particle,Vector3 position)
    {
        GameObject explosionObj = Instantiate(particle, position, Quaternion.identity);
        explosionObj.transform.localScale = new Vector3(2, 2, 2);
    }

    public static void CreateExplosion(GameObject particle, AudioClip clip, Vector3 position)
    {
        CreateSound(clip,position);
        CreateParticle(particle,position);
    }

    public static void GameOver()
    {
        GameObject pauseMenu = FindDescendants(GameObject.Find("Canvas").transform,"PauseMenu").gameObject;
        pauseMenu.SetActive(false);
        foreach (Transform t in pauseMenu.transform.parent)
        {
            t.gameObject.SetActive(false);
        }
        PauseMenu.isGameOver = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static bool AllowPlayerMovement()
    {
        if(!PauseMenu.isPaused && !PauseMenu.isGameOver && !Dialogue.isInDialogue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
