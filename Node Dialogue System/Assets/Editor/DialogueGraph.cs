using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    DialogueGraphView _graphView;

    [MenuItem("Graph/Dialogue Graph")]//This just works when the method is static
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();//strech the element inside the window
        rootVisualElement.Add(_graphView);//parent the element to the window
    }

    void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var nodeCreateButton = new Button(()=> { _graphView.CreateNode("Dialogue Node"); });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        rootVisualElement.Add(toolbar);
    }

    //Here we create an instance of graph view to be seen inside the dialogue window
    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);//Remove when we do not need.
    }
}
