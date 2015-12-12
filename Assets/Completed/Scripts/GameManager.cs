﻿using UnityEngine;
using System.Collections;

namespace Completed
{
    using System.Collections.Generic;       // import Lists
    using UnityEngine.UI;                   // import UI

    public class GameManager : MonoBehaviour
    {
        public float levelStartDelay = 2f;                      // Time to wait before starting level, in seconds.
        public float turnDelay = 0.1f;                          // Delay between each Player turn.
        public int playerFoodPoints = 100;                      // Starting value for Player food points.
        public static GameManager instance = null;              // Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector]
        public int mode; //0 = PlayervPlayer, 1 = PlayervEnemy, 2 = EnemyvEnemy

		public GameObject ui_confirm;
        private GameObject gameoverText;
        public Text ui_confirmText;
		public enum MenuStates{END_TURN, MAIN_MENU};
		private MenuStates currState;

		private Text levelText;                                 // Text to display current level number.
		private GameObject levelImage;                          // Image to block out level as levels are being set up, background for levelText.
		private BoardManager boardScript;                       // Store a reference to our BoardManager which will set up the level.
		private int level = 1;                                  // Current level number, expressed in game as "Day 1".
		private bool doingSetup = true;                         // Boolean to check if we're setting up board, prevent Player from moving during setup.

		public int curTeam = 0;

		//Every time a Player is added to the game board, it will add itself to one of these lists based on the team it's on in start() of Player.cs
		public List<Player> bluePlayer;
		public List<Player> redPlayer;

		//Every time an Enemy is added to the game board, it will add itself to one of these lists based on the team it's on by calling AddEnemyToList in start() of Enemy.cs
		public List<Enemy> blueComp;
		public List<Enemy> redComp;

		public AIBase blueAI;
		public AIBase redAI;

		/*
		 *	Constructor. Always called before Start functions. Used to initialize.
		 */
        void Awake()
        {
            ListAI.initAI();
        }

		/*
		 *	Constructor. Not used.
		 */
		void Start()
		{
		}

		/*
		 *	Button callback; called when the "Yes" button in the confirmation menu is clicked.
		 *	Based on the current menu state, either end the turn or exit to the main menu.
		 */
		public void confirmYes()
		{
			switch (currState)
			{
				case MenuStates.END_TURN:
					endTurn();
				break;
				case MenuStates.MAIN_MENU:
					Application.LoadLevel("Menu");		// go to the Menu scene
				break;
			}
			ui_confirm.SetActive(false);				// hide the confirmation menu
		}

		/*
		 *	Button callback; called when the "Cancel" button in the confirmation menu is clicked.
		 *	Simply hide the confirmation menu.
		 */
		public void confirmNo()
		{
			ui_confirm.SetActive(false);				// hide the confirmation menu
		}

		/*
		 *	Button callback; called when the "End Turn" button is clicked.
		 *	Show the confirmation menu.
		 */
		public void endTurnConfirm()
		{
			// first set the prompt
			ui_confirmText.text = "Are you sure you want to end your turn?";
			currState = MenuStates.END_TURN;			// remember that the confirmation menu is dealing with end turn
			if (!ui_confirm.activeSelf)
			{
				ui_confirm.SetActive(true);				// show the confirmation menu
			}
		}

		/*
		 *	Button callback; called when the "Menu" button is clicked.
		 *	Show the confirmation menu.
		 */
		public void endGameConfirm()
		{
			// first set the prompt
			ui_confirmText.text = "Are you sure you want to quit the game?";
			currState = MenuStates.MAIN_MENU;			// remember that the confirmation menu is dealing with main menu
			if (!ui_confirm.activeSelf)
			{
				ui_confirm.SetActive(true);				// show the confirmation menu
			}
		}

        /*
         * Ends the current player's turn.
		 * Goes through both player's units and sets their turn to true/false and resets their steps left.
         */
        public void endTurn()
        {
            Debug.Log("Current team: " + curTeam);
            Debug.Log(mode);
            if (mode == 0)		// Human vs Human
            {
                Debug.Log(bluePlayer.Count + " in wrong mode " + redComp.Count);
                if (curTeam == 0)
                {
                    setTurn(redPlayer, true);
                    setTurn(bluePlayer, false);
                    curTeam = 1;
                }
                else
                {
                    setTurn(bluePlayer, true);
                    setTurn(redPlayer, false);
                    curTeam = 0;
                }
            }
            else if (mode == 1)	// Human vs Computer
            {
                Debug.Log(bluePlayer.Count + " Count" + "    " + redComp.Count);
                if (curTeam == 0)
                {
                    setEnemyTurn(redComp, true);
                    setTurn(bluePlayer, false);
                    curTeam = 1;
                    //Initialize the AI for this turn
                    List<MovingObject> other = new List<MovingObject>();
                    foreach (Player e in bluePlayer)
                    {
                        //Add all the oppossing units
                        other.Add((MovingObject)e);
                    }
                    redAI.init(redComp, other);
                    //Call AI script to determine behavior
                    redAI.onTurn();
                    //Make the moves and handle animatiosn
                    StartCoroutine(MoveEnemies(redAI));
                }
                else
                {
                    setTurn(bluePlayer, true);
                    setEnemyTurn(redComp, false);
                    curTeam = 0;
                }
            }
            else if (mode == 2)	// Computer vs Computer
            {
                Debug.Log(blueComp.Count + " bCount" + " " + redComp.Count + " rCount");
                if (curTeam == 0)
                {
                    setEnemyTurn(redComp, true);
                    setEnemyTurn(blueComp, false);
                    List<MovingObject> other = new List<MovingObject>();
                    //Initialize the AI for this turn
                    foreach (Enemy e in blueComp)
                    {
                      //Add all the oppossing units
                        other.Add((MovingObject)e);
                    }
                    redAI.init(redComp, other);
                    //Call AI script to determine behavior
                    redAI.onTurn();
                    //Make the moves and handle animatiosn
                    StartCoroutine(MoveEnemies(redAI));
                    curTeam = 1;
                }
                else
                {
                    setEnemyTurn(blueComp, true);
                    setEnemyTurn(redComp, false);
                    List<MovingObject> other = new List<MovingObject>();
                    //Initialize the AI for this turn

                    foreach (Enemy e in redComp)
                    {
                      //Add all the oppossing units
                        other.Add((MovingObject)e);
                    }
                    blueAI.init(blueComp, other);
                    //Call AI script to determine behavior
                    blueAI.onTurn();
                    //Make the moves and handle animatiosn
                    StartCoroutine(MoveEnemies(blueAI));
                    curTeam = 0;
                }
            }
        }

        /*
         * Used by endTurn to set the boolean myTurn of players to true/false and reset their stepsLeft to stepsLeft
         */
        public void setTurn(List<Player> stuff, bool set)
		{
			foreach (Player temp in stuff)
			{
				temp.setTurn(set);
                temp.resetStepsRemaining();
				Debug.Log("Made it here");
				if (!set)
					temp.endPlayerTurn();
			}
		}

        /*
         * Used by endTurn to set the boolean myTurn of enemies to true/false and reset their stepsLeft to stepsLeft
         * At the moment, Enemies will automatically end their turn once they run out of moves, we need to change that so it's up to
         * whoever is implementing an AI algorithm TODO
         */
        public void setEnemyTurn(List<Enemy> stuff, bool set)
		{
			foreach (Enemy temp in stuff)
			{
				temp.myTurn = set;
                temp.resetStepsRemaining();
			}
		}

		/*
         * Sets the current team whose turn it is
         * in Mode 0(PvP) one list of players will be on team0 and another on team1
         * in Mode 1(PvE) the players will be on team0 and the enemies on team1
         * in Mode 2(EvE) one list of enemies will be on team0 and the other on team1
         */
		public void setCurTeam(int team)
		{
			curTeam = team;
		}

		/*
		 *	Called when the scene is loaded; performs setup.
		 */
		void OnLevelWasLoaded(int index)
		{
			if (instance == null)
				instance = this;
			else if (instance != this)
				Destroy(gameObject);		// destroy this if one already exists, to preserve singleton.

			DontDestroyOnLoad(gameObject);	// don't destroy this when reloading scene
			mode = BoardManager.mode;

			// assign enemies to a new List of Enemy objects.
			if (mode == 1)
			{
				blueComp = new List<Enemy>();
			}
			else if (mode == 2)
			{
				redComp = new List<Enemy>();
			}

			// find and remember UI elements for use later
			ui_confirm = GameObject.Find("Confirmation");
			ui_confirmText = GameObject.Find("ConfirmPromptText").GetComponent<Text>();
			ui_confirm.SetActive(false);

			// get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();

			// initialize the rest of the game
			InitGame();
		}

		/*
		 *	Initializes the game.
		 */
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;

			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find("LevelImage");

			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();

            gameoverText = GameObject.Find("GameOver");
            gameoverText.SetActive(false);

            Debug.Log("AIs are " + BoardManager.AIBlue + " (blue), " + BoardManager.AIRed + " (red)");

            if (BoardManager.AIBlue != "none")
    			blueAI = ListAI.AIPrograms[BoardManager.AIBlue];
            if (BoardManager.AIRed != "none")
			    redAI = ListAI.AIPrograms[BoardManager.AIRed];

			//Set levelImage to active blocking player's view of the game board during setup.
			doingSetup = false;

			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			Invoke("HideLevelImage", levelStartDelay);

			// clear all previous units
			blueComp.Clear();
			redComp.Clear();
			bluePlayer.Clear();
			redPlayer.Clear();

			// Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(mode);
		}

		/*
         *	Used to set the mode.
		 *	@param	i		The mode to use, an int.
         */
		public void setMode(int i)
		{
			mode = i;
		}

		/*
		 *	Called every frame to update the game.
		 */
		void Update()
		{
			mode = BoardManager.mode;
		}

        public void removePlayer(Player dead, int team)
        {
            if (team == 0)
            {
                bluePlayer.Remove(dead);
                if (bluePlayer.Count == 0)
                {
                    showEndState();
                }
            }
            else
            {
                redPlayer.Remove(dead);
                if (redPlayer.Count == 0)
                {
                    showEndState();
                }
            }
        }

        /*
         * When a unit dies, remove them from the list, then check for a win condition.
         */
        public void removeEnemy(Enemy dead, int team)
        {
            if (team == 0)
            {
                blueComp.Remove(dead);
                if (blueComp.Count == 0)
                {
                    showEndState();
                }
            }
            else
            {
                redComp.Remove(dead);
                if (redComp.Count == 0)
                {
                    showEndState();
                }
            }
		}

        /*
         *  Disables gameplay and shows the game over information.
         */
        void showEndState()
        {
            Debug.Log("End game!");
            gameoverText.SetActive(true);
            GameObject turnButton = GameObject.Find("EndTurn");
            turnButton.SetActive(false);
        }

		/*
		 *	Hides black sprite background visible between levels.
		 */
        void HideLevelImage()
        {
            //Disable the levelImage gameObject.
            levelImage.SetActive(false);

            //Set doingSetup to false allowing player to move again.
            doingSetup = false;
        }

        /*
         *	When a player is a computer, this function is called in their start() method.
		 *	Adds the unit to the appropriate list.
         */
        public void AddEnemyToList(Enemy script, int team)
        {
            if (team == 0)
            {
                blueComp.Add(script);
            }
            else
				redComp.Add(script);
        }

		/*
		 *	Helper to move computer-controlled units. Called in endTurn().
		 */
		IEnumerator MoveEnemies(AIBase ai)
        {
            Debug.Log("We made it");
            //While enemiesMoving is true player is unable to move.
            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(turnDelay);
                //If there are no enemies spawned (IE in first level):
                if (ai.Count == 0)
                {
                    //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
                    yield return new WaitForSeconds(turnDelay);
                }

                //Loop through List of AIActions
                foreach(AIAction act in ai.acts)
                {

                    Debug.Log(string.Format("Position {0},{1} To {2},{3}", act.obj.currentPos.x, act.obj.currentPos.y, act.pos.x, act.pos.y));
                    //If it is an move than make the unit move
                    if(act.action==AIAction.Actions.Move)
                        act.obj.MoveEnemy((int)act.pos.x, (int)act.pos.y);
                    //If it is a move make it attack
                    if (act.action == AIAction.Actions.Attack) {
                        Debug.Log("ATTACK ENEMY");

                        act.obj.attack((int)act.pos.x, (int)act.pos.y);
                    }
                    //Reset valid tiles to prevent graphical problems.
                    act.obj.resetValidTiles();
                    yield return new WaitForSeconds(act.obj.moveTime);
                }
            Debug.Log(string.Format("End Turn"));

            endTurn();
        }
    }
}
