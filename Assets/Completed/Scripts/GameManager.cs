using UnityEngine;
using System.Collections;

namespace Completed
{
    using System.Collections.Generic;       //Allows us to use Lists.
    using UnityEngine.UI;                   //Allows us to use UI.

    public class GameManager : MonoBehaviour
    {
        public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
        public float turnDelay = 0.1f;                          //Delay between each Player turn.
        public int playerFoodPoints = 100;                      //Starting value for Player food points.
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector]
        public bool playersTurn = true;     //Boolean to check if it's players turn, hidden in inspector but public.
        public int mode; //0 = PlayervPlayer, 1 = PlayervEnemy, 2 = EnemyvEnemy


        private Text levelText;                                 //Text to display current level number.
        private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
        private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
        private int level = 1;                                  //Current level number, expressed in game as "Day 1".
        //private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.
        private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

        public int curTeam = 0;
        public List<Player> player0;
        public List<Player> player1;

        public List<Enemy> enemy0;
        public List<Enemy> enemy1;
        public Player captain;
        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
            mode = BoardManager.mode;
            //Assign enemies to a new List of Enemy objects.
            if(mode == 1)
            {
                enemy0 = new List<Enemy>();
            }
            else if(mode == 2)
            {
                enemy1 = new List<Enemy>();
            }
            

            //Get a component reference to the attached BoardManager script
            boardScript = GetComponent<BoardManager>();

            //Call the InitGame function to initialize the first level
            InitGame();
        }

        public void endTurn()
        {

            Debug.Log("Current team: " + curTeam);
            Debug.Log(mode);
            if (mode == 0) // PlayervPlayer
            {
                Debug.Log(player0.Count + " IN WRONg MODE" + "    " + enemy1.Count);
                if (curTeam == 0)
                {
                    setTurn(player1, true);
                    setTurn(player0, false);
                    curTeam = 1;
                }
                else
                {
                    setTurn(player0, true);
                    setTurn(player1, false);
                    curTeam = 0;
                }
            }
            else if(mode == 1) //PlayervEnemy
            {

                Debug.Log(player0.Count + " Count" + "    " + enemy1.Count);
                if (curTeam == 0)
                {
                    setEnemyTurn(enemy1, true);                    
                    setTurn(player0, false);
                    curTeam = 1;
                    StartCoroutine(MoveEnemies(enemy1));
                }
                else
                {
                    setTurn(player0, true);
                    setEnemyTurn(enemy1, false);
                    curTeam = 0;
                }
            }
            else if(mode == 2) //EvE
            {
                if (curTeam == 0)
                {
                    setEnemyTurn(enemy1, true);
                    setEnemyTurn(enemy0, false);
                    StartCoroutine(MoveEnemies(enemy1));
                    curTeam = 1;
                }
                else
                {
                    setEnemyTurn(enemy0, true);
                    setEnemyTurn(enemy1, false);
                    StartCoroutine(MoveEnemies(enemy0));
                    curTeam = 0;
                }
            }
        }

        /*
         * Makes way more sense to make another class to store teams, but I'm just trying to get something working for now and this was easy
         */
        public void setTurn(List<Player> stuff, bool set)
        {
            Debug.Log(stuff.Count + " fuckin up the count");
            foreach (Player temp in stuff)
            {
                temp.setTurn(set);
                temp.setSteps(5);
                if (!set)
                    temp.endPlayerTurn();
            }
        }

        public void setEnemyTurn(List<Enemy> stuff, bool set)
        {
            foreach (Enemy temp in stuff)
            {
                temp.myTurn = set;
                temp.stepsLeft = 5;
                //if (!set)
                    //temp.endPlayerTurn();
            }
        }
        public void setCurTeam(int team)
        {
            curTeam = team;
        }


        /*
         * When a unit dies, remove them from the list
         * then check for win condition
         */
        public void removePlayer(Player dead, int team)
        {
            if(team == 0)
            {
                player0.Remove(dead);
                Debug.Log(player0.Count + " player0");
                if(player0.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
            }
            else
            {
                player1.Remove(dead);
                if (player1.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
                Debug.Log(player1.Count + " player1");
            }
        }

        /*
         * When a unit dies, remove them from the list
         * then check for win condition
         */
        public void removeEnemy(Enemy dead, int team)
        {
            if (team == 0)
            {
                enemy0.Remove(dead);
                Debug.Log(enemy1.Count + " enemy1");
                if (enemy1.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
            }
            else
            {
                enemy1.Remove(dead);
                if (enemy1.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
                Debug.Log(enemy1.Count + " enemy1");
            }
        }

        //This is called each time a scene is loaded.
        void OnLevelWasLoaded(int index)
        {
            //Add one to our level number.
            level++;
            //Call InitGame to initialize our level.
            InitGame();
        }

        //Initializes the game for each level.
        void InitGame()
        {
            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            doingSetup = true;

            //Get a reference to our image LevelImage by finding it by name.
            levelImage = GameObject.Find("LevelImage");

            //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
            levelText = GameObject.Find("LevelText").GetComponent<Text>();

            //Set the text of levelText to the string "Day" and append the current level number.
            //levelText.text = "Day " + level;

            //Set levelImage to active blocking player's view of the game board during setup.
            //levelImage.SetActive(false);
            doingSetup = false;

            //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
            Invoke("HideLevelImage", levelStartDelay);

            //Clear any Enemy objects in our List to prepare for next level.
            enemy0.Clear();
            enemy1.Clear();
            player0.Clear();
            player1.Clear();

            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardScript.SetupScene(level);

        }

        public void setMode(int i)
        {
            mode = i;
        }

        //Hides black image used between levels
        void HideLevelImage()
        {
            //Disable the levelImage gameObject.
            levelImage.SetActive(false);

            //Set doingSetup to false allowing player to move again.
            doingSetup = false;
        }

        //Update is called every frame.
        void Update()
        {
            mode = BoardManager.mode;
           /* bool continueGame = true;
			for (int i = 0; i < players.Count; i++) {
                continueGame = false;
				Debug.Log (players[i].alive);
				if (players[i].alive) {
					continueGame = true;
					break;
				}
			}
            if (continueGame == false) {
					GameOver();
		    }
            //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
            if (playersTurn || enemiesMoving || doingSetup)

                //If any of these are true, return and do not start MoveEnemies.
                return;

            //Start moving enemies.
            StartCoroutine(MoveEnemies());*/
        }

        //Call this to add the passed in Enemy to the List of Enemy objects.
        public void AddEnemyToList(Enemy script, int team)
        {
            //Add Enemy to List enemies.
            //enemies.Add(script);
            if (team == 0)
            {
                enemy0.Add(script);
            }
            else enemy1.Add(script);
        }


        //GameOver is called when the player reaches 0 food points
        public void GameOver()
        {
            //Set levelText to display number of levels passed and game over message
            levelText.text = "After " + level + " days, you starved.";

            //Enable black background image gameObject.
            levelImage.SetActive(true);

            //Disable this GameManager.
            enabled = false;
        }

        //Coroutine to move enemies in sequence.
       IEnumerator MoveEnemies(List<Enemy> enemies)
        {
            Debug.Log("We made it");
            //While enemiesMoving is true player is unable to move.
            enemiesMoving = true;
            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(turnDelay);               
                //If there are no enemies spawned (IE in first level):
                if (enemies.Count == 0)
                {
                    //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
                    yield return new WaitForSeconds(turnDelay);
                }

                //Loop through List of Enemy objects.
                for (int i = 0; i < enemies.Count; i++)
                {
                //Call the MoveEnemy function of Enemy at index i in the enemies List.
                enemies[i].MoveEnemy();
                    //Wait for Enemy's moveTime before moving next Enemy,
                    yield return new WaitForSeconds(enemies[i].moveTime);
                }
                //Once Enemies are done moving, set playersTurn to true so player can move.
                playersTurn = true;

                //Enemies are done moving, set enemiesMoving to false.
                enemiesMoving = false;
            endTurn();           
           
        }
    }
}
