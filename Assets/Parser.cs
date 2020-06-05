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
            case "LIST":
                string[] list = fileSystem.GetChildList();
                if(list.Length == 0) {
                    navTextPrinter.FeedLine("> No files could be found.");
                }
                else {
                    navTextPrinter.FeedLine("> -- Contents -- ");
                    foreach (string item in list) {
                        navTextPrinter.FeedLine("> " + item);
                    }
                }
                break;
            case "LS":
                string[] LSlist = fileSystem.GetChildList();
                if (LSlist.Length == 0) {
                    navTextPrinter.FeedLine("> No files could be found.");
                }
                else {
                    navTextPrinter.FeedLine("> -- Contents -- ");
                    foreach (string item in LSlist) {
                        navTextPrinter.FeedLine("> " + item);
                    }
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
            case "BACK":
                fileSystem.BackRequest();
                break;
            case "LOST":
                string location = fileSystem.GetCurrentFQN();
                navTextPrinter.FeedLine("> Your current location in the file system is: " + location.ToUpper() + ".");
                break;

            /////////////////////////
            // EASTER EGG COMMANDS //
            /////////////////////////
            case "META":
                navTextPrinter.FeedLine("> M E T A");
                navTextPrinter.FeedLine("> E");
                navTextPrinter.FeedLine("> T");
                navTextPrinter.FeedLine("> A");
                break;
            case "1337":
                navTextPrinter.FeedLine("> flash2:scroll: Buying GF.");
                break;
            case "6969":
                navTextPrinter.FeedLine("> Dude, nice. That's the sex number.");
                break;
            case "NICE":
                navTextPrinter.FeedLine("> Agreed.");
                break;
            case "NEAT":
                navTextPrinter.FeedLine("> Sure is.");
                break;
            case "COOL":
                navTextPrinter.FeedLine("> Very cool.");
                break;
            case "OKAY":
                navTextPrinter.FeedLine("> Okay.");
                break;
            case "DOPE":
                navTextPrinter.FeedLine("> I know.");
                break;
            case "DUDE":
                navTextPrinter.FeedLine("> Don't you \"dude\" me, dude.");
                break;
            case "BRUH":
                navTextPrinter.FeedLine("> B ~ R ~ U ~ H.");
                break;
            case "N00B":
                navTextPrinter.FeedLine("> Pretty rude but okay.");
                break;
            case "SRRY":
                navTextPrinter.FeedLine("> It's alright.");
                break;
            case "PWND":
                navTextPrinter.FeedLine("> Yeah I'm sure I will be. I'm so scared.");
                break;
            case "ZOOP":
                navTextPrinter.FeedLine("> *fingerguns*");
                break;
            case "L055":
                navTextPrinter.FeedLine("\n>         | t    ");
                navTextPrinter.FeedLine(">     I   | h  i ");
                navTextPrinter.FeedLine(">     s   |    s "); ;
                navTextPrinter.FeedLine(">  -------+-------");
                navTextPrinter.FeedLine(">    L  s | .    ");
                navTextPrinter.FeedLine(">    o  s | t    ");
                navTextPrinter.FeedLine(">         |  xt? ");
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
        navTextPrinter.FeedLine("LIST     | Lists all files in the");
        navTextPrinter.FeedLine("         |  current directory.");
        navTextPrinter.FeedLine("OPEN X   | Opens directory/file \"X\".");
        navTextPrinter.FeedLine("BACK     | Navigates one directory up.");
        navTextPrinter.FeedLine("---------|----------------------------");
        navTextPrinter.FeedLine("- The commands below don't work (yet!)");
        navTextPrinter.FeedLine("---------|----------------------------");
        navTextPrinter.FeedLine("ULOK X P | Attempts to use password ");
        navTextPrinter.FeedLine("         |  \"P\" to unlock file \"X\".");
    }
}
