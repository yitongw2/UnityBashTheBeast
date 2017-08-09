using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
	public GameObject pauseButton, pausePanel;
	// Use this for initialization
	public void onPause(){
		pausePanel.SetActive(true);	

	}
	public void onUnPause(){
		pausePanel.SetActive(false);

	}
}
