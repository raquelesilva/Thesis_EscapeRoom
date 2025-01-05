using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class RandomPlaces : MonoBehaviour
    {
        [SerializeField] List<Transform> places = new List<Transform>();
        [SerializeField] List<Transform> pieces = new List<Transform>();

        // Start is called before the first frame update
        void Start()
        {
            ShufflePlaces();
            SetPlaces();
        }

        private void ShufflePlaces()
        {
            places = places.OrderBy(x => Random.value).ToList();
        }

        private void SetPlaces()
        {
            for(int i = 0; i < pieces.Count; i++)
            {
                pieces[i].position = places[i].position;
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}