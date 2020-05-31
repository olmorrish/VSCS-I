using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalPrinter : MonoBehaviour
{
    Text boxText;
    List<string> printBuffer;           //
    public string currentlyPrinting;    //the string being printed currently

    public GameObject[] fireworkObjs;
    public ParticleSystem[] fireworks;

    private float nextPrintTime;
    private float printDelay;
    public float printDelayDefault = 3f; 

    private bool currentHasBeenScanned;

    public bool runNavDemoOnLaunch;

    public bool fwGo;

    private bool dontEndNextLine;       //a flag; tripped when set by a special command. Stops the  automatic newline character for the next line.

    // Start is called before the first frame update
    void Start() {
        boxText = GetComponent<Text>();
        printBuffer = new List<string>();
        currentlyPrinting = null;
        printDelay = printDelayDefault/60f;
        nextPrintTime = Time.time;
        dontEndNextLine = false;

        fwGo = false;

        for (int i=0; i<fireworkObjs.Length; i++) {
            fireworks[i] = fireworkObjs[i].GetComponent<ParticleSystem>();
            fireworks[i].Stop();
        }

        //TODO REMOVE
        if (runNavDemoOnLaunch) {
            RunDemo();
        }


        Debug.Log(TerminalIdle());
    }

    // Update is called once per frame
    void Update(){

        if (fwGo) {
            fireworks[0].Play();
        }

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
                    case "LOADB":                       //the loading bars act as macros, queueing up multiple commands
                        FeedLine("::SPEED 0");
                        FeedLine("[      LOADING      ]");
                        FeedLine("::SPEED " + arg.ToString());
                        FeedLine("|||||||||||||||||||||");
                        FeedLine("::SPEED 0");
                        FeedLine("Loading complete.");
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
                if (dontEndNextLine)
                    boxText.text = boxText.text + currentlyPrinting[0];
                else 
                    boxText.text = boxText.text + currentlyPrinting[0] + "\n";
                currentlyPrinting = null;

                //re-enable the the don't-end-line flag
                if (dontEndNextLine)
                    dontEndNextLine = false;
            }

            nextPrintTime = Time.time + printDelay;
        }
    }


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
        FeedLine("::SPEED 1");
        FeedLine("[        LOADING SYSTEM        ]");
        FeedLine("::SPEED 8");
        FeedLine("||||||||||||||||||||||||||||||||");
        FeedLine("::SPEED 1");
        FeedLine("Loading complete.");

        FeedLine("\n> Welcome to the HAPPY BIRTHDAY terminal (Author: OLIVER MORRISH).");
        FeedLine("> To run the program, enter a name, age, and the number of times to say happy birthday: \n");

    }

    public void PrintHBDMessage(string name, string age, int numTimes) {

        FeedLine("::SPEED 0");

        for (int i = 0; i<(numTimes/2); i++) {
            int j = 2 * i + 1;
            int k = 2 * i + 2;

            string postJ;
            if (j % 10 == 1 && j != 11)
                postJ = "st";
            else if (j % 10 == 2 && j != 12)
                postJ = "nd";
            else if (j % 10 == 3 && j != 13)
                postJ = "rd";
            else
                postJ = "th";

            string postK;
            if (k % 10 == 1 && k != 11)
                postK = "st";
            else if (k % 10 == 2 && k != 12)
                postK = "nd";
            else if (k % 10 == 3 && k != 13)
                postK = "rd";
            else
                postK = "th";

            FeedLine("(#" + j + ") Happy " + j + postJ +" birthday, " + name + "! (#" + k +  ") Happy " + k + postK + " birthday, " + name + "!");

            if (j == 24 || k == 24) {
                FeedLine("You got married, then Diane was born!");
            }
            if (j == 26 || k == 26) {
                FeedLine("Paul was born!");
            }
            if (j == 29 || k == 29) {
                FeedLine("Tony was born! (He did the math and guessed your age for these funfacts).");
            }
            if (j == 47 || k == 47) {
                FeedLine("You became a grandad!");
            }
            if (j == 76 || k == 76) {
                FeedLine("You became a GREAT-grandad!");
            }
            if (j == 80 || k == 80) {
                FeedLine("(If any of the events or dates here were wrong, please take it up with Tony - I had to watch him do math and it was painful.)");
            }







            if (i > (numTimes / 2))
                fwGo = true;
        }


        FeedLine("                  *               *");
        FeedLine("  ~       *                *         ~    *");
        FeedLine("              *       ~        *              *   ~");
        FeedLine("               )       )         )       (             *");
        FeedLine("           *  (_) # ) (_) ) # ( (_) ( # (_)       *");
        FeedLine("              _#.-#(_)-#-(_)#(_)-#-(_)#-.#_");
        FeedLine("           :   #    #  #  #   #  #  #    #   :");
        FeedLine("        *  | `-.__                     __.-' | *");
        FeedLine("           |      `````\"\"\"\"\"\"\"\"\"\"\"`````      |         *");
        FeedLine("     *     |         | ||\\ |~)|~)\\ /         |");
        FeedLine("           |         |~||~\\|~ |~  |          |       ~");
        FeedLine("   ~   *   |                                 | *");
        FeedLine("           |      |~)||~)~|~| ||~\\|\\ \\ /     |         *");
        FeedLine("   *    _.-|      |~)||~\\ | |~|| /|~\\ |      |-._");
        FeedLine("      .'   '.      ~            ~           .'   `.  *");
        FeedLine("      :      `-.__                     __.-'      :");
        FeedLine("       `.         `````\"\"\"\"\"\"\"\"\"\"\"`````         .'");
        FeedLine("         `-.._                             _..-'");
        FeedLine("              ````\"\"\"\"-----------\"\"\"\"`````");

    }



}
