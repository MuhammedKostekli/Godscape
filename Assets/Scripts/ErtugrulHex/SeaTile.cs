using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaTile : MonoBehaviour
{
    [SerializeField] Material seaMat;
    [SerializeField] Mesh seaMesh;
    List<GameObject> Tiles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (gameObj.name == "A_Tile (1)(Clone)")
            {
                Tiles.Add(gameObj);
            }
        }
        foreach (GameObject gameObj in Tiles)
        {
            if (gameObj.name == "A_Tile (1)(Clone)")
            {
                gameObj.GetComponent<Renderer>().material = seaMat;
                gameObj.GetComponent<MeshFilter>().mesh = seaMesh;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
