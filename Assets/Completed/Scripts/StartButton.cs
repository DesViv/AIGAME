using UnityEngine;
using System.Collections;

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

    public void StartGame(string name)
    {
        Application.LoadLevel(name);
    }
}
