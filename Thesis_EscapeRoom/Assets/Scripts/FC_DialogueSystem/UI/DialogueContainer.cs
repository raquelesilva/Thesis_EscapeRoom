using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FancyCrab.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = StudioInfo.ASSET_MENU_PATH + "Dialogue Container")]
    public class DialogueContainer : ScriptableObject
    {
        [Header("Dialogue Settings")]
        public string dialogueID;
        public string dialogueTitle;

        [TextArea(2, 5)]
        public string description;

        [Header("Nodes")]
        public List<DialogueNode> nodes = new List<DialogueNode>();
        public DialogueNode startNode;

        [Header("Audio")]
        public AudioClip backgroundMusic;
        public float backgroundMusicVolume = 1f;

        [Header("Events")]
        public string onDialogueStartEvent;
        public string onDialogueEndEvent;

        // Dicionário para acesso rápido por índice (apenas IndexNodes)
        private Dictionary<int, IndexDialogueNode> indexNodes = new Dictionary<int, IndexDialogueNode>();

        // Obtém um node pelo seu índice (apenas IndexNodes)
        public DialogueNode GetNodeByIndex(int index)
        {
            // Atualiza o dicionário se necessário
            if (indexNodes.Count == 0 || indexNodes.Count != GetIndexNodes().Count)
            {
                RebuildIndexDictionary();
            }

            if (indexNodes.TryGetValue(index, out IndexDialogueNode indexNode))
            {
                return indexNode.nextNode ?? indexNode; // Retorna o nextNode se existir, senão retorna o próprio indexNode
            }

            Debug.LogWarning($"[FancyCrabStudios] No index node found with value: {index}");
            return null;
        }

        // Obtém todos os IndexNodes
        public List<IndexDialogueNode> GetIndexNodes()
        {
            return nodes.OfType<IndexDialogueNode>().ToList();
        }

        // Verifica se um índice específico existe
        public bool HasIndex(int index)
        {
            return GetIndexNodes().Any(n => n.indexValue == index);
        }

        // Encontra o próximo índice disponível
        public int GetNextAvailableIndex()
        {
            var usedIndices = GetIndexNodes().Select(n => n.indexValue).ToHashSet();
            int nextIndex = 0;

            while (usedIndices.Contains(nextIndex))
            {
                nextIndex++;
            }

            return nextIndex;
        }

        // Reconstroi o dicionário de índices
        private void RebuildIndexDictionary()
        {
            indexNodes.Clear();
            foreach (var node in GetIndexNodes())
            {
                if (!indexNodes.ContainsKey(node.indexValue))
                {
                    indexNodes[node.indexValue] = node;
                }
                else
                {
                    Debug.LogWarning($"[FancyCrabStudios] Duplicate index value found: {node.indexValue} in node {node.name}");
                }
            }
        }

        // Valida se não há índices duplicados
        public bool ValidateIndices(out string errorMessage)
        {
            var indexNodes = GetIndexNodes();
            var duplicateIndices = indexNodes
                .GroupBy(n => n.indexValue)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIndices.Count > 0)
            {
                errorMessage = $"Duplicate indices found: {string.Join(", ", duplicateIndices)}";
                return false;
            }

            errorMessage = "";
            return true;
        }
    }
}