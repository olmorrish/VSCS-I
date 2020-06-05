using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalPrinter : MonoBehaviour
{
    Text boxText;
    List<string> printBuffer;           //
    public string currentlyPrinting;    //the string being printed currently

    private float nextPrintTime;
    private float printDelay;
    public float printDelayDefault = 3f; 

    private bool currentHasBeenScanned;

    public bool runNavDemoOnLaunch;

    private bool dontEndNextLine;       //a flag; tripped when set by a special command. Stops the  automatic newline character for the next line.

    // Start is called before the first frame update
    void Start() {
        boxText = GetComponent<Text>();
        printBuffer = new List<string>();
        currentlyPrinting = null;
        printDelay = printDelayDefault/60f;
        nextPrintTime = Time.time;
        dontEndNextLine = false;

        //TODO REMOVE
        if (runNavDemoOnLaunch) {
            RunDemo();
        }


        Debug.Log(TerminalIdle());
    }

    // Update is called once per frame
    void Update(){

        //if nothing is printing but the buffer has a string, begin printing it
        if (printBuffer.Count > 0 && currentlyPrinting == null) {
            currentlyPrinting = printBuffer[0];
            currentHasBeenScanned = false;
            printBuffer.RemoveAt(0);
        }

        //check for special commands in the current string
        if (currentlyPrinting !=null && !currentHasBeenScanned) {
            if (currentlyPrinting.Substring(0, 2) == "::") {
                string command = currentlyPrinting.Substring(2, 5);
                int arg = 0;
                if (currentlyPrinting.Length > 7)
                    arg = int.Parse(currentlyPrinting.Substring(8));

                switch (command) {
                    case "SPEED":
                        if (arg == 0)
                            printDelay = printDelayDefault/60f;
                        else
                            printDelay = arg / 60f;
                        break;
                    case "NONEW":
                        dontEndNextLine = true;
                        break;
                    default:
                        Debug.Log("Script detected unknown :: special command: \"" + command + "\"");
                        break;
                    

                }

                currentlyPrinting = null;   //we always discard a special command so it isn't printed
            }
            currentHasBeenScanned = true;
        }
        
        //print out a character on every non-skip frame
        if (currentlyPrinting != null && Time.time >= nextPrintTime) {
            if(currentlyPrinting.Length >= 2) {
                boxText.text = boxText.text + currentlyPrinting[0];
                currentlyPrinting = currentlyPrinting.Substring(1);  
            }
            //on the last character, print a newline char as well
            else {
                if (dontEndNextLine) {
                    boxText.text = boxText.text + currentlyPrinting[0];
                    dontEndNextLine = false;
                }
                else
                    boxText.text = boxText.text + currentlyPrinting[0] + "\n";
                currentlyPrinting = null;
            }

            nextPrintTime = Time.time + printDelay;
        }
    }


    /* Wipe
     * Halts any text currenly printing and clears the textbox.
     * Used when a new text document is opened to prevent clashes. 
     */
    public void Wipe() {
        currentlyPrinting = null;
        boxText.text = "";
    }

    /* Terminal Idle
     * Returns true only if the terminal is not currently printing anything and has no messages queued. 
     */
    public bool TerminalIdle() {
        return (printBuffer.Count == 0 && currentlyPrinting == null);
    }

    /* FeedLine
     * Takes a string to print to the game-terminal and queues it to print
     */
    public void FeedLine(string line) {
        printBuffer.Add(line);
    }

    /* FeedLines
     * Takes an array of strings to print to the game-terminal and queues them to print
     */
    public void FeedLines(string[] lines) {
        for (int i=0; i<lines.Length; i++) {
            FeedLine(lines[i]);
        }
    }

    private void RunDemo() {
        FeedLine("::SPEED 0");
        FeedLine("[        LOADING        ]");
        FeedLine("::SPEED 12");
        FeedLine("|||||||||||||||||||||||||");
        FeedLine("::SPEED 0");
        FeedLine("Loading complete.");

        FeedLine("> Welcome to the VSCS-I terminal.");
        FeedLine("> Enter a command below to begin, or type HELP for a list of commands.");
    }

    public void ShowNavIntro() {
        FeedLine("::SPEED 0");
        FeedLine("\n> This left segment is the NAVIGATION TERMINAL. It will display information about files, as well as error messages when appropriate.");
        FeedLine("> Enter a command below to begin, or type HELP for a list of commands.");
    }

    public void ShowFileIntro() {
        FeedLine("::SPEED 0");
        FeedLine("  The VSCS monitor utilizes a state - of - the - art DYNAMIC DUAL DISPLAY.");
        FeedLine("  The right segment of your display is the FILE VIEWER. It is of a higher resolution than the left, allowing you to read files on your machine. " +
            "When a text file is opened, it will be displayed here.");
        FeedLine("\n  The FILE VIEWER will allow you to enter passwords while reading their hints on the other side of the screen. Hopefully it should make puzzles more manageable.");
        FeedLine("  Currently, you can push data to either one of these displays with a \"FeedLine(str)\" call. Special commands can be used to slow down the speed of the text by setting the delay length.");
        FeedLine("  For instance, setting the speed to 60 will print a character every 60 frames: ");
        FeedLine("::SPEED 60");
        FeedLine("2398");
    }



}
