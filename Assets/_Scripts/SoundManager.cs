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
    public AudioClip StageOver;
    public AudioClip StageVictory;

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

    /* For the following methods, load the default sounds if there is no audio supplied. */
    
    // Player's bullet shooting SFX
    public void PlayShootSound()
    {
        if(ShootSound != null)
            source.PlayOneShot(ShootSound);
        else
            source.PlayOneShot(Resources.Load<AudioClip>("Audio/Laser_blast"));
    }

    // Teleporting between scenes SFX
    public void PlayConfirmSound()
    {
        if(ShootSound != null)
            source.PlayOneShot(ConfirmSound);
        else
            source.PlayOneShot(Resources.Load<AudioClip>("Audio/Level_Change"));
    }

    // Enemy death SFX
    public void PlayObjectDestroyedSound()
    {
        if(ObjectDestroyedSound != null)
            source.PlayOneShot(ObjectDestroyedSound);
        else
            source.PlayOneShot(Resources.Load<AudioClip>("Audio/Enemy_Death"));
    }

    // Stage failure SFX
    public void PlayStageOverSound()
    {
        if(StageOver != null)
            source.PlayOneShot(StageOver);
        else
            source.PlayOneShot(Resources.Load<AudioClip>("Audio/Stage_Over"));
    }

    // Stage success SFX
    public void PlayStageWonSound()
    {
        if(StageVictory != null)
            source.PlayOneShot(StageVictory);
        else
            source.PlayOneShot(Resources.Load<AudioClip>("Audio/Stage_Victory"));
    }
}
