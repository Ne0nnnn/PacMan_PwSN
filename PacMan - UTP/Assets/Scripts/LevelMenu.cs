using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public static bool isOnePlayerGame = true;

    public Text level1;
    public Text level2;
    public Text selector;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (!isOnePlayerGame)
            {
                isOnePlayerGame = true;
                selector.transform.localPosition = new Vector3(selector.transform.localPosition.x, level1.transform.localPosition.y, selector.transform.localPosition.z);
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (isOnePlayerGame)
            {
                isOnePlayerGame = false;
                selector.transform.localPosition = new Vector3(selector.transform.localPosition.x, level2.transform.localPosition.y, selector.transform.localPosition.z);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            if(isOnePlayerGame)
            {
                isOnePlayerGame = true;
                SceneManager.LoadScene("Level1");
                Debug.Log("zaladowano1");
            }
            else if (!isOnePlayerGame)
            {
                isOnePlayerGame = false;
                SceneManager.LoadScene("Level2");
                Debug.Log("zaladowano2");
            }
           
        }
    }
}
