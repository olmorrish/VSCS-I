using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystem : MonoBehaviour
{
    public FileTree coreFileTree;
    FileNode currentNode;

    public GameObject navTextObject;
    public GameObject fileTextObject;
    private TerminalPrinter navTextPrinter;
    private TerminalPrinter fileTextPrinter;

    // Start is called before the first frame update
    void Start() {

        //get references to the two terminals
        navTextPrinter = navTextObject.GetComponent<TerminalPrinter>();
        fileTextPrinter = fileTextObject.GetComponent<TerminalPrinter>();

        //build the basic file tree
        string[] headFilePath = { "Home" };
        FileNode headDirectory = new FileNode("Home", headFilePath, NodeType.Directory);
        coreFileTree = new FileTree(headDirectory);
        currentNode = coreFileTree.head;

        string[] filePathA = { "Home", "Alice" };
        FileNode userA = new FileNode("Alice", filePathA, NodeType.User);
        coreFileTree.AddFileNode(userA);

        string[] filePathB = { "Home", "Bob" };
        FileNode userB = new FileNode("Bob", filePathB, NodeType.User);
        coreFileTree.AddFileNode(userB);

        string[] exampleFilePath = { "Alice", "ExampleDoc"};
        FileNode exampleDoc = new FileNode("ExampleDoc", exampleFilePath, NodeType.Text);
        exampleDoc.playerReadPermission = true;
        exampleDoc.printFileOnEnter = "    A computer is a machine that can be instructed to carry out sequences of arithmetic or logical operations " +
            "automatically via computer programming. Modern computers have the ability to follow generalized sets of operations, called programs. " +
            "\n    These programs enable computers to perform an extremely wide range of tasks. A \"complete\" computer including the hardware, the " +
            "operating system (main software), and peripheral equipment required and used for \"full\" operation can be referred to as a computer " +
            "system. This term may as well be used for a group of computers that are connected and work together, in particular a computer network or computer cluster. ";
        coreFileTree.AddFileNode(exampleDoc);

    }



    // Update is called once per frame
    void Update(){

    }






    /* Open Request
     * The player has requested access to a file with GOTO or OPEN
     * Determine if the file exists, then if they have permission to open it.
     */
    public bool OpenRequest(string fileToOpen) {

        bool found = false;

        //search the child nodes of the player's location to see if their request makes sense
        foreach (FileNode child in currentNode.children){

            if (fileToOpen.ToUpper().Equals(child.nodeName.ToUpper())) {

                //we've found the file, now CHECK PERMISSIONS
                //first, check that the file is not a (non-user) file that is restricted by read-permissions
                if (PlayerCanOpen(child)) {   

                    if (child.nodeType == NodeType.Text) {
                        //text nodes are special; we don't navigateinto them, we just read them
                        fileTextPrinter.Wipe(); //clear space for the new file
                        navTextPrinter.FeedLine("> File opened.");
                        fileTextPrinter.FeedLine(child.printFileOnEnter);
                        found = true;
                    }
                    else {
                        currentNode = child;
                        string location = GetCurrentFQN();
                        navTextPrinter.FeedLine("> Now at: " + location.ToUpper());
                        navTextPrinter.FeedLine(currentNode.printNavOnEnter);
                        fileTextPrinter.FeedLine(currentNode.printFileOnEnter);

                    }
                }

                //if permission was denied:
                else {
                    navTextPrinter.FeedLine("> You do not have permission to access this file (ERR: file permission [_]).");
                    navTextPrinter.FeedLine("> Please contact a MEGA user to alter this file's permission state to [R].");
                }

                //set the flag since we found the right node
                found = true;
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

            //then, pre-append the file permissions for the node
            if(nType != NodeType.User) {
                if (currentNode.children[i].playerReadPermission)
                    printName = "[R] " + printName;
                else
                    printName = "[_] " + printName;
            }

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


}
