using UnityEngine;
using System.Collections;
using Completed;

public class StartButton : MonoBehaviour {

    // Use this for initialization
    void Start () {
    
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void StartGame(int index)
    {
        Application.LoadLevel(index);
    }

    public void StartGamePvP(string name)
    {
        BoardManager.mode = 0;
        Application.LoadLevel(name);
    }
    public void StartGamePvE(string name)
    {
        BoardManager.mode = 1;
        Application.LoadLevel(name);
    }
    public void StartGameEvE(string name)
    {
        BoardManager.mode = 2;
        Application.LoadLevel(name);
    }
}
