using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* 
 * Interlinked with the FileTreeConstructor (bidirectional dependency)
 */
public class FileSystem : MonoBehaviour
{
    public FileTree coreFileTree;
    public FileNode currentNode;

    public int numberOfFilesToProcess;

    public GameObject navTextObject;
    public GameObject fileTextObject;
    private TerminalPrinter navTextPrinter;
    private TerminalPrinter fileTextPrinter;


    //public bool runBuildRoutineFromFiles;

    // Start is called before the first frame update
    void Start() {

        //get references to the two terminals and the constructor
        navTextPrinter = navTextObject.GetComponent<TerminalPrinter>();
        fileTextPrinter = fileTextObject.GetComponent<TerminalPrinter>();

        //Build routine requires a head node
        string[] headFilePath = { "Home" };
        FileNode headDirectory = new FileNode("Home", headFilePath, NodeType.Directory);
        coreFileTree = new FileTree(headDirectory);
        RunBuildRoutine();
        currentNode = coreFileTree.head;
    }

    /* Open Request
     * The player has requested access to a file with GOTO or OPEN
     * Determine if the file exists, then if they have permission to open it.
     */
    public bool OpenRequest(string fileToOpen, string optionalPassword) {   //the password is only acknowledged if the requirement comes up

        bool found = false;

        //search the child nodes of the player's location to see if their request makes sense
        foreach (FileNode child in currentNode.children){

            if (fileToOpen.Equals(child.nodeName, StringComparison.CurrentCultureIgnoreCase)) {

                found = true;

                //we've found the file, now CHECK PERMISSIONS
                //first, check that the file is not a (non-user) file that is restricted by read-permissions
                if (PlayerCanOpen(child)) {   

                    if (child.nodeType == NodeType.Text) {
                        //text nodes are special; we don't navigateinto them, we just read them
                        fileTextPrinter.Wipe(); //clear space for the new file
                        navTextPrinter.FeedLine("> File opened.");
                        fileTextPrinter.FeedLine(child.printFileOnEnter); //we print the text for the CHILD TEXT NODE
                    }
                    else {
                        currentNode = child;                                             //go into the node
                        navTextPrinter.FeedLine(currentNode.printNavOnEnter);            //then print details
                        fileTextPrinter.FeedLine(currentNode.printFileOnEnter);     
                        string location = GetCurrentFQN();
                        navTextPrinter.FeedLine("> Now at: " + location.ToUpper());
                    }
                }

                //if default permission was denied, it was either due to read permission being disabled, or due to a password lock
                else {
                    //1. PASSWORD CHECKS - These will only occur if the read permission is enabled. 
                    //check if the player provided a password (field will be null if they haven't). If so, try it against the password for the node:
                    if(optionalPassword != null && child.locked && child.playerReadPermission) {
                        if (child.password.Equals(optionalPassword)) {
                            child.locked = false;
                            //the success message is different if a user profile was just unlocked.
                            if (child.nodeType == NodeType.User)
                                navTextPrinter.FeedLine("> User profile " + child.nodeName + " unlocked.");
                            else
                                navTextPrinter.FeedLine("> File unlocked successfully.");
                        }
                        else {
                            navTextPrinter.FeedLine("> Incorrect password.");
                            Debug.Log("Player entered \"" + optionalPassword + "\" but the correct password is \"" + child.password + "\".");
                        }
                    }
                    else if (child.locked){
                        navTextPrinter.FeedLine("> This file is password locked. Please provide a password as a second argument to open.");
                    }

                    //2. READ PERMISSION CHECKS - The file is not password locked; permission must have been denied due to read permission error.
                    else if (!child.playerReadPermission) {
                        navTextPrinter.FeedLine("> You do not have permission to access this file (ERR: read permission [_]).");
                        navTextPrinter.FeedLine("> Please contact a MEGA user to alter this file's read permission state to [R].");

                    }
                }
            }
        }

        //if node was not found, print an error.
        if (!found) {
            navTextPrinter.FeedLine("> Could not find requested file.");
            return false;
        }
        else
            return true;
    }



    /* Back Request
     * Processes a player's request to go to the parent node
     * Finishes my printing the player's current location in the filetree, or an error if at the head node
     */
    public bool BackRequest() {
        if (!currentNode.Equals(coreFileTree.head)) {
            currentNode = currentNode.parent;
            navTextPrinter.FeedLine("> Now at: " + GetCurrentFQN().ToUpper());
            return true;
        }
        else {
            navTextPrinter.FeedLine("> Cannot go back further.");
            return false;
        }
    }



    /* Get Child List
     * Formats a string array of details and permissions of the current node's children
     * Used to build a string set to print out when player enters LIST or similar commands
     */
    public string[] GetChildList() {

        int numChildren = currentNode.children.Count;

        string[] ret = new string[numChildren];
        for (int i=0; i< numChildren; i++) {

            //grab each file name and modify it's prinout based on the node type
            string printName = currentNode.children[i].nodeName;
            NodeType nType = currentNode.children[i].nodeType;
            switch (nType) {
                case NodeType.User:
                    printName = "[USER] " + printName;
                    break;
                case NodeType.Directory:
                    printName = "[FLDR] " + printName;
                    break;
                case NodeType.Text:
                    printName = "[TEXT] " + printName;
                    break;
            }

            printName = "] " + printName;

            //pre-append the lock status of the file
            if (currentNode.children[i].locked)
                printName = "#" + printName;
            else
                printName = "_" + printName;

            //then, pre-append the file permissions for the node
            if (nType != NodeType.User) {
                if (currentNode.children[i].playerReadPermission)
                    printName = "R," + printName;
                else
                    printName = "_," + printName;
            }

            printName = "[" + printName;

            

            ret[i] = printName;
        }

        return ret;
    }


    /* Get Current Fully-Qualified Name
     * Retrieves the FQN of the current node as a formatted string, including slashes
     */
    public string GetCurrentFQN() {
        string[] fqn = currentNode.fullyQualifiedName;

        string ret = fqn[0];
        for (int i = 1; i<fqn.Length; i++) {
            ret += "/" + fqn[i];
        }
        return ret;
    }
    
    
    
    /* Player Can Open
     * Given a node, checks if the player is allowed to open it. 
     * Utilized internally by method OpenRequest() 
     */
    private bool PlayerCanOpen(FileNode node) {

        bool passwordIssue = false; //TODO
        bool readPermissionIssue = false;

        //any type of file may be password locked
        if (node.locked) {
            passwordIssue = true;
        }

        //read permission locks cannot be set on users, so we check node-types differently for read permission
        switch (node.nodeType) {
            case NodeType.User:
                break;
            case NodeType.Text:
                if (!node.playerReadPermission)
                    readPermissionIssue = true;
                break;
            case NodeType.Directory:
                if (!node.playerReadPermission)
                    readPermissionIssue = true;
                break;
        }

        return !(passwordIssue || readPermissionIssue); //if either flag was tripped, player is not allowed access; return false
        
    }




    /* Run Build Routine
     * Called by the FileSystem as a part of its setup
     * Runs ten phases of nodes
     */
    private void RunBuildRoutine() {
        for (int i = 1; i < numberOfFilesToProcess + 1; i++) {
            AddFileContentsToTree(i);
        }
    }

    private void AddFileContentsToTree(int fileNumber) {
        string rawText = Resources.Load<TextAsset>("FileTree_" + fileNumber.ToString()).text;
        string[] lines = rawText.Split('\n');

        int numLinesRemaining = lines.Length;
        int positionIndex = 0;

        while (numLinesRemaining >= 8) {

            string[] thisNodeLines = new string[8];

            for (int i = 0; i < 8; i++) {
                thisNodeLines[i] = lines[positionIndex + i];
            }

            //build the actual node and add if to the FileSystem's core tree
            FileNode newNode = ParseNode(thisNodeLines);
            coreFileTree.AddFileNode(newNode);

            Debug.Log("Added node with name \"" + newNode.nodeName + "\".");

            positionIndex += 9;        //8 of the lines just processed, and 1 space
            numLinesRemaining -= 9;    //same; reduce the flag so we know when to stop
        }

    }



    /* NodeType: <NodeType> 
     * FQName: <Name> <Name2> ... <NodeName>
     * ReadBit: <1/0>
     * Owner: <OwnerName>
     * LockBit: <1/0>
     * Password: <Password>
     * NT: <SomeString>
     * FT: <SomeString>
     * 
     * Above is the structure expected for an entry. 
     * Each node must be separated by line beginning with a single dash: '-'.
     * Exclude "Head" node in the FNW for anything not directly below it in the tree. 
     */

    public FileNode ParseNode(string[] chunk) {

        //this sets children to empty and parent to null
        FileNode n = new FileNode();

        //nodetype
        string nTypeString = chunk[0].Split(' ')[1];
        NodeType nType = (NodeType)Enum.Parse(typeof(NodeType), nTypeString, true);
        n.nodeType = nType;

        //name and fully-qualified name
        string[] fqName = chunk[1].Split(' ')[1].Split('/');
        string name = fqName[fqName.Length - 1];
        n.fullyQualifiedName = fqName;
        n.nodeName = name.Substring(0,name.Length-1);                //DELETES THE CARRAIGE RETURN CHARACTER

        //read permission
        char bitChar = chunk[2].Split(' ')[1][0]; //0 index is just grabbing the first char of the string
        if (bitChar == '0')
            n.playerReadPermission = false;
        else
            n.playerReadPermission = true;  //thus, defaults to allow access on any typo

        //owner name
        string ownerName = chunk[3].Split(' ')[1];
        n.owner = ownerName;

        //lock y/n
        bitChar = chunk[4].Split(' ')[1][0]; //0 index is just grabbing the first char of the string
        if (bitChar == '1')
            n.locked = true;
        else
            n.locked = false;  //thus, defaults to allow access on any typo

        //password
        if (n.locked) {
            string passwordStr = chunk[5].Split(' ')[1];
            n.password = passwordStr.Substring(0, passwordStr.Length - 1);                //DELETES THE CARRAIGE RETURN CHARACTER;
        }
        else
            n.password = null; //don't even try to parse a password if locked wasn't enabled

        //navigation text 
        if (chunk[6].Length > 4)
            n.printNavOnEnter = chunk[6].Substring(4);
        else {
            n.printNavOnEnter = null;
            //Debug.Log("Null nav text");
        }

        //file text
        if (chunk[7].Length > 4) {
            string rawFileText = chunk[7].Substring(4);
            n.printFileOnEnter = rawFileText.Replace('|', '\n');
        }
        else {
            n.printNavOnEnter = null;
            //Debug.Log("Null file text");
        }

        //return the newly-built node
        return n;
    }

}
