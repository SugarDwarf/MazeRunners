using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject runnerInstance;
    public MazeGen mazeManager;
    public GameObject bettingMenuCanvas;
    public GameObject gameOverMenuCanvas;
    public TextMeshProUGUI runnerNbLabel;
    public GameObject inputFieldBet;
    public TextMeshProUGUI gameOverMessage;
    public TextMeshProUGUI prizeMultiplicatorLabel;
    public TextMeshProUGUI prizeLabel;
    public AudioSource music;
    public AudioClip cheer;
    public AudioClip missed;

    private List<GameObject> listOfRunners = new List<GameObject>();
    private int choosedRunnerTag;
    private float prizeMult;
    private float amountBet;

    private void addRunner() {
        if (listOfRunners.Count < 32) {
            GameObject runner = Instantiate<GameObject>(runnerInstance);
            runner.transform.position = new Vector3((float)(7.5 + ((listOfRunners.Count % 8) * 0.75)), (float)(6.75 - (((int)listOfRunners.Count / 8) * 0.75)), runner.transform.position.z - 0.2f);
            runner.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 1f, 1f, 1f, 1f);

            listOfRunners.Add(runner);
            runner.GetComponent<Runner>().setTag(listOfRunners.Count.ToString());

            //random speed variation
            runner.GetComponent<Runner>().timeBtwSquare = 0.25f - (Random.value * 0.05f);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        gameOverMenuCanvas.SetActive(false);
        choosedRunnerTag = -1;
    }

    // Update is called once per frame
    void Update()
    {
        int nbOfRunner = Convert.ToInt32(runnerNbLabel.text);
        if(nbOfRunner < listOfRunners.Count) {
            GameObject toDestroy = listOfRunners[listOfRunners.Count - 1].gameObject;
            listOfRunners.RemoveAt(listOfRunners.Count - 1);
            Destroy(toDestroy);

        }
        if(nbOfRunner > listOfRunners.Count) {
            addRunner();
        }

        prizeMult = (1f + (0.2f * listOfRunners.Count));
        prizeMultiplicatorLabel.text = "Prize: " + prizeMult.ToString() + "X \nyour bet";

    }

    public void startGame() {
        if (choosedRunnerTag > -1 && !string.IsNullOrEmpty(inputFieldBet.GetComponent<TMP_InputField>().text)) {
            music.Play();
            int k = (int)Math.Sqrt( listOfRunners.Count * 25);

            mazeManager.StartGame(2 * k, k);
            bettingMenuCanvas.SetActive(false);

            foreach (var item in listOfRunners) {
                //Random pos in corners rectangle of maze
                int x, y;
                y = Random.Range((int)0, 2) * (mazeManager.getHeight() - 1);
                x = Random.Range((int)0, 2) * (mazeManager.getWidth() - 1);


                item.GetComponent<Runner>().StartRun(new Vector2(x, y));
                item.GetComponent<Runner>().setClickable(false);
            }

            amountBet = Convert.ToInt32(inputFieldBet.GetComponent<TMP_InputField>().text);
        }
    }

    public void claimWin(int runnerTag) {
        music.Stop();

        if(runnerTag == choosedRunnerTag) {
            //player won
            gameOverMessage.text = "Runner #" + runnerTag.ToString() + " Win. \nYou win!";
            prizeLabel.text = "You've just won " + (prizeMult * amountBet).ToString()  +"!!!";

            music.PlayOneShot(cheer);
        }
        else {
            //player lost
            gameOverMessage.text = "Runner #" + runnerTag.ToString() + " Win. \nYou lose. :(";
            music.PlayOneShot(missed);

        }
        //Menu appear
        gameOverMenuCanvas.SetActive(true);

        foreach(var item in listOfRunners) {
            item.GetComponent<Runner>().stopRun();
        }
    }

    public void betOn(int tag) {
        choosedRunnerTag = tag;

        foreach (var item in listOfRunners) {
            item.GetComponent<Runner>().unselect();
        }
    }
}
