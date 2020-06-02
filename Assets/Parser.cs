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

    public GameObject fileSystemObject;
    private FileSystem fileSystem;

    private InputField inputField;

    // Start is called before the first frame update
    void Start(){

        navTextPrinter = navTextObject.GetComponent<TerminalPrinter>();
        fileTextPrinter = fileTextObject.GetComponent<TerminalPrinter>();
        inputField = gameObject.GetComponent<InputField>();
        fileSystem = fileSystemObject.GetComponent<FileSystem>();

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
        string command = inputs[0].ToUpper();

        switch (command) {
            case "HELP":
                PrintHelpMsg();
                break;
            case "DEMO":
                navTextPrinter.ShowNavIntro();
                fileTextPrinter.ShowFileIntro();
                break;
            case "HELLO":
                navTextPrinter.FeedLine("> Hello world!");
                break;
            case "LIST":
                string[] list = fileSystem.getChildList();
                foreach (string item in list) {
                    navTextPrinter.FeedLine("> " + item);
                }
                if(list.Length == 0) {
                    navTextPrinter.FeedLine("> No files could be found.");
                }
                break;
            case "GOTO":
                if (inputs.Length != 2) {
                    navTextPrinter.FeedLine("> GOTO requires a single argument to function.");
                    navTextPrinter.FeedLine("> Format: GOTO X, where X is your destination.");
                }
                else
                    fileSystem.OpenRequest(inputs[1]);
                break;
            case "OPEN":
                if (inputs.Length != 2) {
                    navTextPrinter.FeedLine("> OPEN requires a single argument to function.");
                    navTextPrinter.FeedLine("> Format: OPEN X, where X is the file to open.");
                }
                else
                    fileSystem.OpenRequest(inputs[1]);
                break;
            default:
                navTextPrinter.FeedLine("> Did not recognize command \"" + command + "\".");
                navTextPrinter.FeedLine("> Type HELP for a list of commands.");
                break;
        }

    }

    private void PrintHelpMsg() {
        navTextPrinter.FeedLine("\n-=- HELP MENU -=- ");
        navTextPrinter.FeedLine("Command  | Usage");
        navTextPrinter.FeedLine("---------|----------------------------");
        navTextPrinter.FeedLine("HELP     | Displays this menu.");
        navTextPrinter.FeedLine("DEMO     | Run a VSCS demo.");
        navTextPrinter.FeedLine("---------|----------------------------");
        navTextPrinter.FeedLine("- The commands below don't work (yet!)");
        navTextPrinter.FeedLine("---------|----------------------------");
        navTextPrinter.FeedLine("LIST     | Lists all files in the");
        navTextPrinter.FeedLine("         |  current directory.");
        navTextPrinter.FeedLine("OPEN X   | Opens directory/file \"X\".");
        navTextPrinter.FeedLine("BACK     | Navigates one directory up.");
        navTextPrinter.FeedLine("ULOK X P | Attempts to use password ");
        navTextPrinter.FeedLine("         |  \"P\" to unlock file \"X\".");
    }
}
