using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Games.WiresGame
{
    public class WireTipScript : MonoBehaviour
    {
        WiresGameManager manager;

        [SerializeField] private bool isLocked;
        [SerializeField] private Material material;

        [SerializeField] private Transform tip;
        [SerializeField] private SpriteRenderer naipe;
        [SerializeField] private WireTipScript otherWire;

        private void Start()
        {
            manager = WiresGameManager.instance;
        }


        private void OnMouseOver()
        {
            if (isLocked || manager.UsingPliers()) return;
            manager.SetCurrentWireTip(this);
        }
        private void OnMouseExit()
        {
            if (isLocked || manager.UsingPliers()) return;
            manager.SetCurrentWireTip(null);
        }


        private void OnMouseUp()
        {
            if (isLocked || manager.UsingPliers()) return;
            manager.SetCombination();
        }
        private void OnMouseDown()
        {
            if (isLocked || manager.UsingPliers()) return;
            manager.SetSelectedWireTip(this);
            manager.SetNewWireTip(this);
        }

        #region GettersAndSetters

        public bool IsLocked()
        {
            return isLocked;
        }

        public WireTipScript GetOtherWire()
        {
            return otherWire;
        }

        public Material GetMaterial()
        {
            return material;
        }

        public Transform GetTip()
        {
            return tip;
        }
        public GameObject GetNaipe()
        {
            return naipe.gameObject;
        }

        public void SetIsLocked(bool newState)
        {
            isLocked = newState;
        }
        public void SetOtherWireTip(WireTipScript newOtherWire)
        {
            otherWire = newOtherWire;
        }

        #endregion
    }
}