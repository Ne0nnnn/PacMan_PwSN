﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{

    private static int boardWidth = 28;
    private static int boardHeight = 36;

    private bool didStartDeath = false;

    public int totalPellets = 0;
    public int score = 0;
    public int pacManLives = 3;

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioFrightened;
    public AudioClip backgroundAudioPacManDeath;

    public Text playerText;
    public Text readyText;


    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject i in objects)
        {
            Vector2 pos = i.transform.position;

            if(i.name != "PacMan" && i.name != "Nodes" && i.name != "NonNodes" && i.name != "Maze" && i.name != "Pellets" && i.tag != "Ghost" && i.tag != "ghostHome" && i.name != "Canvas" && i.name != "PlayerText" && i.name != "ReadyText")
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

        StartGame();
    }

    public void StartGame()
    {
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
            ghost.transform.GetComponent<Ghost>().canMove = false;
        }
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.transform.GetComponent<PacMan>().canMove = false;


        StartCoroutine(ShowObjectsAfter(2.25f));
    }

    IEnumerator ShowObjectsAfter (float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            
        }
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        playerText.transform.GetComponent<Text>().enabled = false;
        
        StartCoroutine(StartGameAfter(2));
    }

    IEnumerator StartGameAfter(float delay)
    {

        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;

        }
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().canMove = true;

        readyText.transform.GetComponent<Text>().enabled = false;

        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();
    }

    public void StartDeath()
    {
        if (!didStartDeath)
        {
            didStartDeath = true;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
            foreach (GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            pacMan.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessDeathAfter(2));   
        }
    }

    IEnumerator ProcessDeathAfter (float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(ProcessDeathAnimation(1.9f));

    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.localScale = new Vector3(1, 1, 1);
        pacMan.transform.localRotation = Quaternion.Euler(0, 0, 0);

        pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().deathAnimation;
        pacMan.transform.GetComponent<Animator>().enabled = true;

        transform.GetComponent<AudioSource>().clip = backgroundAudioPacManDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(1));
    } 
    IEnumerator ProcessRestart(float delay)
    {

        playerText.transform.GetComponent<Text>().enabled = true;
        readyText.transform.GetComponent<Text>().enabled = true;

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        transform.GetComponent<AudioSource>().Stop(); 

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestartShowObjects(1));

      
    }
    IEnumerator ProcessRestartShowObjects (float delay)
    {
        playerText.transform.GetComponent<Text>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<Animator>().enabled = false;
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();
        yield return new WaitForSeconds(delay);
        


        Restart();
    }

    public void Restart()
    {

        readyText.transform.GetComponent<Text>().enabled = false;
        pacManLives -= 1;

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().Restart();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().Restart();
        }

        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();
        didStartDeath = false;
    }

}
