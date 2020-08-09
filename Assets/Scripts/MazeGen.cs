using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* Grey  -> path available
 * Black -> wall
 * Blue  -> start
 * Cyan  -> end
 * White -> nothing (before gen maze)
 */


public class MazeGen : MonoBehaviour
{
    private const float RATIO_W_CAM = (float)5.5;
    private const float RATIO_H_CAM = (float)3;


    public GameObject squareInstance;
    public GameObject horizontalWallInstance;
    public GameObject verticalWallInstance;
    public GameObject camera;

    private int width;
    private int height;
    private Dictionary<Vector2, GameObject> board = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, GameObject> edges = new Dictionary<Vector2, GameObject>();
    private Vector2 endCoords;

    private ArrayList directions = new ArrayList(4);


    public void StartGame(int width, int height) {
        this.width = width;
        this.height = height;

        float camZH = (float)(height / RATIO_H_CAM);
        float camZW = (float)(width / RATIO_W_CAM);
        float camX = (float)0.3 * (width - 1);
        float camY = (float)0.3 * (height - 1);

        camera.transform.position = new Vector3(camX, camY, -1);

        if (camZW < camZH) {
            camera.GetComponent<Camera>().orthographicSize = camZH;
        }
        else {
            camera.GetComponent<Camera>().orthographicSize = camZW;
        }


        directions.Add(new Vector2Int(0, 1));   //up
        directions.Add(new Vector2Int(0, -1));  //down
        directions.Add(new Vector2Int(1, 0));   //right
        directions.Add(new Vector2Int(-1, 0));  //left


        genEmptyBoard();

        Vector2Int startCoords = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        board[startCoords].GetComponent<SpriteRenderer>().color = Color.yellow;
        genMaze(startCoords);

        //END POINT
        endCoords = new Vector2((int)(width / 2), (int)(height / 2));
        board[endCoords].GetComponent<SpriteRenderer>().color = Color.cyan;


    }



    private void genEmptyBoard() {
        for (int i = -1; i < width; i++) {
            for (int j = -1; j < height; j++) {
                Vector2 coords = new Vector2(i, j);

                if (j > -1 && i > -1) {
                    //Initiate position and game object
                    board[coords] = Instantiate<GameObject>(squareInstance);
                    board[coords].GetComponent<Transform>().position = new Vector3((float)0.6 * i, (float)0.6 * j, board[coords].GetComponent<Transform>().position.z);

                    //Initiate squares
                    board[coords].GetComponent<SpriteRenderer>().color = Color.white;

                }

                //Initiate walls
                if (i < width && j < height) {
                    //horizontal wall
                    if (i > -1) {
                        Vector2 edgesCoords = new Vector2(coords.x, (float)0.5 + coords.y);

                        edges[edgesCoords] = Instantiate<GameObject>(horizontalWallInstance);
                        edges[edgesCoords].GetComponent<Transform>().position = new Vector3((float)(0.6 * i), (float)((0.6 * j) + 0.3), edges[edgesCoords].GetComponent<Transform>().position.z - 0.1f);
                    }

                    //vertical wall
                    if (j > -1) {
                        Vector2 edgesCoords = new Vector2((float)0.5 + coords.x, coords.y);

                        edges[edgesCoords] = Instantiate<GameObject>(verticalWallInstance);
                        edges[edgesCoords].GetComponent<Transform>().position = new Vector3((float)((0.6 * i) + 0.3), (float)(0.6 * j), edges[edgesCoords].GetComponent<Transform>().position .z - 0.1f);
                    }
                }
            }
        }
    }

    private void genMaze(Vector2Int startCoords) {


        //GROWING TREE ALGORITHM


        //PARTIE A OPTIMISER VRAIMENT (BESOIN DES 4 DIRECTIONS DANS UN ORDRE ALEATOIRE)
        List<int> directionsTryingOrder = new List<int>();

        while ( !(directionsTryingOrder.Count == 4 && directionsTryingOrder.Contains(0) && directionsTryingOrder.Contains(1) && directionsTryingOrder.Contains(2) && directionsTryingOrder.Contains(3))) {
            directionsTryingOrder = new List<int>();
            
            directionsTryingOrder.Add((int)UnityEngine.Random.Range(0, 4));
            directionsTryingOrder.Add((int)UnityEngine.Random.Range(0, 4));
            directionsTryingOrder.Add((int)UnityEngine.Random.Range(0, 4));
            directionsTryingOrder.Add((int)UnityEngine.Random.Range(0, 4));

        }

        Vector2Int nextCoords;

        foreach(int it in directionsTryingOrder) {
            nextCoords = startCoords + (Vector2Int)directions[it];

            if (board.ContainsKey(nextCoords)) {
                if(board[nextCoords].GetComponent<SpriteRenderer>().color != Color.yellow) {
                    //Valid next square
                    board[nextCoords].GetComponent<SpriteRenderer>().color = Color.yellow;

                    //remove edge between path
                    Vector2 edgeToRemove = ((Vector2)nextCoords + (Vector2)startCoords) / 2;
                    edges[edgeToRemove].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

                    //recursive call
                    genMaze(nextCoords);
                }
            }
        }

    }

    public Dictionary<Vector2, GameObject> getBoard() {
        return board;
    }
    public Dictionary<Vector2, GameObject> getEdges() {
        return edges;
    }

    public Vector2 getEndCoord() {
        return endCoords;
    }

    public int getHeight() {
        return height;
    }
    public int getWidth() {
        return width;
    }
}
