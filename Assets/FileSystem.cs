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
        FileNode headDirectory = new FileNode("Home", NodeType.Directory);
        coreFileTree = new FileTree(headDirectory);
        currentNode = coreFileTree.head;

        //string[] filePath = { "Home", "Oliver" };
        //FileNode userOliver = new FileNode(filePath, NodeType.User);
        //coreFileTree.AddFileNode(userOliver);

        FileNode headDirectory2 = new FileNode("Home2", NodeType.Directory);
        coreFileTree.head.children.Add(headDirectory2);

    }

    // Update is called once per frame
    void Update(){

    }

    public bool OpenRequest(string fileToOpen) {

        //search the child nodes of the player's location to see if their request makes sense
        foreach (FileNode child in currentNode.children){
            if (fileToOpen.Equals(child.nodeName)) {
                currentNode = child;
                navTextPrinter.FeedLine(currentNode.printNavOnEnter);
                fileTextPrinter.FeedLine(currentNode.printFileOnEnter);
            }
        }

        return false;
    }

    public string[] getChildList() {
        int numChildren = currentNode.children.Count;

        string[] ret = new string[numChildren];
        for (int i=0; i< numChildren; i++) {
            ret[i] = currentNode.children[i].nodeName;
        }

        return ret;
    }
}
