using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueNode : Node
{
    public string GUID;//Node id. Each node will have a unique id.
    public string dialogueText;//it will hold the text inside the node.
    public bool entryPoint = false;//This will be the start point
}
