using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    private static int boardWidth = 28;
    private static int boardHeight = 36;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject i in objects)
        {
            Vector2 pos = i.transform.position;

            if(i.name != "PacMan")
            {
                board[(int)pos.x, (int)pos.y] = i;
            }
            else
            {
                Debug.Log("Pacman at: " + pos);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
