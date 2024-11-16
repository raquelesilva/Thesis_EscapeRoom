using CoreSystems.Extensions.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace IGameMech
{
    public class IGameMechMerger : IGameMech
    {
        [SerializeField] private List<IGameMech> games = new();
        [ShowOnly] public bool checking = false;

        private void OnValidate()
        {
            for (int i = 0; i < games.Count; i++)
            {
                games[i]._CheckSelf.Clear();
                games[i]._CheckEach = _CheckEach;
                games[i]._merger = this;
            }
        }

        public void CheckAll()
        {
            CheckAll(null);
        }

        public void CheckAll(IGameMech caller)
        {
            checking = true;
            List<byte> dones = new();
            if (caller != null) { dones.Add(caller.done); }
            for (int i = 0; i < games.Count; i++)
            {
                if (games[i] == caller) continue;
                dones.Add(games[i].GetDone());
            }
            checking = false;

            if (dones.Contains(2))
            {
                onNotDone();
            }
            else if (dones.Contains(0))
            {
                onFailure();
            }
            else
            {
                onSuccess();
            }
        }
    }
}