using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parser : MonoBehaviour
{
    public GameObject navTextObject;
    public GameObject fileTextObject;
    private TerminalPrinter navTextPrinter;
    private TerminalPrinter fileTextPrinter;

    private InputField inputField;

    // Start is called before the first frame update
    void Start(){

        navTextPrinter = navTextObject.GetComponent<TerminalPrinter>();
        fileTextPrinter = fileTextObject.GetComponent<TerminalPrinter>();
        inputField = gameObject.GetComponent<InputField>();

    }

    // Update is called once per frame
    void Update(){

        //ensure the field is always the focus
        inputField.Select();
        inputField.ActivateInputField();


        if (Input.GetKeyDown(KeyCode.Return)) {

            //the parser will only process a command if the nav terminal is nto currently printing; pevents command spam
            if (navTextPrinter.TerminalIdle()) {
                Parse(inputField.text);
            }

            //input is deleted, whether parsed or not
            inputField.text = "";
        }

    }

    public void Parse(string rawInput) {

        string[] inputs = rawInput.Split(' ');
        string name = inputs[0];
        string age = inputs[1];
        int numTimes = int.Parse(inputs[1]);

        navTextPrinter.PrintHBDMessage(name, age, numTimes);

    }
}
