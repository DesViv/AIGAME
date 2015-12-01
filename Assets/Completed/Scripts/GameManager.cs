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
        public int mode; //0 = PlayervPlayer, 1 = PlayervEnemy, 2 = EnemyvEnemy


        private Text levelText;                                 //Text to display current level number.
        private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
        private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
        private int level = 1;                                  //Current level number, expressed in game as "Day 1".
        private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

        public int curTeam = 0;

        //Every time a Player is added to the game board, it will add itself to one of these lists based on the team it's on in start() of Player.cs
        public List<Player> bluePlayer;
        public List<Player> redPlayer;


        //Every time an Enemy is added to the game board, it will add itself to one of these lists based on the team it's on by calling AddEnemyToList in start() of Enemy.cs
        public List<Enemy> blueComp;
        public List<Enemy> redComp;

        public AIBase blueAI;
        public AIBase redAI;
        //Awake is always called before any Start functions
        void Awake()
        {
            blueAI = new AISimple();
            redAI = new AISimple();
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
                blueComp = new List<Enemy>();
            }
            if(mode == 2)
            {
                redComp = new List<Enemy>();
            }
            

            //Get a component reference to the attached BoardManager script
            boardScript = GetComponent<BoardManager>();

            //Call the InitGame function to initialize the first level
            InitGame();
        }

        /*
         * Goes through a list of enemies/players, sets their turn to true/false and resets their stepsleft to 5
         */ 
        public void endTurn()
        {
            Debug.Log("Current team: " + curTeam);
            Debug.Log(mode);
            if (mode == 0) // PlayervPlayer
            {
                Debug.Log(bluePlayer.Count + " IN WRONg MODE" + "    " + redComp.Count);
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
            else if(mode == 1) //PlayervEnemy
            {

                Debug.Log(bluePlayer.Count + " Count" + "    " + redComp.Count);
                if (curTeam == 0)
                {
                    setEnemyTurn(redComp, true);                    
                    setTurn(bluePlayer, false);
                    curTeam = 1;
                    List<MovingObject> other = new List<MovingObject>();
                    foreach (Player e in bluePlayer)
                    {
                        other.Add((MovingObject)e);
                    }
                    redAI.init(redComp, other);
                    redAI.onTurn();
                    StartCoroutine(MoveEnemies(redAI));
                }
                else
                {
                    setTurn(bluePlayer, true);
                    setEnemyTurn(redComp, false);
                    curTeam = 0;
                }
            }
            else if(mode == 2) //EvE
            {
                if (curTeam == 0)
                {
                    setEnemyTurn(redComp, true);
                    setEnemyTurn(blueComp, false);
                    List<MovingObject> other = new List<MovingObject>();
                    foreach (Player e in bluePlayer)
                    {
                        other.Add((MovingObject)e);
                    }
                    redAI.init(redComp, other);
                    redAI.onTurn();
                    StartCoroutine(MoveEnemies(redAI));
                    curTeam = 1;
                }
                else
                {
                    setEnemyTurn(blueComp, true);
                    setEnemyTurn(redComp, false);
                    List<MovingObject> other = new List<MovingObject>();
                    foreach (Player e in redPlayer)
                    {
                        other.Add((MovingObject)e);
                    }
                    blueAI.init(blueComp, other);
                    blueAI.onTurn();
                    StartCoroutine(MoveEnemies(blueAI));
                    curTeam = 0;
                }
            }
        }

        /*
         * Used by endTurn to set the boolean myTurn of players to true/false and reset their stepsLeft to 5
         */
        public void setTurn(List<Player> stuff, bool set)
        {
            foreach (Player temp in stuff)
            {
                temp.setTurn(set);
                temp.setSteps(5);
                Debug.Log("Made it here");
                if (!set)
                    temp.endPlayerTurn();
            }
        }

        /*
         * Used by endTurn to set the boolean myTurn of enemies to true/false and reset their stepsLeft to 5
         * At the moment, Enemies will automatically end their turn once they run out of moves, we need to change that so it's up to 
         * whoever is implementing an AI algorithm TODO
         */
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
         * When a unit dies, remove them from the list
         * then check for win condition
         * Each player unit checks to see if it's dead when it gets attacked in Player.cs attack()
         * If it dies, it will call this function so the gamemanager can remove it from the list and destroy it's corresponding GameObject
         * 
         */
        public void removePlayer(Player dead, int team)
        {
            if(team == 0)
            {
                bluePlayer.Remove(dead);
                Debug.Log(bluePlayer.Count + " player0");
                if(bluePlayer.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
            }
            else
            {
                redPlayer.Remove(dead);
                if (redPlayer.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
                Debug.Log(redPlayer.Count + " player1");
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
                blueComp.Remove(dead);
                Debug.Log(redComp.Count + " enemy1");
                if (redComp.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
            }
            else
            {
                redComp.Remove(dead);
                if (redComp.Count == 0)
                {
                    Application.LoadLevel("GameOver");
                }
                Debug.Log(redComp.Count + " enemy1");
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

            //Clear any Player/Enemy objects in our List to prepare for next level.
            blueComp.Clear();
            redComp.Clear();
            bluePlayer.Clear();
            redPlayer.Clear();

            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardScript.SetupScene(level);

        }

        /*
         * Used to set the mode
         */ 
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
        }

        /*
         * Enemies call this function in their start() method
         */ 
        public void AddEnemyToList(Enemy script, int team)
        {
            //Add Enemy to List enemies.
            //enemies.Add(script);
            if (team == 0)
            {
                blueComp.Add(script);
            }
            else redComp.Add(script);
        }

        //Coroutine to move enemies in sequence.
        //Currently this is being called in endturn() to move our enemies
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

                //Loop through List of Enemy objects.
                foreach(AIAction act in ai.acts)
                {
                Debug.Log(string.Format("Position {0},{1} To {2},{3}", act.obj.currentPos.x, act.obj.currentPos.y, act.pos.x, act.pos.y));
                    if(act.action==AIAction.Actions.Move)
                        act.obj.MoveEnemy((int)act.pos.x, (int)act.pos.y);
                    yield return new WaitForSeconds(act.obj.moveTime);
                }
            endTurn();           
           
        }
    }
}
