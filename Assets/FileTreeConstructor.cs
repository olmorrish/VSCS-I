using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* File Tree Constructor
 * Locates any files in the game directory titled "FileTree_X.txt", where X is an integer between 1 and 10
 * Adds nodes *sequentially* by parsing the text documents in order.
 *  ex: FileTree_2.txt contents should only be dependent on nodes from parsing FileTree_1.txt
 */
public class FileTreeConstructor : MonoBehaviour
{
    public bool addTestNode = false;

    public GameObject fileSystemObj;
    private FileSystem fileSystem;

    FileNode testNode;

    // Start is called before the first frame update
    void Start() {

        fileSystem = fileSystemObj.GetComponent<FileSystem>();

        

    }

    private void Update() {
        if(addTestNode) {   //just used for debuggin; adds a dummy node
            AddTestNode();
        }
    }

    





    /* NodeType: <NodeType> 
     * FQName: <Name> <Name2> ... <NodeName>
     * ReadBit: <1/0>
     * Owner: <OwnerName>
     * LockBit: <1/0>
     * Password: <Password>
     * NavText: <SomeString>
     * FileText: <SomeString>
     * 
     * Above is the structure expected for an entry. 
     * Each node must be separated by a single empty line.
     * Exclude "Head" node in the FNW for anything not directly below it in the tree. 
     */

    public FileNode ParseNode(string[] chunk) {
        FileNode n = new FileNode();

        //nodetype
        string nTypeString = chunk[0].Split(' ')[1];
        NodeType nType = (NodeType)Enum.Parse(typeof(NodeType), nTypeString, true);
        n.nodeType = nType;

        //name and fully-qualified name
        string[] rawFQN = chunk[1].Split(' ');
        string[] fqName = new string[rawFQN.Length - 1];
        for (int i = 0; i < fqName.Length; i++) {
            fqName[i] = rawFQN[i+1];
        }
        string name = fqName[fqName.Length - 1];
        n.fullyQualifiedName = fqName;
        n.nodeName = name;

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
            n.password = passwordStr;
        }
        else
            n.password = null; //don't even try to parse a password if locked wasn't enabled

        //navigation text 
        n.printNavOnEnter = chunk[6].Substring(9);

        //file text
        n.printFileOnEnter = chunk[7].Substring(10);

        //return the newly-built node
        return n;
    }

    private void AddTestNode() {
        string[] test = new string[8];
        test[0] = "NodeType: Text";
        test[1] = "FQName: Alice AutoDoc";
        test[2] = "ReadBit: 1";
        test[3] = "Owner: Alice";
        test[4] = "LockBit: 0";
        test[5] = "Password: ";
        test[6] = "NavText: Nav text works.";
        test[7] = "FileText: File text works.";

        testNode = ParseNode(test);
        fileSystem.coreFileTree.AddFileNode(testNode);

        addTestNode = false;
    }

}
