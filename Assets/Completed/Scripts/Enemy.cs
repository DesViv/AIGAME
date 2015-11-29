using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;   //Allows us to use UI.

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MovingObject
	{
		public int playerDamage; 							//The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.
		public int health;
        public int attackPower;
		public bool alive;
		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target;							//Transform to attempt to move toward each turn.
		private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.

        public int team;
        private Text attackText;
        private Text healthText;
        public int moveRange;
        //Moves a unit can make each turn
        public int stepsLeft;
        Vector3 currentPos;
        public bool myTurn;
        //Start overrides the virtual Start function of the base class.
        protected override void Start ()
		{
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects.
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList(this, team);
            attackText = GameObject.Find("Attack").GetComponent<Text>();
            healthText = GameObject.Find("Health").GetComponent<Text>();
            //Get and store a reference to the attached Animator component.
            animator = GetComponent<Animator> ();
            myTurn = false;
            //Find the Player GameObject using it's tag and store a reference to its transform component.
            currentPos = transform.position; //Returns the current position of the unit

			//Call the start function of our base class MovingObject.
			base.Start ();
		}

       
        //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
        //See comments in MovingObject for more on how base AttemptMove function works.
        protected override void AttemptMove (int xDir, int yDir)
		{
			//Check if skipMove is true, if so set it to false and skip this turn.
			if(skipMove)
			{
				skipMove = false;
				return;

			}

			//Call the AttemptMove function from MovingObject.
			base.AttemptMove (xDir, yDir);

			//Now that Enemy has moved, set skipMove to true to skip next move.
			skipMove = true;
		}


		//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
		public void MoveEnemy ()
		{
            Debug.Log(stepsLeft + " fuck this");
            while (stepsLeft != 0)
            {
                List<GameObject> validMoves = showValidTiles(1);
                StartCoroutine(wait());
                transform.position = validMoves[0].transform.position;
                stepsLeft--;
            }
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
        private List<GameObject> showValidTiles(int moveRange)
        {
            GameObject[] floors;
            List<GameObject> validFloors = new List<GameObject>();
            floors = GameObject.FindGameObjectsWithTag("Floor");
            int curX = (int)transform.position.x;
            int curY = (int)transform.position.y;

            for (int i = 0; i < floors.Length; i++)
            {
                int fX = (int)floors[i].transform.position.x;
                int fY = (int)floors[i].transform.position.y;
                int xdif = Mathf.Abs(fX - curX);
                int ydif = Mathf.Abs(fY - curY);
                if (xdif + ydif <= moveRange && xdif + ydif != 0)
                {

                    Vector3 rayOrigin = new Vector3(10, 10, -100);
                    Vector3 rayDes = new Vector3(fX, fY, 0.1f);
                    Vector3 rayDir = (rayDes - rayOrigin).normalized;
                    Ray ray = new Ray(rayOrigin, rayDir);
                    if (!Physics.Raycast(ray))
                    {
                        SpriteRenderer renderer = floors[i].GetComponent<SpriteRenderer>();
                        //renderer.color = Color.green;
                        validFloors.Add(floors[i]);
                        validPositions.Add(floors[i].transform.position);
                    }
                    else
                    {
                        //Debug.Log("wall at (" + fX + "," + fY + ")");
                    }
                }

            }
            return validFloors;
        }
   
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



        private List<GameObject> showValidAttack()
        {
            List<GameObject> valid = new List<GameObject>();
            int curX = (int)transform.position.x;
            int curY = (int)transform.position.y;
            Vector3 cur = new Vector3(10, 10, -100);
            List<Vector3> list = new List<Vector3>();
            list.Add(new Vector3(curX, curY + 1, 0.1f));
            list.Add(new Vector3(curX, curY - 1, 0.1f));
            list.Add(new Vector3(curX - 1, curY, 0.1f));
            list.Add(new Vector3(curX + 1, curY, 0.1f));
            for (int i = 0; i < list.Count; i++)
            {
                RaycastHit hit;
                Ray ray = new Ray(cur, (list[i] - cur).normalized);
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject target = hit.collider.gameObject;

                    if (target.tag == "Player")
                    {
                        Player current = target.gameObject.GetComponent<Player>();
                        if (this.team != current.team)
                        {
                            valid.Add(hit.collider.gameObject);
                            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
                            sr.color = new Color(0f, 0f, 0f, 1f);
                        }
                    }
                    else if(target.tag == "Enemy2" || target.tag == "Enemy")
                    {
                        Enemy current = target.gameObject.GetComponent<Enemy>();
                        if (this.team != current.team)
                        {
                            valid.Add(hit.collider.gameObject);
                            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
                            sr.color = new Color(0f, 0f, 0f, 1f);
                        }
                    }
                }
            }
            return valid;
        }


        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        protected override void OnCantMove <T> (T component)
		{
			//Declare hitPlayer and set it to equal the encountered component.
			Player hitPlayer = component as Player;

			//Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
			//hitPlayer.LoseFood (playerDamage); TODO: change how enemy attacks work

			//Set the attack trigger of animator to trigger Enemy attack animation.
			animator.SetTrigger ("enemyAttack");

			//Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}

        void OnMouseOver()
        {
            attackText.text = "AP: " + attackPower;
            healthText.text = "HP: " + health;
            //if (myTurn)
                //stepsLeftText.text = "Moves remaining: " + stepsLeft;
        }
        void OnMouseExit()
        {
            attackText.text = "";
            healthText.text = "";
            //stepsLeftText.text = "";
        }
    }
}
