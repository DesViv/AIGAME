using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;   //Allows us to use UI.

namespace Completed
{
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class Enemy : MovingObject
    {

        public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
        public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.

        public bool alive; // Don't really need this, keeping it for now
        private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
        private Transform target;                           //Transform to attempt to move toward each turn.
        private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.

        //Moves a unit can make each turn
        // All of these values are being set in the Enemy/Enemy2 prefabs
        //Start overrides the virtual Start function of the base class.
        public override void Start()
        {
            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects.
            //This allows the GameManager to issue movement commands.
            GameManager.instance.AddEnemyToList(this, team);
            attackText = GameObject.Find("Attack").GetComponent<Text>();
            healthText = GameObject.Find("Health").GetComponent<Text>();
            //Get and store a reference to the attached Animator component.
            animator = GetComponent<Animator>();
            myTurn = false;
            //Find the Player GameObject using it's tag and store a reference to its transform component.
            currentPos = transform.position; //Returns the current position of the unit

            //Call the start function of our base class MovingObject.
            base.Start();
        }


        //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
        //See comments in MovingObject for more on how base AttemptMove function works.
        public override void AttemptMove(int xDir, int yDir)
        {
            //Check if skipMove is true, if so set it to false and skip this turn.
            if (skipMove)
            {
                skipMove = false;
                return;
            }

            //Call the AttemptMove function from MovingObject.
            int stepsTaken = Move(xDir, yDir, validPositions);
            if (stepsTaken > 0)
            {
                stepsLeft -= stepsTaken;
            }

            resetValidTiles();

            //Now that Enemy has moved, set skipMove to true to skip next move.
            skipMove = true;
        }


        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveEnemy(int x, int y)
        {
            showValidTiles(stepsLeft);
            Debug.Log(validMoves.Count);
            StartCoroutine(wait());
            int stepsTaken = Move(x, y, validPositions);
            stepsLeft -= stepsTaken;
            resetValidTiles();
            myTurn = false;
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            /*int xDir = 0;
			int yDir = 0;

			//If the difference in positions is approximately zero (Epsilon) do the following:
			if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)

				//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
				yDir = target.position.y > transform.position.y ? 1 : -1;

			//If the difference in positions is not approximately zero (Epsilon) do the following:
			else
				//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
				xDir = target.position.x > transform.position.x ? 1 : -1;

			//Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
			AttemptMove (xDir, yDir);*/
        }
        //Kill some time
        public IEnumerator wait()
        {
            yield return new WaitForSeconds(5);
        }

        /*
         * Literally just copied/pasted this form player, but it should return the number of valid moves based on move range
         * Probably not even necessary for this class
         */

        /*
         * Don't actually use any of these because they don't check to see if it's valid to even move, just some examples on how to move.
         */
        public void moveLeft()
        {
            currentPos = transform.position;
            currentPos = new Vector3(currentPos.x - 1, currentPos.y, 1f);
            transform.position = currentPos;
        }
        public void moveRight()
        {
            currentPos = transform.position;
            currentPos = new Vector3(currentPos.x + 1, currentPos.y, 1f);
            transform.position = currentPos;
        }

        public void moveUp()
        {
            currentPos = transform.position;
            currentPos = new Vector3(currentPos.x, currentPos.y + 1, 1f);
            transform.position = currentPos;
        }
        public void moveDown()
        {
            currentPos = transform.position;
            currentPos = new Vector3(currentPos.x, currentPos.y - 1, 1f);
            transform.position = currentPos;
        }






        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        public override void OnCantMove<T>(T component)
        {
            //Declare hitPlayer and set it to equal the encountered component.
            Player hitPlayer = component as Player;

            //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
            //hitPlayer.LoseFood (playerDamage); TODO: change how enemy attacks work

            //Set the attack trigger of animator to trigger Enemy attack animation.
            animator.SetTrigger("enemyAttack");

            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
        }

    }
}
