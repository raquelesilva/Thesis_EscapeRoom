using CoreSystems.Extensions.Attributes;
using CoreSystems.Managers;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace IGameMech
{
    public class IGameMech : MonoBehaviour
    {
        [Space(10)]
        [HorizontalLine]
        [Header("Manager - for setting up")]

        [SerializeField] public CheckSelf _CheckSelf;
        [SerializeField] public CheckEach _CheckEach;

        [ShowOnly] public byte done = 0;
        [ShowOnly] public IGameMechMerger _merger = null;
        
        [SerializeField] private NotificationManager notificationManager;
         
        private void Start()
        {
            if (notificationManager == null)
            {
                notificationManager = FindObjectOfType<NotificationManager>();
            }
        }

        public void Notify(string text)
        {
            notificationManager.SetMessage(text, Color.white);
        }

        public void NotifyRight(string text)
        {
            notificationManager.SetMessage(text, notificationManager.GetColorRight());
        }

        public void NotifyWrong(string text)
        {
            notificationManager.SetMessage(text, notificationManager.GetColorWrong());
        }

        public virtual void CheckAnswers()
        {
            if (_merger == null)
            {
                switch (done)
                {
                    case 0:
                        onFailure();
                        break;

                    case 1:
                        onSuccess();
                        break;

                    default:
                        onNotDone();
                        break;
                }
            }
            else if (!_merger.checking)
            {
                _merger.CheckAll(this);
            }
        }

        public byte GetDone()
        {
            CheckAnswers();
            return done;
        }

        #region CheckSelf
        public bool checkSelf() { return _CheckSelf.checkSelf; }
        public void uncheckSelf() { _CheckSelf.checkSelf = false; }
        public void onSuccess() { _CheckSelf.onSuccess?.Invoke(); }
        public void onFailure() { _CheckSelf.onFailure?.Invoke(); }
        public void onNotDone() { _CheckSelf.onNotDone?.Invoke(); }
        #endregion

        #region CheckSelf
        public bool checkEach() { return _CheckEach.checkEach; }
        public void uncheckEach() { _CheckEach.checkEach = false; }
        public void onRightEach() { _CheckEach.onRightEach?.Invoke(); }
        public void onWrongEach() { _CheckEach.onWrongEach?.Invoke(); }
        #endregion

        [Serializable]
        public class CheckSelf
        {
            public bool checkSelf = false;
            [Space(10)]
            public UnityEvent onSuccess;
            public UnityEvent onFailure;
            public UnityEvent onNotDone;

            public void Clear()
            {
                checkSelf = false;
                onSuccess = null;
                onFailure = null;
                onNotDone = null;
            }
        }

        [Serializable]
        public class CheckEach
        {
            public bool checkEach = false;
            [Space(10)]
            public UnityEvent onRightEach;
            public UnityEvent onWrongEach;

            public void Clear()
            {
                checkEach = false;
                onRightEach = null;
                onWrongEach = null;
            }
        }
    }

    public enum Transition
    {
        SpriteSwap, ColorTint
    }

    [Serializable]
    public class BoolOption
    {
        public BoolOption(string text, bool isTrue)
        {
            this.text = text;
            this.isTrue = isTrue;
        }

        public string text;
        public bool isTrue;
    }

    [Serializable]
    public class SwapPair
    {
        public bool spriteOrColor = true;
        public Sprite sprite = null;
        public Color32 color = Color.white;

        public SwapPair()
        {
            spriteOrColor = true;
            sprite = null;
            color = Color.white;
        }

        public void SwapValues(Image image)
        {
            if (spriteOrColor)
            {
                image.sprite = sprite;
            }
            else
            {
                image.color = color;
            }
        }
    }
}