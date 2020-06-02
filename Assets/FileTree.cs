using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType {
    User,
    Directory,
    Text
}

public class FileNode {

    //all node properties
    NodeType type;                                //ex: text
    public string nodeName;                       //ex: "passwords"
    public string[] fullyQualifiedName;           //ex: "Oliver, myFiles, passwords"
    public string printNavOnEnter;
    public string printFileOnEnter;
    public bool locked;
    public string password;

    //navigation properties
    public FileNode parent;
    public List<FileNode> children;
    private string v1;
    private string v2;

    /* Head Node Constructor
     * 
     */
    public FileNode(string name, NodeType t) {
        type = t;
        nodeName = name;
        children = new List<FileNode>();
        printNavOnEnter = null;
        printFileOnEnter = null;
        locked = false;
        password = null;
    }

    public FileNode(string[] fqName, NodeType node) {

        //TODO
    }



    public FileNode(string fqName, NodeType node, string password) {

        //TODO
    }

}

public class FileTree {

    public FileNode head; 

    /* Default Contructor
     */
    public FileTree() {
        head = null;
    }

    public bool AddFileNode(FileNode toAdd) {

        if (head == null) {     //if there is no head node, add the node there
            head = toAdd;
        }
        else {
            FileNode current = head;
            string[] nodeDestination = toAdd.fullyQualifiedName;
        }
                
        //TODO
        return false;
    }

    public bool DeleteFileNode() {

        //TODO
        return false;
    }

    //CONSTRUCTOR, head node
    public FileTree(FileNode h) {
        head = h;
    }
}


