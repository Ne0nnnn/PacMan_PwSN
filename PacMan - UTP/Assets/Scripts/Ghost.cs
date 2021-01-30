using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum Mode
    {
        Chase,
        Scatter,
        Frightened,
        Consumed
    }

    public enum GhostType
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;

    public float moveSpeed = 5.9f;
    public float normalMoveSpeed = 5.9f;
    public float frigtendedMoveSpeed = 2.9f;
    private float previousMoveSpeed;
    public float consumedMovedSpeed = 15f;

    public bool canMove = true;

    public float ghostReleaseTimer = 0;
    public int pinkReleaseTimer = 5;
    public int inkyReleaseTimer = 14;
    public int clydeReleaseTimer = 21;

    public int frightendedModeDuration = 10;
    public int startBlinkingAt = 7;

    public bool isInGhostHouse;

    public Node startingPosition;
    public Node homeNode;
    public Node ghostHouse;

    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostWhite;
    public RuntimeAnimatorController ghostBlue;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    private AudioSource backgroundAudio;

    private float frightendedModeTimer = 0;
    private float blinkTimer = 0;
    private bool frightendedModeisWhite = false;

    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    private GameObject pacMan;

    private Node currentNode, targetNode, previousNode;
    private Vector2 direction, nextDirection;

    // Start is called before the first frame update
    void Start()
    {

        backgroundAudio = GameObject.Find("Game").transform.GetComponent<AudioSource>();
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

        UpdateAnimatorController();

    }


    public void MoveToStartingPosition()
    {
        if (transform.name != "Ghost_Blinky")
            isInGhostHouse = true;

        transform.position = startingPosition.transform.position;

        if (isInGhostHouse)
        {
            direction = Vector2.up;
        }
        else
        {
            direction = Vector2.left;
        }
        UpdateAnimatorController();
    }

    public void Restart()
    {

        canMove = true;

        //transform.GetComponent<SpriteRenderer>().enabled = true;

        currentMode = Mode.Scatter;
        moveSpeed = normalMoveSpeed;
        previousMoveSpeed = 0;


        ghostReleaseTimer = 0;
        modeChangeIteration = 1;
        modeChangeTimer = 0;

        if (transform.name != "Ghost_Blinky")
            isInGhostHouse = true;

        currentNode = startingPosition;

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
        UpdateAnimatorController();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            ModeUpdate();
            Move();
            ReleaseGhosts();
            CheckCollision();
            CheckIsInGhostHouse();
        }
       
    }

    void CheckIsInGhostHouse()
    {
        if (currentMode == Mode.Consumed) 
        {
            GameObject tile = GetTileAtPosition(transform.position);
            if(tile !=null)
            {
                if(tile.transform.GetComponent<Tile>() !=null)
                {
                    if (tile.transform.GetComponent<Tile>().isGhostHouse)
                    {
                        moveSpeed = normalMoveSpeed;
                        Node node = GetNodeAtPosition(transform.position);

                        if(node != null)
                        {
                            currentNode = node;

                            direction = Vector2.up;
                            targetNode = currentNode.neighbors[0];

                            previousNode = currentNode;
                            currentMode = Mode.Chase;

                            UpdateAnimatorController();
                        }
                    }
                }
            }
        }
    }

    void CheckCollision()
    {
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4); //obiekt kolizji dla duchow - 4 razy mniejszy niz grafika
        Rect pacManRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);

        if (ghostRect.Overlaps(pacManRect))
        {
            if (currentMode == Mode.Frightened)
            {
                Consumed();
            }
            else
            {
                if(currentMode != Mode.Consumed)
                {
                    GameObject.Find("Game").transform.GetComponent<GameBoard>().StartDeath();
                }
                
            }

        }
    }

    void Consumed() {

        currentMode = Mode.Consumed;
        previousMoveSpeed = moveSpeed;
        moveSpeed = consumedMovedSpeed;
        UpdateAnimatorController();

    }


    void UpdateAnimatorController()
    {
        if (currentMode != Mode.Frightened && currentMode != Mode.Consumed)
        {
            if (direction == Vector2.up)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
            }
            else if (direction == Vector2.down)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
            }
            else if (direction == Vector2.left)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }
            else if (direction == Vector2.right)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
            }
            else
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }
        }

        else if (currentMode == Mode.Frightened)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
        }
        else if (currentMode == Mode.Consumed)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = null;

            if (direction == Vector2.up)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            } else if (direction == Vector2.down)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
            else if (direction == Vector2.left)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }
            else if (direction == Vector2.right)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
        }


    }
    Node GetNodeAtPosition(Vector2 pos)
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

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);            
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").transform.GetComponent<GameBoard>().board[tileX, tileY];

        if (tile != null)
            return tile;
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
        if (currentMode == Mode.Frightened)
        {
            moveSpeed = previousMoveSpeed;
        }

        if (m == Mode.Frightened)
        {
            previousMoveSpeed = moveSpeed;
            moveSpeed = frigtendedMoveSpeed;
        }

        if (currentMode != m)
        {
            previousMode = currentMode;
            currentMode = m;
        }

        UpdateAnimatorController();
    }

    public void StartFrightenedMode()
    {
        if(currentMode != Mode.Consumed)
        {
            frightendedModeTimer = 0;
            backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudioFrightened;
            backgroundAudio.Play();
            ChangeMode(Mode.Frightened);
        }

       
    }

    Vector2 GetRedGhostTargetTile()
    {
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y)); //tam gdzie pac man
        return targetTile;
    }

    Vector2 GetPinkGhostTargetTile()
    {
        
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (4 * pacManOrientation);  //4 kroki przed pac manem

        return targetTile;

    }

    Vector2 GetBlueGhostTargetTile()
    {
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (2 * pacManOrientation); //2 kroki przed pac manem

        Vector2 tempBlinkyPosition = GameObject.Find("Ghost_Blinky").transform.localPosition;
        int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPosition.x);
        int blinkyPositionY= Mathf.RoundToInt(tempBlinkyPosition.y);

        tempBlinkyPosition = new Vector2(blinkyPositionX, blinkyPositionY);

        float distance = GetDistance(tempBlinkyPosition, targetTile);
        distance *= 2;

        targetTile = new Vector2(tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance);

        return targetTile;

    }

    Vector2 GetOrangeGhostTargetTile()
    {
        Vector2 pacManPosition = pacMan.transform.localPosition;
        float distance = GetDistance(transform.localPosition, pacManPosition);
        Vector2 targetTile = Vector2.zero;

        if(distance > 8)
        {
            targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));

        }
        else if(distance < 8)
        {
            targetTile = homeNode.transform.position;
        }

        return targetTile;
    }


    Vector2 GetTargetTile()
    {
        Vector2 targetTile = Vector2.zero;

        if (ghostType == GhostType.Red)
        {
            targetTile = GetRedGhostTargetTile();
        }
        else if (ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        else if (ghostType == GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }
        else if (ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }

        return targetTile;
    }   

    Vector2 GetRandomTile()
    {
        int x = Random.Range(0, 28);
        int y = Random.Range(0, 36);

        return new Vector2(x, y);
    }

    Node ChooseNextNode()  //nastepne skrzyzowanie
    {
        Vector2 targetTile = Vector2.zero;

        if (currentMode == Mode.Chase)
        {
            targetTile = GetTargetTile();
        } else if (currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }
        else if (currentMode == Mode.Frightened)
        {
            targetTile = GetRandomTile();
        }
        else if (currentMode == Mode.Consumed)
        {
            targetTile = ghostHouse.transform.position;
        }



        Node moveToNode = null;

        Node[] foundNodes = new Node[4];
        Vector2[] foundNodesDirection = new Vector2[4];

        int nodeCounter = 0;

        for(int i=0; i<currentNode.neighbors.Length; i++) 
        {

        if(currentNode.validDirections[i] != direction * -1)
            {
                if (currentMode != Mode.Consumed)
                {
                    GameObject tile = GetTileAtPosition(currentNode.transform.position);
                    if (tile.transform.GetComponent<Tile>().isGhostHouseEntrance == true)
                    {
                        if (currentNode.validDirections[i] != Vector2.down)
                        {
                            foundNodes[nodeCounter] = currentNode.neighbors[i];
                            foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                            nodeCounter++;

                        }
                    }
                    else
                    {
                        foundNodes[nodeCounter] = currentNode.neighbors[i];
                        foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                        nodeCounter++;
                    }
                }
                else
                {
                    foundNodes[nodeCounter] = currentNode.neighbors[i];
                    foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                    nodeCounter++;
                }
                
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

    void ReleasePinkGhost()
    {
        if (ghostType == GhostType.Pink && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseBlueGhost()
    {
        if (ghostType == GhostType.Blue && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Orange && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;

        if (ghostReleaseTimer > pinkReleaseTimer)
            ReleasePinkGhost();
        if (ghostReleaseTimer > inkyReleaseTimer)
            ReleaseBlueGhost();
        if (ghostReleaseTimer > clydeReleaseTimer)
            ReleaseOrangeGhost();


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

                UpdateAnimatorController();
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
            frightendedModeTimer += Time.deltaTime;

            if(frightendedModeTimer >= frightendedModeDuration)
            {

                backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudioNormal;
                backgroundAudio.Play();

                frightendedModeTimer = 0;
                ChangeMode(previousMode);
            }

            if(frightendedModeTimer >= startBlinkingAt)
            {
                blinkTimer += Time.deltaTime;
                if(blinkTimer >= 0.1f)
                {
                    blinkTimer = 0f;

                    if (frightendedModeisWhite)
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
                        frightendedModeisWhite = false;
                    }
                    else
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostWhite;
                        frightendedModeisWhite = true;
                    }
                }
            }
        }
    }
}
