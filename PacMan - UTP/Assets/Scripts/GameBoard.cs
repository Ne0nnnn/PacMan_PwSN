using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    private static int boardWidth = 28;
    private static int boardHeight = 36;

    public int totalPellets = 0;
    public int score = 0;
    public int pacManLives = 3;

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioFrightened;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject i in objects)
        {
            Vector2 pos = i.transform.position;

            if(i.name != "PacMan" && i.name != "Nodes" && i.name != "NonNodes" && i.name != "Maze" && i.name != "Pellets" && i.tag != "Ghost" && i.tag != "ghostHome")
            {
                if (i.GetComponent < Tile>() != null)
                {
                    if(i.GetComponent < Tile>().isPellet || i.GetComponent <Tile>().isSuperPellet){

                        totalPellets++;
                    }
                }

                board[(int)pos.x, (int)pos.y] = i;
            }
            else
            {
                Debug.Log("Pacman at: " + pos);
            }
        }
    }

    public void Restart()
    {

        pacManLives -= 1;

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().Restart();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().Restart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
