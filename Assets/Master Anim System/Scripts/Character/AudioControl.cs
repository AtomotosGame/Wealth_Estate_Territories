using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour {
    Animator anim;
    [Header("Movement")]
    [Range(0,1)]
    public float MovementAudioVolume = 0.9f;
    public bool MovementSounds;
    public AudioClip TakeCoverSFX;
    public AudioClip SwitchSound;
    public AudioClip getupSFX;
    public AudioClip vaultSFX;
    public AudioClip climbSFX;
    public AudioClip fallSFX;
    public AudioClip ParachuteOpen;
    public AudioClip ParachuteResist;
    public AudioClip ParachuteFlap;

    [Header("Vehicle")]
    public AudioClip OpenVdoor;
    public AudioClip CloseVdoor;
    public AudioClip BreakVWindow;

    [Header("Weapons")]
    [Range(0, 1)]
    public float WeaponAudioVolume = 0.8f;
    public bool WeaponSounds;
    public AudioClip reshaftSFX;
    public AudioClip unloadSFX;
    public AudioClip reloadSFX;
    public AudioClip swishSFX;
    public AudioClip swashSFX;


    [Header("Breathe")]
    [Range(0, 1)]
    public float BreatheAudioVolume = 1f;
    public bool BreathingSounds;
    public AudioClip breathe1;
    public AudioClip breathe2;
    public AudioClip breathe3;


    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	}


    public void PlaySFX()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(TakeCoverSFX, transform.position, MovementAudioVolume);
        }
    }


    public void PlayParaOp()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(ParachuteOpen, transform.position, 1);
        }
    }
    public void PlayParaRes()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(ParachuteResist, transform.position, 1);
        }
    }
    public void PlayParaFlap()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(ParachuteFlap, transform.position, 1);
        }
    }


    public void PlayOpenVdoor()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(OpenVdoor, transform.position, MovementAudioVolume);
        }
    }

    public void PlayWindowVBreak()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(BreakVWindow, transform.position, MovementAudioVolume);
        }
    }

    public void PlayCloseVdoor()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(CloseVdoor, transform.position, MovementAudioVolume);
        }
    }

    public void SwitchSFX()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(SwitchSound, transform.position, MovementAudioVolume);
        }
    }
    public void SwishSFX()
    {
        if (WeaponSounds)
        {
            AudioSource.PlayClipAtPoint(swishSFX, transform.position, WeaponAudioVolume);
        }
    }
    public void SwashSFX()
    {
        if (WeaponSounds)
        {
            AudioSource.PlayClipAtPoint(swashSFX, transform.position, WeaponAudioVolume);
        }
    }


    public void ReshaftSFX()
    {
        if (WeaponSounds)
        {
            AudioSource.PlayClipAtPoint(reshaftSFX, transform.position, WeaponAudioVolume);
        }
    }
    public void UnloadSFX()
    {
        if (WeaponSounds)
        {
            AudioSource.PlayClipAtPoint(unloadSFX, transform.position, WeaponAudioVolume);
        }
    }
    public void ReloadSFX()
    {
        if (WeaponSounds)
        {
            AudioSource.PlayClipAtPoint(reloadSFX, transform.position, WeaponAudioVolume);
        }
    }
    public void GetUpSFX()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(getupSFX, transform.position, MovementAudioVolume);
        }
    }
    public void VaultSFX()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(vaultSFX, transform.position, MovementAudioVolume);
        }
    }
    public void ClimbSFX()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(climbSFX, transform.position, MovementAudioVolume);
        }
    }
    public void FallSFX()
    {
        if (MovementSounds)
        {
            AudioSource.PlayClipAtPoint(fallSFX, transform.position, MovementAudioVolume);
        }
    }
    public void BreathSFX1()
    {
        if (BreathingSounds)
        {
            AudioSource.PlayClipAtPoint(breathe1, transform.position, BreatheAudioVolume);
        }
    }
    public void BreathSFX2()
    {
        if (BreathingSounds)
        {
            AudioSource.PlayClipAtPoint(breathe2, transform.position, BreatheAudioVolume);
        }
    }
    public void BreathSFX3()
    {
        if (BreathingSounds)
        {
            AudioSource.PlayClipAtPoint(breathe3, transform.position, BreatheAudioVolume);
        }
    }
}
