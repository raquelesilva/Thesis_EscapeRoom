using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace FancyCrab.DialogueSystem.Editor
{
    public class DialogueNodeView : Node
    {
        public DialogueNode node;
        private DialogueGraphView graphView;

        public DialogueNodeView(DialogueNode node, DialogueGraphView graphView)
        {
            this.node = node;
            this.graphView = graphView;

            // Style
            title = node.name;
            style.backgroundColor = GetNodeColor(node.GetType());

            // Add border
            style.borderTopWidth = 2;
            style.borderBottomWidth = 2;
            style.borderLeftWidth = 2;
            style.borderRightWidth = 2;
            style.borderTopColor = new Color(1f, 0.5f, 0f);
            style.borderBottomColor = new Color(1f, 0.5f, 0f);
            style.borderLeftColor = new Color(1f, 0.5f, 0f);
            style.borderRightColor = new Color(1f, 0.5f, 0f);

            // Fixar largura do node
            style.width = 300;

            // Garantir que o conteúdo não expanda a largura
            mainContainer.style.flexWrap = Wrap.Wrap;
            mainContainer.style.maxWidth = 280;

            extensionContainer.style.maxWidth = 280;
            inputContainer.style.maxWidth = 280;
            outputContainer.style.maxWidth = 280;

            // Input port - apenas se não for IndexNode
            if (!(node is IndexDialogueNode))
            {
                var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
                inputPort.portName = "In";
                inputContainer.Add(inputPort);
            }

            // Actor field (apenas para nodes que não são IndexNode)
            if (!(node is IndexDialogueNode))
            {
                var actorField = new ObjectField("Actor")
                {
                    objectType = typeof(DialogueActor),
                    value = node.actor
                };
                actorField.style.maxWidth = 260;
                actorField.RegisterValueChangedCallback(evt => node.actor = evt.newValue as DialogueActor);
                mainContainer.Add(actorField);
            }

            // Create specific fields based on node type
            CreateNodeSpecificFields();

            RefreshExpandedState();
            RefreshPorts();
        }

        private Color GetNodeColor(System.Type nodeType)
        {
            if (nodeType == typeof(TextDialogueNode))
                return new Color(0.2f, 0.3f, 0.5f);
            else if (nodeType == typeof(ChoiceDialogueNode))
                return new Color(0.5f, 0.3f, 0.2f);
            else if (nodeType == typeof(IndexDialogueNode))
                return new Color(0.3f, 0.5f, 0.2f);
            else
                return new Color(0.3f, 0.3f, 0.3f);
        }

        private void CreateNodeSpecificFields()
        {
            if (node is TextDialogueNode textNode)
            {
                CreateTextNodeFields(textNode);
            }
            else if (node is ChoiceDialogueNode choiceNode)
            {
                CreateChoiceNodeFields(choiceNode);
            }
            else if (node is IndexDialogueNode indexNode)
            {
                CreateIndexNodeFields(indexNode);
            }
        }

        private void CreateTextNodeFields(TextDialogueNode textNode)
        {
            var textField = new TextField("Dialogue Text")
            {
                multiline = true,
                value = textNode.dialogueText
            };
            textField.style.height = 100;
            textField.style.maxWidth = 260;
            textField.style.whiteSpace = WhiteSpace.Normal;
            textField.RegisterValueChangedCallback(evt =>
            {
                textNode.dialogueText = evt.newValue;
                EditorUtility.SetDirty(textNode);
            });
            mainContainer.Add(textField);

            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
            outputPort.portName = "Next";
            outputContainer.Add(outputPort);
        }

        private void CreateChoiceNodeFields(ChoiceDialogueNode choiceNode)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = i;

                var choiceField = new TextField($"Choice {i + 1}")
                {
                    multiline = true,
                    value = choiceNode.Choices[index].choiceText
                };
                choiceField.style.height = 60;
                choiceField.style.maxWidth = 260;
                choiceField.style.whiteSpace = WhiteSpace.Normal;
                choiceField.RegisterValueChangedCallback(evt => {
                    var choices = choiceNode.Choices;
                    var newChoice = choices[index];
                    newChoice.choiceText = evt.newValue;
                    choices[index] = newChoice;

                    var fieldInfo = typeof(ChoiceDialogueNode).GetField("choices",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(choiceNode, choices);
                        EditorUtility.SetDirty(choiceNode);
                    }
                });
                mainContainer.Add(choiceField);

                var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                outputPort.portName = $"Option {i + 1}";
                outputContainer.Add(outputPort);
            }
        }

        private void CreateIndexNodeFields(IndexDialogueNode indexNode)
        {
            var indexField = new IntegerField("Index Value")
            {
                value = indexNode.indexValue
            };
            indexField.style.maxWidth = 260;
            indexField.RegisterValueChangedCallback(evt => {
                indexNode.indexValue = evt.newValue;
                indexNode.UpdateName();
                title = indexNode.name;
                EditorUtility.SetDirty(indexNode);
            });
            mainContainer.Add(indexField);

            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
            outputPort.portName = "Next";
            outputContainer.Add(outputPort);
        }

        public void UpdateTitle()
        {
            title = node.name;
        }
    }
}