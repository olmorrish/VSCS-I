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
    public NodeType nodeType;                     //ex: text
    public string nodeName;                       //ex: "passwords"
    public string[] fullyQualifiedName;           //ex: "Oliver, myFiles, passwords"

    public bool playerReadPermission;             // if true, player may open to view text/directory. Users cannot be locked behind R-permission and must use a password.
    public string owner;

    public string printNavOnEnter;
    public string printFileOnEnter;
    public bool locked;
    public string password;

    //navigation properties
    public FileNode parent;
    public List<FileNode> children;
    

    //Empty Constructor
    // Used when building the tree via the FileTreeConstructor
    public FileNode() {
        parent = null;
        children = new List<FileNode>();
    }

    public FileNode(string name, string[] fqName, NodeType nType) {
        nodeType = nType;
        nodeName = name;
        fullyQualifiedName = fqName;

        children = new List<FileNode>();
        printNavOnEnter = null;
        printFileOnEnter = null;
        locked = false;
        password = null;
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

    /*
     * Intended for use by external classes.
     */
    public bool AddFileNode(FileNode toAdd) {

        int depth = toAdd.fullyQualifiedName.Length;
        string[] targetAddress = toAdd.fullyQualifiedName;
        return AddFileNodeAt(toAdd, targetAddress, head); ;
    }



    /*
     * Internal class recursive call. 
     * Recursively navigates the tree to find where the node must go.
     */
    private bool AddFileNodeAt(FileNode toAdd, string[] remainingAddress, FileNode current) {

        if (head == null) {     //if there is no head node, add the node there
            head = toAdd;
            return true;
        }
               
        //otherwise, look at the children of the current node
        //if any match the first part fo the address, go deeper. 
            // Otherwise, if the address is done, insert the node 
        else {
            //if the current node has children, check their names
            if (current.children.Count != 0) {
                foreach (FileNode child in current.children) {
                    if (child.nodeName.Equals(remainingAddress[0])) {

                        Debug.Log("Match found for node "+ toAdd +" at address " + remainingAddress[0]+ ".");

                        //we've found the next layer to go down, and must recursively call
                        //first update the address to cut what we just found:
                        string[] newAdr = new string[remainingAddress.Length - 1];
                        for (int i = 0; i < remainingAddress.Length - 1; i++) {
                            newAdr[i] = remainingAddress[i + 1];    //basically copying all but the first element to a new array
                        }

                        //perform the recursive call with the new shorter address and at the child node:
                        Debug.Log("Recursing into node \"" + child.nodeName + "\".");
                        return AddFileNodeAt(toAdd, newAdr, child);
                    }
                }

                //if this point is reached, then the search did not find a match
                //check if the address is done
                if (remainingAddress.Length <= 2) {  //if done, add the node as a child of current location

                    toAdd.parent = current;
                    current.children.Add(toAdd);
                    return true;
                }
                else {
                    string errorLog = "Addition of node " + toAdd.nodeName + " failed. Address was too long (no path found).The remaining address was: \"";
                    for (int i = 0; i < remainingAddress.Length; i++) {
                        errorLog += remainingAddress[i] + " | ";
                    }
                    Debug.Log(errorLog + "\".");
                    return false;                   //otherwise return that the addition failed
                }
            }

            //no more children below the current node 
            // check if the address is done
            else {
                if(remainingAddress.Length <= 2) {  //if done, add the node as a child of current location
                    toAdd.parent = current;
                    current.children.Add(toAdd);
                    return true;
                }
                else {
                    string errorLog = "Addition of node " + toAdd.nodeName + " failed. Address was too long (no children at last node). The remaining address was: \"";
                    for(int i=0; i<remainingAddress.Length; i++) {
                        errorLog += remainingAddress[i] + " | ";
                    }
                    Debug.Log(errorLog + "\".");

                    return false;                   //otherwise return that the addition failed
                }
            }




        }
        
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


