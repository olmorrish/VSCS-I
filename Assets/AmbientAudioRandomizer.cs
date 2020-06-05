using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAudioRandomizer : MonoBehaviour
{
    public GameObject driveSoundObj;
    private AudioSource driveSound;

    // Start is called before the first frame update
    void Start()
    {
        driveSound = driveSoundObj.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        
        if(Time.time % 3 <= 0.05f) {
            driveSound.pitch = 1f + (Random.Range(0f, 0.07f));
            driveSound.volume = driveSound.pitch / 12f;
        }

    }
}
