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

    List<KeyCode> LeftKeys = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Z, KeyCode.X, KeyCode.C,
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Tilde,
        KeyCode.Tab, KeyCode.LeftShift, KeyCode.CapsLock,
        KeyCode.LeftControl, KeyCode.Escape, KeyCode.LeftAlt};

    List<KeyCode> RightKeys = new List<KeyCode> { KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Plus,
        KeyCode.O, KeyCode.P, KeyCode.LeftCurlyBracket, KeyCode.RightCurlyBracket, KeyCode.Backslash,
        KeyCode.L, KeyCode.Semicolon, KeyCode.Quote, KeyCode.Return,
        KeyCode.Comma, KeyCode.Period, KeyCode.Slash, KeyCode.RightShift,
        KeyCode.RightAlt, KeyCode.LeftArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow };



    // Start is called before the first frame update
    void Start(){

        keyClick = keyClickObject.GetComponent<AudioSource>();
        spaceBarClick = spaceBarclickObj.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update(){

        if (Input.GetKeyDown(KeyCode.Space)) {
            spaceBarClick.panStereo = 0f;
            spaceBarClick.pitch = 1f + Random.Range(-0.1f, 0.1f);
            spaceBarClick.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace)) {
            spaceBarClick.panStereo = 0.5f; //because the backspace is on the right side of the keyboard lol
            spaceBarClick.pitch = 1f + Random.Range(-0.1f, 0.1f);
            spaceBarClick.Play();
        }

        else if (Input.anyKeyDown) {

            

            //default to zero; modify if in a keyCode set
            keyClick.panStereo = 0f;

            foreach (KeyCode keyCode in LeftKeys) {
                if (Input.GetKeyDown(keyCode)){
                    keyClick.panStereo = -0.5f;
                }
            }

            foreach (KeyCode keyCode in RightKeys) {
                if (Input.GetKeyDown(keyCode)) {
                    keyClick.panStereo = 0.5f;
                }
            }







            keyClick.pitch = 1f + Random.Range(-0.1f, 0.1f);
            keyClick.Play(); 
        }

    }
}
