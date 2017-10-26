using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;

    public static SoundManager GetInstance()
    {
        if (instance == null)
        {
            var k = GameObject.Find("ManagerObject").AddComponent<SoundManager>();
            instance = k;

        }
        return instance;
    }

    public AudioClip ShootSound;
    public AudioClip ConfirmSound;
    public AudioClip ObjectDestroyedSound;
    public AudioClip GameOver;

    private AudioSource source;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            source = GetComponent<AudioSource>();
            Debug.Log("SoundManager initialized");
        }
    }

    // For the following methods, load the default sounds if there is no audio supplied.
    public void PlayShootSound()
    {
        if(ShootSound != null)
            source.PlayOneShot(ShootSound);
        else
            source.PlayOneShot(Resources.Load<AudioClip>("Audio/Laser_blast"));
    }

    public void PlayConfirmSound()
    {
        return;
        source.PlayOneShot(ConfirmSound);
    }

    public void PlayObjectDestroyedSound()
    {
        return;
        source.PlayOneShot(ObjectDestroyedSound);
    }

    public void PlayGameOverSound()
    {
        if(GameOver != null)
            source.PlayOneShot(GameOver);
        else
            source.PlayOneShot(Resources.Load<AudioClip>("Audio/Game_Over"));
    }
}
