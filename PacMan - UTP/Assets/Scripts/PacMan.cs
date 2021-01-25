using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    public float speed = 4.0f;

    private Vector2 direction = Vector2.zero;

    private Node currentNode;

    // Start is called before the first frame update
    void Start()
    {
        Node node = getNodeAtPosition(transform.localPosition);

        if (node != null)
        {
            currentNode = node;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        UpdateOrientation();
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2.left;
            MoveToNode(direction);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2.right;
            MoveToNode(direction);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            direction = Vector2.up;
            MoveToNode(direction);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2.down;
            MoveToNode(direction);
        }
    }

    void Move()
    {
        transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
    }

    void MoveToNode(Vector2 v)
    {
        Node moveToNode = CanMove(v);

        if (moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    void UpdateOrientation()
    {
        if(direction == Vector2.left) {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            var newScale = transform.localScale;
            newScale.x = -1;

            transform.localScale = newScale;
        }
        else if (direction == Vector2.right) {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            var newScale = transform.localScale;
            newScale.x = 1;

            transform.localScale = newScale;
        }
        else if (direction == Vector2.up) {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.down) {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }

    Node getNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }
        else
        {
            return null;
        }
    }

    Node CanMove (Vector2 v)
    {
        Node moveToNode = null;
        
        for(int i=0; i<currentNode.neighbors.Length; i++)
        {
            if(currentNode.validDirections[i] == v)
            {
                moveToNode = currentNode.neighbors[i];
                break;
            }
        }

        return moveToNode;
    }
}
