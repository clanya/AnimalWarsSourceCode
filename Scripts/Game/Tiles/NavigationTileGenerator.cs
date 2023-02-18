using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tiles
{
    public class NavigationTileGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform tileParent;

        public GameObject GenerateTile(Vector2 generatePos)
        {
            var tile = Instantiate(tilePrefab, new Vector3(generatePos.x,0,generatePos.y), Quaternion.Euler(new Vector3(90,0,0)), tileParent);
            return tile;
        }
    }
}

