using UnityEngine;
using System.Collections;
using Completed;

public class AISelectorControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnLevelWasLoaded()
	{
		GameObject human = GameObject.Find("Human");
		Debug.Log("mode = " + BoardManager.mode);
		if (BoardManager.mode == 2)
			human.SetActive(false);
		else
			human.SetActive(true);
	}
}
