using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    readonly Vector2 defaultNodeSize = new Vector2(150, 200);
   public DialogueGraphView()
    {
        //Add the ability to zoom in and zoom out.
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        //Add monipulators to enable drag selection feature
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        //Create a grid to help in visualization and movimentation. We need to create a resource folder
        // and create an UI element editor window creator.
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        //add the css of the grid color.
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

       AddElement(GenerateEntryPointNode());
    }

    //Creates a port in a node. This need to know what is the node, if is a input or output port and if
    //it will have a single or multiple connections allowed.
     Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));//This typeof is just for shader graph stuff. Doesnt matter here.
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    DialogueNode GenerateEntryPointNode()//This will generate our nodes 
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            dialogueText = "EntryPoint",
            entryPoint = true,            
        };


        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();//Need to call it to update the creation of a new port.
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));

        return node;
    }

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName, //Pass the name from the other node
            dialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),//Create a new id
        };

        //We can choose which port we want to connect
        var button = new Button(()=> { AddChoicePort(dialogueNode); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);//Add the button in the right side

        //Create a input port as multiple so we can have multiple connections.
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.name = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));


        return dialogueNode;
    }

    void AddChoicePort(DialogueNode dialogueNode)
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;//lookup the output node name to put a name in the last one
        generatedPort.portName = $"Choice {outputPortCount}";//port name
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }
        
    //To connect the ports we need to know each one is good. We need to avoid the self connection and connection with the self output.
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
        
    }
}
