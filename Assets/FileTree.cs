using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FileType {
    User,
    Directory,
    Text
}

public class FileNode {

    public string printOnEnter;

    public string locationName;
    public bool locked;
    public string password;
    public FileNode[] children;

    public FileNode() {

    }
}

public class FileTree {



}
