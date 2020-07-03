using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalPrinter : MonoBehaviour
{
    Text boxText;
    List<string> printBuffer;           //buffer for text to print in sequence
    public string currentlyPrinting;    //the string being printed currently

    private float nextPrintTime;
    private float printDelay;
    public float printDelayDefault = 3f; 

    public bool runNavDemoOnLaunch;


    // Start is called before the first frame update
    void Start() {
        boxText = GetComponent<Text>();
        printBuffer = new List<string>();
        currentlyPrinting = null;
        printDelay = printDelayDefault/60f;
        nextPrintTime = Time.time;

        printDelay = printDelayDefault / 60f;

        //TODO REMOVE
        if (runNavDemoOnLaunch) {
            RunDemo();
        }
    }

    // Update is called once per frame
    void Update(){

        //if nothing is printing but the buffer has a string, begin printing it
        if (printBuffer.Count > 0 && currentlyPrinting == null) {
            currentlyPrinting = printBuffer[0];
            printBuffer.RemoveAt(0);
        }
        
        //print out a character whenever the delay expires
        if (currentlyPrinting != null && Time.time >= nextPrintTime) {
            if(currentlyPrinting.Length >= 2) {
                boxText.text = boxText.text + currentlyPrinting[0];
                currentlyPrinting = currentlyPrinting.Substring(1);  
            }

            //on the last character, print a newline char and free up currentlyPrinting by nulling it
            else {
                boxText.text = boxText.text + currentlyPrinting[0] + "\n";
                currentlyPrinting = null;
            }

            nextPrintTime = Time.time + printDelay;
        }
    }

    #region Support Methods

    /* Set Delay
     * Changes the number of delay frames before a character is printed
     * 3 frames by default - does not work mid-buffer between lines
     */
    public void SetDelay(float frameDelay) {
        if (frameDelay == 0f)
            printDelay = printDelayDefault / 60f;
        else
            printDelay = frameDelay / 60f;
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
        FeedLine("[        LOADING        ]");
        FeedLine("|||||||||||||||||||||||||");
        FeedLine("Loading complete.");
        FeedLine("> Welcome to the VSCS-I terminal.");
        FeedLine("> Enter a command below to begin, or type HELP for a list of commands.");
    }
    #endregion
}
