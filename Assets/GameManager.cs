using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public float gameTime;
    public float remainingTime;
    public int redOreValue, blueOreValue, greenOreValue;
    public int score;

    public DropOff dropOff;

    void Awake() {
        remainingTime = gameTime * 60;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        remainingTime -= Time.deltaTime;
		
	}

    public void UpdateScore() {
        score = dropOff.redOre * redOreValue + dropOff.blueOre * blueOreValue + dropOff.greenOre * greenOreValue;
        Player player = GetComponent<Player>();
        player.ui.scoreText.text = score.ToString("0");
    }
}
