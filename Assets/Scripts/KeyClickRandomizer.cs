using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class KeyClickRandomizer : MonoBehaviour
{
    public GameObject keyClickObject;
    private AudioSource keyClick;

    public GameObject spaceBarclickObj;
    private AudioSource spaceBarClick;

    // Start is called before the first frame update
    void Start(){

        keyClick = keyClickObject.GetComponent<AudioSource>();
        spaceBarClick = spaceBarclickObj.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update(){

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Backspace)) {
            spaceBarClick.pitch = 1f + Random.Range(-0.1f, 0.1f);
            spaceBarClick.Play();
        }
        else if (Input.anyKeyDown) {
            keyClick.pitch = 1f + Random.Range(-0.1f, 0.1f);
            keyClick.Play(); 
        }

    }
}
