using UnityEngine;
using System.Collections;
using Completed;

public class endButtonOnClick : MonoBehaviour {

    public GameManager gm;

	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}

    public void endturn()
    {
		gm.endTurnConfirm();
    }

	public void confirmYes()
	{
		gm.confirmYes();
	}

	public void confirmNo()
	{
		gm.confirmNo();
	}

	// Update is called once per frame
	void Update () {
	
	}

    public void menu()
    {
		gm.endGameConfirm();
        //Application.LoadLevel("Menu");
    }
    public void exit()
    {
        Application.Quit();
    }

}
