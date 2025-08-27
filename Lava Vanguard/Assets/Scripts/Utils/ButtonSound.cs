using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{


    public AudioClip clickSound;
    public AudioClip refreshSound;
    public AudioClip purchaseSound;
    public AudioClip bossApproachSound;
    private AudioSource audioSource;

    public bool mute = true;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

    }
    public void PlayClickSound()
    {
        if (mute || clickSound == null) return;
        audioSource.PlayOneShot(clickSound);
    }

    public void PlayRefreshSound()
    {
        if (mute || refreshSound != null) return;
            audioSource.PlayOneShot(refreshSound);
    }

    public void PlayPurchaseSound()
    {
        if (mute || purchaseSound != null) return;
            audioSource.PlayOneShot(purchaseSound);
    }

    public void PlayBossApproachSound()
    {
        if (mute || bossApproachSound != null) return;
            audioSource.PlayOneShot(bossApproachSound);


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
