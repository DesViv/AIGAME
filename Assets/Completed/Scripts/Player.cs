using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace Completed
{
    //Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class Player : MovingObject
    {
        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
        public Text foodText;                       //UI Text to display current player food total.
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
        public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
        public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
        public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
        public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
        public AudioClip gameOverSound;             //Audio clip to play when player dies.

        public Color myColor;
        private Animator animator;                  //Used to store a reference to the Player's animator component.
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.

        private Button endAction;

        private bool movingPhase;
        private bool attackPhase;

        //Start overrides the Start function of MovingObject
        public override void Start()
		{
			//Call the Start function of the MovingObject base class.
			base.Start();

            //Get a component reference to the Player's animator component
            animator = GetComponent<Animator>();

            myTurn = false;
            movingPhase = true;
            attackPhase = true;
            currentStepsLeft = stepsLeft;

            if (team == 0)
            {
                //Player 1 always goes first
                this.myTurn = true;
                GM.bluePlayer.Add(this);
            }
            else GM.redPlayer.Add(this);

        }


        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {

        }

        void OnMouseDown()
        {
			// quit if confirmation dialogue is visible
			if (GM.ui_confirm.activeSelf) return;

            if (myTurn)//   &&  (GM.curPlayer==null|| GM.curPlayer == this))
            {
                Debug.Log(this.GetType());
                GM.setCurTeam(team);
                if (currentStepsLeft > 0)
                {
                    //Move one tile at a time because there are pathing issues when moving multiple tiles at a time, and I don't want to spend the time fixing them.
                    showValidTiles(moveRange);
                    showValidAttack();
                }
                if (!moveSelection)
                {
                    Debug.Log("move phase" + currentStepsLeft);
                    if (currentStepsLeft > 0)
                    {

                        canMove = true;
                        firstClick = true;
                    }
                    moveSelection = true;
                }
                else
                    moveSelection = false;
            }
        }

        public new void endPlayerTurn()
        {
            myTurn = false;
            Debug.Log(myTurn + " this should be false");
            resetValidTiles();
            currentStepsLeft = stepsLeft;
        }

        public void setTurn(bool turn)
        {
            myTurn = turn;
        }

        public void setSteps(int steps)
        {
            currentStepsLeft = stepsLeft = steps;
        }

        public override void showValidTiles(int stepsLeft)
        {
            GameObject[] floors;
            floors = GameObject.FindGameObjectsWithTag("Floor");
            int curX = (int)transform.position.x;
            int curY = (int)transform.position.y;

            for (int i = 0; i < floors.Length; i++)
            {
                int fX = (int)floors[i].transform.position.x;
                int fY = (int)floors[i].transform.position.y;
                int xdif = Mathf.Abs(fX - curX);
                int ydif = Mathf.Abs(fY - curY);
                if (xdif + ydif <= stepsLeft && xdif + ydif != 0)
                {

                    Vector3 rayOrigin = new Vector3(10, 10, -100);
                    Vector3 rayDes = new Vector3(fX, fY, 0.1f);
                    Vector3 rayDir = (rayDes - rayOrigin).normalized;
                    Ray ray = new Ray(rayOrigin, rayDir);
                    if (!Physics.Raycast(ray))
                    {
                        SpriteRenderer renderer = floors[i].GetComponent<SpriteRenderer>();
                        renderer.color = Color.green;
                        validMoves.Add(floors[i]);
                        validPositions.Add(floors[i].transform.position);
                    }
                    else
                    {
                        //Debug.Log("wall at (" + fX + "," + fY + ")");
                    }
                }

            }
        }

 



        private void Update()
        {
            //If it's not the player's turn, exit the function.
            if (!myTurn)
                return;

            int desX = -100;    //Used to store the horizontal move direction.
            int desY = -100;		//Used to store the vertical move direction.


            if (canMove && !firstClick)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Pressed left click.");
                    Vector3 posVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    desX = Mathf.RoundToInt(posVec.x);
                    desY = Mathf.RoundToInt(posVec.y);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                firstClick = false;

            }

            //Check if we have a non-zero value for horizontal or vertical
            if (desX != -100 && desY != -100)
            {
                if (!attack(desX, desY))
                {
                    Debug.Log("attackfail");
                    //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                    //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                    AttemptMove(desX, desY);
                }
            }
        }





        //Kill some time
        public IEnumerator wait()
        {
            yield return new WaitForSeconds(1);
        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        public override void AttemptMove(int xDir, int yDir)
        {
            //If Move returns true, meaning Player was able to move into an empty space.
            int stepsTaken = Move(xDir, yDir, validPositions);
            if (stepsTaken > 0)
            {
                currentStepsLeft -= stepsTaken;
                stepsLeftText.text = "Moves remaining: " + currentStepsLeft;
            }

            resetValidTiles();
            canMove = false;
            moveSelection = false;



            //Set the playersTurn boolean of GameManager to false now that players turn is over.
            //GameManager.instance.playersTurn = false;
        }


        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        public override void OnCantMove<T>(T component)
        {
            //Set hitWall to equal the component passed in as a parameter.
            Wall hitWall = component as Wall;

            //Call the DamageWall function of the Wall we are hitting.
            hitWall.DamageWall(wallDamage);

            //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
            animator.SetTrigger("playerChop");
        }


        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "Exit")
            {
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                Invoke("Restart", restartLevelDelay);

                //Disable the player object since level is over.
                enabled = false;
            }

        }


        //Restart reloads the scene when called.
        private void Restart()
        {
            //Load the last scene loaded, in this case Main, the only scene in the game.
            Application.LoadLevel(Application.loadedLevel);
        }

        void OnMouseOver()
        {
            attackText.text = attackPower.ToString();
            rangeText.text = stepsLeft.ToString();
            healthText.text = health.ToString();
            if (myTurn)
                stepsLeftText.text = "Moves remaining: " + currentStepsLeft + "/" + stepsLeft;
            unitPortrait.enabled = true;
            switch (unitType)
            {
                case UnitType.Blocker:
                    nameText.text = (team == 0 ? "Blue" : "Red") + " Blocker";
                    descriptionText.text = "A defensive unit with increased HP.\n- 1.5x vs Rushers\n- 0.5x vs Breakers";
                    unitPortrait.sprite = (Sprite)Resources.Load<Sprite>((team == 0 ? "blue" : "red") + "_blocker");
                break;
                case UnitType.Breaker:
                    nameText.text = (team == 0 ? "Blue" : "Red") + " Breaker";
                    descriptionText.text = "An assault unit with better damage.\n- 1.5x vs Blockers\n- 0.5x vs Rushers";
                    unitPortrait.sprite = (Sprite)Resources.Load<Sprite>((team == 0 ? "blue" : "red") + "_breaker");
                    break;
                case UnitType.Rusher:
                    nameText.text = (team == 0 ? "Blue" : "Red") + " Rusher";
                    descriptionText.text = "A flanking unit with higher range.\n- 1.5x vs Breakers\n- 0.5x vs Blockers";
                    unitPortrait.sprite = (Sprite)Resources.Load<Sprite>((team == 0 ? "blue" : "red") + "_rusher");
                    break;
            }
        }
        void OnMouseExit()
        {
            nameText.text = "";
            attackText.text = "";
            rangeText.text = "";
            healthText.text = "";
            stepsLeftText.text = "";
            descriptionText.text = "";
            unitPortrait.enabled = false;
        }
    }
}
