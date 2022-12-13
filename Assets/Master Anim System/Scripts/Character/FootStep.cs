using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour {
    public ParticleSystem dustEffect;
    public AudioClip ImpactSFX;
    [Range (0,1)]
    public float AudioLevel = 1;
	// Use this for initialization
	void Update () {
	}

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            dustEffect.Emit(1);
            AudioSource.PlayClipAtPoint(ImpactSFX, transform.position, AudioLevel);
        }
    }
}
