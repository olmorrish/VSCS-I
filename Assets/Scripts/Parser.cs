using System;
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

    private string previousCommand;

    //cinematics
    public GameObject spookyMusicObject;
    private AudioSource spookyMusic;

    // Start is called before the first frame update
    void Start(){

        navTextPrinter = navTextObject.GetComponent<TerminalPrinter>();
        fileTextPrinter = fileTextObject.GetComponent<TerminalPrinter>();
        inputField = gameObject.GetComponent<InputField>();
        fileSystem = fileSystemObject.GetComponent<FileSystem>();

        previousCommand = null;

        //cinematics
        spookyMusic = spookyMusicObject.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update(){

        //ensure the field is always the focus
        inputField.Select();
        inputField.ActivateInputField();

        //ENTER KEY
        if (Input.GetKeyDown(KeyCode.Return)) {

            //every command is added to a memory of previous commands, then limit total memory to ten 
            previousCommand = inputField.text;

            //the parser will only process a command if the nav terminal is nto currently printing; pevents command spam
            if (navTextPrinter.TerminalIdle()) {
                Parse(inputField.text);
            }

            //input is deleted, whether parsed or not
            inputField.text = "";
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            inputField.text = previousCommand;
        }
        
    }

    public void Parse(string rawInput) {
        
        
        //SPECIAL CINEMATIC TRIGGERS
        if (rawInput.ToUpper().Equals("OPEN PLEASEHELP", StringComparison.CurrentCultureIgnoreCase)
            || rawInput.ToUpper().Equals("GOTO PLEASEHELP", StringComparison.CurrentCultureIgnoreCase) 
            || rawInput.ToUpper().Equals("CD PLEASEHELP", StringComparison.CurrentCultureIgnoreCase)){

                spookyMusic.Play();
        }
        
                
                
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

            /////////////////////////
            // NAVIGATION COMMANDS //
            /////////////////////////

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
                if (inputs.Length < 2) {
                    navTextPrinter.FeedLine("> GOTO requires an argument.");
                    navTextPrinter.FeedLine("> Format: GOTO X, where X is a file to open.");
                }
                else if (inputs.Length == 2) {    //player is attempting to open with no password
                    fileSystem.OpenRequest(inputs[1], null);
                }
                else {  //player must be attempting to open with password; second arg is passed in as pwd
                    fileSystem.OpenRequest(inputs[1], inputs[2]);
                }
                break;
            case "OPEN":
                if (inputs.Length < 2) {
                    navTextPrinter.FeedLine("> OPEN requires an argument.");
                    navTextPrinter.FeedLine("> Format: OPEN X, where X is a file to open.");
                }
                else if(inputs.Length == 2) {    //player is attempting to open with no password
                    fileSystem.OpenRequest(inputs[1], null); 
                }
                else {  //player must be attempting to open with password; second arg is passed in as pwd
                    fileSystem.OpenRequest(inputs[1], inputs[2]);
                }
                break;
            case "CD":
                if (inputs.Length < 2) {
                    navTextPrinter.FeedLine("> Command requires an argument.");
                }
                else if (inputs.Length == 2) {    //player is attempting to open with no password

                    if (inputs[1].Equals(".."))
                        fileSystem.BackRequest();
                    else
                        fileSystem.OpenRequest(inputs[1], null);
                }
                else {  //player must be attempting to open with password; second arg is passed in as pwd
                    fileSystem.OpenRequest(inputs[1], inputs[2]);
                }
                break;
            case "BACK":
                fileSystem.BackRequest();
                break;
            case "CD..":
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
        navTextPrinter.FeedLine("LIST     | Lists all files in the");
        navTextPrinter.FeedLine("         |  current directory.");
        navTextPrinter.FeedLine("OPEN X   | Opens directory/file \"X\".");
        navTextPrinter.FeedLine("OPEN X Y | Opens directory/file \"X\"");
        navTextPrinter.FeedLine("         |   using password \"Y\".");
        navTextPrinter.FeedLine("BACK     | Navigates one directory up.");
        navTextPrinter.FeedLine("LOST     | Displays your file location.");
    }
}
