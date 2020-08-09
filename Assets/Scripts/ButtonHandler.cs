using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class ButtonHandler : MonoBehaviour
{

    //Call to change scene
    public void toNextScene(string sceneName) {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void addRunner(TextMeshProUGUI nb) {
        int intNb = Convert.ToInt32(nb.text);
        if(intNb < 32)
            intNb++;
        nb.text = intNb.ToString();
    }

    public void removeRunner(TextMeshProUGUI nb) {
        int intNb = Convert.ToInt32(nb.text);
        if(intNb > 3)
            intNb--;
        nb.text = intNb.ToString();
    }
}
