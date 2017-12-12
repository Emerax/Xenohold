using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float remainingTime;
    public int redOreValue, blueOreValue, greenOreValue;
    public int score;
    public UI ui;

    public DropOff dropOff;

    private bool started = false;
	
	// Update is called once per frame
	void Update () {
        remainingTime -= Time.deltaTime;
        if(started && remainingTime <= 0) {
            EndGame("Time up!");
        }
		
	}

    public void UpdateScore() {
        score = dropOff.redOre * redOreValue + dropOff.blueOre * blueOreValue + dropOff.greenOre * greenOreValue;
        Player player = GetComponent<Player>();
        player.ui.scoreText.text = score.ToString("0");
    }

    public void StartGame(int gameTime) {
        remainingTime = gameTime * 60;
        started = true;
    }

    public void EndGame(string cause) {
        ui.CloseUI();
        ui.OpenEndScreen(cause);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
