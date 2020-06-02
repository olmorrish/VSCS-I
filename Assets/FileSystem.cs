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

        string[] filePath = { "Home", "Oliver" };
        FileNode userOliver = new FileNode("Oliver", filePath, NodeType.User);
        coreFileTree.AddFileNode(userOliver);


    }

    // Update is called once per frame
    void Update(){

    }


    public bool OpenRequest(string fileToOpen) {

        //search the child nodes of the player's location to see if their request makes sense
        foreach (FileNode child in currentNode.children){
            if (fileToOpen.ToUpper().Equals(child.nodeName.ToUpper())) {
                currentNode = child;
                navTextPrinter.FeedLine(currentNode.printNavOnEnter);
                fileTextPrinter.FeedLine(currentNode.printFileOnEnter);
            }
        }

        return false;
    }

    public bool BackRequest() {
        if (!currentNode.Equals(coreFileTree.head)) {
            currentNode = currentNode.parent;
            return true;
        }
        else
            return false;
    }

    public string[] GetChildList() {
        int numChildren = currentNode.children.Count;

        string[] ret = new string[numChildren];
        for (int i=0; i< numChildren; i++) {
            ret[i] = currentNode.children[i].nodeName;
        }

        return ret;
    }


}
