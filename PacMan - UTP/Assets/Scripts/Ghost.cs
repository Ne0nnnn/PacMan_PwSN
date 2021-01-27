using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum Mode
    {
        Chase,
        Scatter,
        Frightened
    }

    public enum GhostType
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;

    public float moveSpeed = 3.9f;

    public float ghostRealeaseTimer = 0;
    public int pinkRealeaseTimer = 5;
    public bool isInGhostHouse;

    public Node startingPosition;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    private GameObject pacMan;

    private Node currentNode, targetNode, previousNode;
    private Vector2 direction, nextDirection;

    // Start is called before the first frame update
    void Start()
    {
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        Node node = GetNodeAtPosition(transform.position);

        if (node != null)
        {
            currentNode = node;
        }

        if (isInGhostHouse)
        {
            direction = Vector2.up;
            targetNode = currentNode.neighbors[0];
        }
        else
        {
            direction = Vector2.left;
            targetNode = ChooseNextNode();
        }
        previousNode = currentNode;
        
    }

    // Update is called once per frame
    void Update()
    {
        ModeUpdate();
        Move();
        RealeaseGhosts();
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            if (tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>();
            }
        }

        return null;
    }

    GameObject GetPortal (Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            if (tile.GetComponent<Tile>().isPortal)
            {
                GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                return otherPortal;
            }
        }

        return null;
    }

    float LenghtFromNode (Vector2 targetPostion)
    {
        Vector2 v = targetPostion - (Vector2)previousNode.transform.position;
        return v.sqrMagnitude;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LenghtFromNode(targetNode.transform.position);
        float nodeToSelf = LenghtFromNode(transform.position);

        return nodeToSelf > nodeToTarget;
    }

    float GetDistance(Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        return Mathf.Sqrt((dx * dx) + (dy * dy));
    }

    void ChangeMode (Mode m)
    {
        previousMode = currentMode;
        currentMode = m;
    }

    Vector2 GetRedGhostTargetTile()
    {
        Vector2 pacManPosition = pacMan.transform.position;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y)); //tam gdzie pac man

        return targetTile;
    }

    Vector2 GetPinkGhostTargetTile()
    {
        Vector2 pacManPosition = pacMan.transform.position;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (4 * pacManOrientation);  //4 kroki przed pac manem

        return targetTile;
    }

    Vector2 GetTargetTile()
    {
        Vector2 targetTile = Vector2.zero;
        if (ghostType == GhostType.Red)
        {
            targetTile = GetRedGhostTargetTile();
        }
        else if(ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }

        return targetTile;
    }

    Node ChooseNextNode()  //nastepne skrzyzowanie
    {
        Vector2 targetTile = Vector2.zero;

        targetTile = GetTargetTile();

        Node moveToNode = null;

        Node[] foundNodes = new Node[4];
        Vector2[] foundNodesDirection = new Vector2[4];

        int nodeCounter = 0;

        for(int i=0; i<currentNode.neighbors.Length; i++) 
        {

        if(currentNode.validDirections[i] != direction * -1)
            {
                foundNodes[nodeCounter] = currentNode.neighbors[i];
                foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                nodeCounter++;
            }
        }

        if(foundNodes.Length == 1)
        {
            moveToNode = foundNodes[0];
            direction = foundNodesDirection[0];
        }
        if(foundNodes.Length > 1)
        {
            float leastDistance = 1000000f;

            for(int i=0; i<foundNodes.Length; i++)
            {
                if(foundNodesDirection[i]!= Vector2.zero)
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);

                    if(distance < leastDistance)
                    {
                        leastDistance = distance;
                        moveToNode = foundNodes[i];
                        direction = foundNodesDirection[i];
                    }
                }
            }
        }

        return moveToNode;
    }

    void RealeasePinkGhost()
    {
        if(ghostType == GhostType.Pink && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void RealeaseGhosts()
    {
        ghostRealeaseTimer += Time.deltaTime;
        if(ghostRealeaseTimer > pinkRealeaseTimer)
        {
            RealeasePinkGhost();
        }
    }
    void Move()
    {
        if(targetNode != currentNode && targetNode != null && !isInGhostHouse)
        {
            if (OverShotTarget())
            {

                currentNode = targetNode;

                transform.position = currentNode.transform.position;

                GameObject otherPortal = GetPortal(currentNode.transform.position);
                if (otherPortal != null)
                {
                    transform.position = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
                }

                targetNode = ChooseNextNode();
                previousNode = currentNode;
                currentNode = null;
            }
            else
            {
                transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    void ModeUpdate()
    {
        if(currentMode!= Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;
            if(modeChangeIteration == 1)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 4)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
            }
        }
        else if (currentMode == Mode.Frightened)
        {

        }
    }
}
