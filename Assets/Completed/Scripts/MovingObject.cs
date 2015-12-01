using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Completed
{
    //The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
    public abstract class MovingObject : MonoBehaviour
    {
        public float moveTime = 0.1f;           //Time it will take object to move, in seconds.
        public LayerMask blockingLayer;         //Layer on which collision will be checked.


        public BoxCollider boxCollider;      //The BoxCollider2D component attached to this object.
        public Rigidbody rb2D;               //The Rigidbody2D component attached to this object.
        private float inverseMoveTime;          //Used to make movement more efficient.

        public bool canMove = false;
        public bool moveSelection = false;
        public bool firstClick = false;
        public List<GameObject> validMoves = new List<GameObject>();
        public List<Vector3> validPositions = new List<Vector3>();
        public List<GameObject> validAttack = new List<GameObject>();
        public Vector3 currentPos;

        protected Text attackText;
        protected Text rangeText;
        protected Text healthText;
        protected Text nameText;
        protected Text descriptionText;
        protected Text stepsLeftText;
        protected Text damageText;


        public int team;                          //player 1 or 2
        public int health;
        public int attackPower;
        public int moveRange;
        public int stepsLeft;
        protected int currentStepsLeft;
        public bool myTurn;


        public enum UnitType {Blocker, Breaker, Rusher};
        public UnitType unitType;
        protected Image unitPortrait;

        //public, virtual functions can be overridden by inheriting classes.
        public virtual void Start()
        {
            //Get a component reference to this object's BoxCollider2D
            boxCollider = GetComponent<BoxCollider>();

            //Get a component reference to this object's Rigidbody2D
            rb2D = GetComponent<Rigidbody>();

            nameText = GameObject.Find("Name").GetComponent<Text>();
            stepsLeftText = GameObject.Find("StepsText").GetComponent<Text>();
            descriptionText = GameObject.Find("Description").GetComponent<Text>();
            attackText = GameObject.Find("Attack").GetComponent<Text>();
            rangeText = GameObject.Find("Range").GetComponent<Text>();
            healthText = GameObject.Find("Health").GetComponent<Text>();
            damageText = GameObject.Find("Damage").GetComponent<Text>();
            unitPortrait = GameObject.Find("UnitPortrait").GetComponent<Image>();
            unitPortrait.enabled = false;

            //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
            inverseMoveTime = 1f / moveTime;
        }

        public virtual void showValidAttack()
        {

        }

        //Move returns true if it is able to move and false if not. 
        //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
        public int Move(int desX, int desY, List<Vector3> validP)
        {
            bool valid = false;

            Vector3 des = new Vector3(desX, desY, 0);

            for (int i = 0; i < validP.Count; i++)
            {
                if (des == validP[i])
                {
                    valid = true;
                    break;
                }
            }

            //Check if anything was hit
            if (valid)
            {

                int fX = (int)transform.position.x;
                int fY = (int)transform.position.y;
                int xdif = Mathf.Abs(desX - fX);
                int ydif = Mathf.Abs(desY - fY);
                int steps = xdif + ydif;
                //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
                StartCoroutine(SmoothMovement(des));
                currentPos = des;
                //Return true to say that Move was successful
                Debug.Log(steps);
                return steps;

            }

            //If something was hit, return false, Move was unsuccesful.
            return 0;
        }


        public virtual void showValidTiles(int stepsLeft)
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
                if (xdif + ydif <= stepsLeft && xdif + ydif != 0)
                {

                    Vector3 rayOrigin = new Vector3(10, 10, -100);
                    Vector3 rayDes = new Vector3(fX, fY, 0.1f);
                    Vector3 rayDir = (rayDes - rayOrigin).normalized;
                    Ray ray = new Ray(rayOrigin, rayDir);
                    if (!Physics.Raycast(ray))
                    {
                        validFloors.Add(floors[i]);
                        validPositions.Add(floors[i].transform.position);
                    }
                    else
                    {
                        //Debug.Log("wall at (" + fX + "," + fY + ")");
                    }
                }

            }
        }

        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        public IEnumerator SmoothMovement(Vector3 end)
        {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                //Debug.Log(rb2D.position+" "+end);
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
                //Debug.Log(newPostion);
                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                rb2D.MovePosition(newPostion);
                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }
        }


        //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
        //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
        public virtual void AttemptMove(int desX, int desY)
        {
            /*//Hit will store whatever our linecast hits when Move is called.
			RaycastHit2D hit;
			
			//Set canMove to true if Move was successful, false if failed.
			bool canMove = Move (desX, , out hit);
			
			//Check if nothing was hit by linecast
			if(hit.transform == null)
				//If nothing was hit, return and don't execute further code.
				return;
			
			//Get a component reference to the component of type T attached to the object that was hit
			T hitComponent = hit.transform.GetComponent <T> ();
			
			//If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
			if(!canMove && hitComponent != null)
				
				//Call the OnCantMove function and pass it hitComponent as a parameter.
				OnCantMove (hitComponent);*/
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

        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        public abstract void OnCantMove<T>(T component)
            where T : Component;
    }
}