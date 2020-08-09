using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Runner : MonoBehaviour {
    private const int UP = 0;
    private const int DOWN = 1;
    private const int RIGHT = 2;
    private const int LEFT = 3;

    public GameObject runnerSprite;
    public GameManager gameManager;
    public MazeGen maze;
    public float timeBtwSquare;
    public Vector2 offset;
    public GameObject spriteNo0;
    public GameObject spriteNo1;
    public GameObject spriteNo2;
    public GameObject spriteNo3;
    public GameObject spriteNo4;
    public GameObject spriteNo5;
    public GameObject spriteNo6;
    public GameObject spriteNo7;
    public GameObject spriteNo8;
    public GameObject spriteNo9;

    private Dictionary<Vector2, GameObject> board;
    private Dictionary<Vector2, GameObject> edges;
    private Stack<Position> pathStack;
    private int tag;
    private bool firstUpdate = true;
    private bool isMoving = false;
    private bool isRunning = false;
    private Animator animator;
    private Vector3 tagOffsetNumber1;
    private Vector3 tagOffsetNumber2;
    private GameObject number1;
    private GameObject number2;
    private bool selected = false;
    private bool clickable = true;
    private Vector2 startCoords;


    private void Start() {

        animator = GetComponent<Animator>();
        tagOffsetNumber1 = new Vector3(-0.35f, 0.3f, -0.1f);
        tagOffsetNumber2 = new Vector3(-0.2f, 0.3f, -0.1f);
    }

    private void OnDestroy() {
        
        if(number1)
            Destroy(number1);
        if (number2)
            Destroy(number2);
    }

    public void StartRun(Vector2 startCoords) {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 101);
        this.startCoords = startCoords;

        isRunning = true;
        board = maze.getBoard();
        edges = maze.getEdges();

        //place runner at start point
        runnerSprite.transform.position = new Vector3(startCoords.x * (float)0.6 + offset.x, startCoords.y * (float)0.6+ offset.y, runnerSprite.transform.position.z);

        pathStack = new Stack<Position>();
        pathStack.Push(new Position(startCoords.x, startCoords.y));

        //InvokeRepeating("goFoward", (float)0.0, (float)1.0 / speedPerSec);
        

    }

    public void stopRun() {
        isRunning = false;
        animator.SetInteger("direction", 1);
    }


    IEnumerator moveToX(Transform fromPosition, Vector3 toPosition, float duration) {
        Vector2 diff = new Vector2(toPosition.x, toPosition.y) - new Vector2(fromPosition.position.x, fromPosition.position.y);

        if(diff.x > 0) { 
            animator.SetInteger("direction", RIGHT);
        }else if(diff.x < 0) {
            animator.SetInteger("direction", LEFT);
        }
        else if(diff.y > 0) {
            animator.SetInteger("direction", UP);
        }
        else {
            animator.SetInteger("direction", DOWN);
        }

        //Make sure there is only one instance of this function running
        if (isMoving) {
            yield break; ///exit if this is still running
        }
        isMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        Vector3 startPos = fromPosition.position;

        while (counter < duration) {
            counter += Time.deltaTime;
            fromPosition.position = Vector3.Lerp(startPos, toPosition, counter / duration);
            yield return null;
        }

        isMoving = false;
    }

    // Update is called once per frame
    void Update() {
        if (isRunning) {
            if (firstUpdate) {
                firstUpdate = false;
                getDirections();
            }
            // Résolution...
            if (!isMoving) {
                if (pathStack.Peek().pos != maze.getEndCoord()) {
                    switch (pathStack.Peek().getNumberOfDirections()) {
                        //auncune possibiliter
                        case 0:
                            //condition pour que le programme ne plante pas lorsquil n'a aucun chemin possible
                            if (pathStack.Peek().pos != startCoords)
                                pathStack.Pop();

                            break;
                        //une seule possibiliter
                        case 1:
                            if ((bool)pathStack.Peek().direction[UP])
                                moveUp();
                            else {
                                if ((bool)pathStack.Peek().direction[DOWN])
                                    moveDown();
                                else {
                                    if ((bool)pathStack.Peek().direction[RIGHT])
                                        moveRight();
                                    else {
                                        if ((bool)pathStack.Peek().direction[LEFT])
                                            moveLeft();
                                    }
                                }
                            }
                            break;
                        //plusieurs possibiliter
                        default:
                            int random = Random.Range(0, 4);

                            while (!(bool)pathStack.Peek().direction[random])
                                random = random = Random.Range(0, 4);

                            switch (random) {
                                case UP:
                                    moveUp();
                                    break;
                                case DOWN:
                                    moveDown();
                                    break;
                                case RIGHT:
                                    moveRight();
                                    break;
                                case LEFT:
                                    moveLeft();
                                    break;
                            }


                            break;
                    }
                }
                else {
                    //Runner won the race
                    gameManager.claimWin(tag);
                }
                StartCoroutine(moveToX(runnerSprite.transform, new Vector3(pathStack.Peek().pos.x * (float)0.6 + offset.x, pathStack.Peek().pos.y * (float)0.6 + offset.y, runnerSprite.transform.position.z), timeBtwSquare));
            }
        }

        if (number1)
            number1.transform.position = transform.position + tagOffsetNumber1;
        if(number2)
            number2.transform.position = transform.position + tagOffsetNumber2;
    }



    //verifier les directions possible
    void getDirections() {
        //new Color(0, 0, 0, 0) -> invisible

        if (edges[pathStack.Peek().upEdgePos()].GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 0)) {
            pathStack.Peek().direction[UP] = true;
        }

        if (edges[pathStack.Peek().downEdgePos()].GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 0)) {
            pathStack.Peek().direction[DOWN] = true;
        }

        if (edges[pathStack.Peek().rightEdgePos()].GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 0)) {
            pathStack.Peek().direction[RIGHT] = true;
        }

        if (edges[pathStack.Peek().leftEdgePos()].GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 0)) {
            pathStack.Peek().direction[LEFT] = true;
        }

    }



    void moveUp() {
        pathStack.Peek().direction[UP] = false;
        pathStack.Push(new Position(pathStack.Peek().pos.x, pathStack.Peek().pos.y + 1));
        getDirections();
        pathStack.Peek().direction[DOWN] = false;
    }
    void moveDown() {
        pathStack.Peek().direction[DOWN] = false;
        pathStack.Push(new Position(pathStack.Peek().pos.x, pathStack.Peek().pos.y - 1));
        getDirections();
        pathStack.Peek().direction[UP] = false;
    }
    void moveRight() {
        pathStack.Peek().direction[RIGHT] = false;
        pathStack.Push(new Position(pathStack.Peek().pos.x + 1, pathStack.Peek().pos.y));
        getDirections();
        pathStack.Peek().direction[LEFT] = false;
    }
    void moveLeft() {
        pathStack.Peek().direction[LEFT] = false;
        pathStack.Push(new Position(pathStack.Peek().pos.x - 1, pathStack.Peek().pos.y));
        getDirections();
        pathStack.Peek().direction[RIGHT] = false;
    }

    private void OnMouseDown() {
        if (clickable) {
            Collider2D coll = GetComponent<Collider2D>();

            

            if (coll.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
                gameManager.betOn(tag);
                selected = true;

                if (number1)
                    number1.GetComponent<SpriteRenderer>().color = new Color32(255, 100, 0, 255); //orange
                if (number2)
                    number2.GetComponent<SpriteRenderer>().color = new Color32(255, 100, 0, 255); //orange
            }
        }
    }

    public bool isSelected() {
        return selected;
    }
    public void unselect(){
        this.selected = false;
        if (number1)
            number1.GetComponent<SpriteRenderer>().color = new Color32(0, 248, 148, 255); //green
        if (number2)
            number2.GetComponent<SpriteRenderer>().color = new Color32(0, 248, 148, 255); //green
    }
    public void setClickable(bool clickable) {
        this.clickable = clickable;
    }
    public int getTag() {
        return tag;
    }

    public void setTag(string tagNumber) {
        tag = Convert.ToInt32(tagNumber);

        int firstNb = Convert.ToInt32(tagNumber[0].ToString());

        switch (firstNb) {
            case 1:
                number1 = Instantiate(spriteNo1);
                break;
            case 2:
                number1 = Instantiate(spriteNo2);
                break;
            case 3:
                number1 = Instantiate(spriteNo3);
                break;
            case 4:
                number1 = Instantiate(spriteNo4);
                break;
            case 5:
                number1 = Instantiate(spriteNo5);
                break;
            case 6:
                number1 = Instantiate(spriteNo6);
                break;
            case 7:
                number1 = Instantiate(spriteNo7);
                break;
            case 8:
                number1 = Instantiate(spriteNo8);
                break;
            case 9:
                number1 = Instantiate(spriteNo9);
                break;
        }

        if(tagNumber.Length > 1) {
            int secondNb = Convert.ToInt32(tagNumber[1].ToString());

            switch (secondNb) {
                case 0:
                    number2 = Instantiate(spriteNo0);
                    break;
                case 1:
                    number2 = Instantiate(spriteNo1);
                    break;
                case 2:
                    number2 = Instantiate(spriteNo2);
                    break;
                case 3:
                    number2 = Instantiate(spriteNo3);
                    break;
                case 4:
                    number2 = Instantiate(spriteNo4);
                    break;
                case 5:
                    number2 = Instantiate(spriteNo5);
                    break;
                case 6:
                    number2 = Instantiate(spriteNo6);
                    break;
                case 7:
                    number2 = Instantiate(spriteNo7);
                    break;
                case 8:
                    number2 = Instantiate(spriteNo8);
                    break;
                case 9:
                    number2 = Instantiate(spriteNo9);
                    break;
            }
        }
    }
}

