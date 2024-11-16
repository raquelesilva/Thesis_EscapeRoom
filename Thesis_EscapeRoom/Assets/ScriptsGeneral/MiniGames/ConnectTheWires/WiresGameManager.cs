using CoreSystems.Extensions.Attributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Games.WiresGame
{
    public class WiresGameManager : MonoBehaviour
    {
        public static WiresGameManager instance;

        [SerializeField] private List<WiresCombination> wiresCombination;
        [SerializeField] private WireScript wirePrefab;
        [SerializeField] private GameObject connectorPrefab;
        [SerializeField] private PliersScript pliers;

        [SerializeField] private UnityEvent onStartGame;
        [SerializeField] private UnityEvent onPauseGame;
        [SerializeField] private UnityEvent onResumeGame;

        [SerializeField, Disable] bool focusedInGame;

        private List<WireTipScript> allWiresTips = new();
        private List<WireTipScript> leftWireTips = new();

        private WireTipScript selectedTip;
        private WireTipScript currentTip;
        private WireScript currentWire;

        private bool usingPliers;
        private Animator animator;

        private bool hasCompletedGame = false;
        private bool hasStarted = false;
        private bool gameIsPaused = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            animator = GetComponent<Animator>();    
        }

        public void StartGame()
        {
            if (hasStarted)
            {
                Resume();
                return;
            }

            pliers.SetInteractable(true);

            foreach (var combination in wiresCombination)
            {
                combination.rightWire.SetIsLocked(false);
                allWiresTips.Add(combination.rightWire);
                combination.leftWire.SetIsLocked(false);
                allWiresTips.Add(combination.leftWire);
                leftWireTips.Add(combination.leftWire);
            }
            hasStarted = true;
            gameIsPaused = false;

            onStartGame?.Invoke();
        }

        public void PauseGame()
        {
            pliers.SetInteractable(false);
            foreach (var item in allWiresTips)
            {
                item.SetIsLocked(true);
            }

            gameIsPaused = true;
            onPauseGame?.Invoke();
        }

        private void Resume()
        {
            if (!hasCompletedGame)
            {
                pliers.SetInteractable(true);
                foreach (var item in allWiresTips)
                {
                    item.SetIsLocked(false);
                }
            }

            gameIsPaused = false;
            onResumeGame?.Invoke();
        }

        public void SetIsFocus(bool isPlaying)
        {
            focusedInGame = isPlaying;
        }

        private void Update()
        {
            if (focusedInGame && !gameIsPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Pausing game from Electrical");
                PauseGame();
            }
        }

        public void SetCombination()
        {
            if (currentTip != null)
            {
                if (currentTip.GetOtherWire() == null)
                {
                    selectedTip.SetOtherWireTip(currentTip);
                    currentTip.SetOtherWireTip(selectedTip);
                    currentWire.SetIsBlock(true);
                    currentWire.DrawLine(selectedTip.GetTip().position, currentTip.GetTip().position);
                    SetNewConnector(currentTip);

                    currentWire.tips.Add(currentTip);
                    currentWire.tips.Add(selectedTip);
                    VerifyConnections();
                }
                else
                {
                    DestroyWire(currentWire);
                }
            }
            else
            {
                DestroyWire(currentWire);
            }
        }

        public void DestroyWire(WireScript currentWire)
        {
            foreach (var item in currentWire.tips)
            {
                item.SetOtherWireTip(null);
            }
            foreach (var item in currentWire.connectors)
            {
                Destroy(item.gameObject);
            }

            Rigidbody rb = currentWire.AddComponent<Rigidbody>();
            Interactable interactable = currentWire.AddComponent<Interactable>();
            MeshCollider meshColl = currentWire.GetComponent<MeshCollider>();

            meshColl.convex = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            interactable.IsGrabable(true);

            Destroy(currentWire);
        }

        private void VerifyConnections()
        {
            bool allCorrect = false;

            for (int i = 0; i < wiresCombination.Count; i++)
            {
                WireTipScript leftFoundWireTip = leftWireTips.Find((x) => x.name == wiresCombination[i].leftWire.name);
                if (leftFoundWireTip == null)
                {
                    allCorrect = false;
                    break;
                }

                WireTipScript otherWireTip = leftFoundWireTip.GetOtherWire();
                if (otherWireTip == null)
                {
                    allCorrect = false;
                    break;
                }

                if (wiresCombination[i].rightWire == otherWireTip)
                {
                    allCorrect = true;
                    continue;
                }
                else
                {
                    allCorrect = false;
                    break;
                }
            }

            if (allCorrect)
            {
                Debug.Log("Win Game");
                animator.SetTrigger("Completed");
                pliers.SetInteractable(false);
                foreach (var item in allWiresTips)
                {
                    item.SetIsLocked(true);
                    item.GetNaipe().SetActive(true);
                }
                hasCompletedGame = true;
            }
        }

        public void SetNewWireTip(WireTipScript tip)
        {
            currentWire = Instantiate(wirePrefab, tip.GetTip());
            currentWire.GetComponent<MeshRenderer>().material = tip.GetMaterial();
            SetNewConnector(tip);
        }

        public void SetNewConnector(WireTipScript tip)
        {
            var connectorBall = Instantiate(connectorPrefab, tip.GetTip());
            connectorBall.GetComponent<MeshRenderer>().material = tip.GetMaterial();
            currentWire.connectors.Add(connectorBall);
        }

        #region Getters & Setters
        public void SetCurrentWireTip(WireTipScript newCurrentTip)
        {
            currentTip = newCurrentTip;
        }
        public void SetSelectedWireTip(WireTipScript newSelectedTip)
        {
            selectedTip = newSelectedTip;
        }

        public WireScript GetCurrentWire()
        {
            return currentWire;
        }

        public bool UsingPliers()
        {
            return usingPliers;
        }
        public void SetUsingPliers(bool decision)
        {
            usingPliers = decision;
        }
        
        public PliersScript GetPliers()
        {
            return pliers;  
        }
        #endregion
    }

    [System.Serializable]
    public class WiresCombination
    {
        public string combinationName;

        [Header("Wires")]
        public WireTipScript leftWire;
        public WireTipScript rightWire;

        [Header("Others")]
        public Material wireMaterial;
    }

}
