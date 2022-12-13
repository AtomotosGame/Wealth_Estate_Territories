using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV_Display : MonoBehaviour
{
    Renderer r;
    UnityEngine.Video.VideoClip videoClip;
    AudioSource AD_s;
    public bool TV_On;

    // Use this for initialization
    void Start()
    {
        AD_s = GetComponent<AudioSource>();
        r = GetComponent<Renderer>();
        // movie = (MovieTexture)r.material.mainTexture;
        var videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.playOnAwake = false;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.targetMaterialProperty = "_MainTex";
        videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, AD_s);

    }

    // Update is called once per frame
    void Update()
    {

        if (TV_On)
        {
            r.enabled = true;
            var vp = GetComponent<UnityEngine.Video.VideoPlayer>();
            if (!vp.isPlaying && !AD_s.isPlaying)
            {
                vp.Play();
                // vp.loop = true;
                AD_s.Play();
            }
        }
        else
        {
            r.enabled = false;
            var vp = GetComponent<UnityEngine.Video.VideoPlayer>();
            if (vp.isPlaying && AD_s.isPlaying)
            {
                vp.Stop();
                AD_s.Stop();
            }
        }
    }
}
