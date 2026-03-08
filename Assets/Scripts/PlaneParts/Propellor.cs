using UnityEngine;

public class Propellor : MonoBehaviour
{
    private Aircraft plane;
    public AudioClip propellorSound;
    private AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = transform.root.GetComponent<Aircraft>();
        source = GetComponent<AudioSource>();
        source.clip = propellorSound;
    }

    // Update is called once per frame
    void Update()
    {
        if (plane.speed > 0)
        {
            EssentialFunctions.HandleSound(source, false);
            transform.Rotate(Vector3.up * plane.speed * 20 * Time.deltaTime);
        }

        EssentialFunctions.HandleSound(source, (PauseMenu.isPaused || plane.speed==0));
    }
}
