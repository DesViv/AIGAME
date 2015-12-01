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

		// ---------- UNIT INFORMATION ----------------------------------------
		public enum UnitType{Rusher, Breaker, Blocker}
		public UnitType unitType;	// 0 - rusher; 1 - breaker; 2 - blocker

		// All of these values are being set in the Player prefab
		public int team;                          //player 1 or 2
		public int health;
		public int attackPower;
		public int moveRange;
		public int stepsLeft;
        public Text damageText;

        //
        // ---------- UNIT INFORMATION END ------------------------------------


        //Protected, virtual functions can be overridden by inheriting classes.
        protected virtual void Start()
        {
            //Get a component reference to this object's BoxCollider2D
            boxCollider = GetComponent<BoxCollider>();

            //Get a component reference to this object's Rigidbody2D
            rb2D = GetComponent<Rigidbody>();
            damageText = GameObject.Find("Damage").GetComponent<Text>();


            //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
            inverseMoveTime = 1f / moveTime;
        }


        //Move returns true if it is able to move and false if not. 
        //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
        protected int Move(int desX, int desY, List<Vector3> validP)
        {
            bool valid = false;

            Vector3 des = new Vector3(desX, desY, 0);

            for (int i = 0; i < validP.Count; i++)
            {
                if (des == validP[i]) {
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

                //Return true to say that Move was successful
                Debug.Log(steps);
                return steps;

            }

            //If something was hit, return false, Move was unsuccesful.
            return 0;
        }


        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        protected IEnumerator SmoothMovement(Vector3 end)
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

        public IEnumerator floatingText(Vector2 start, Vector2 end)
        {
            damageText.rectTransform.position = start;
            damageText.enabled = true;
            float duration = 1f;
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration; //0 means the animation just started, 1 means it finished
                damageText.rectTransform.position = Vector2.Lerp(start, end, t);
                elapsedTime += 2 * Time.deltaTime;
                yield return null;
            }

            damageText.enabled = false;
            yield return null;
        }


        //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
        //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
        protected virtual void AttemptMove(int desX, int desY)
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


        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        protected abstract void OnCantMove<T>(T component)
            where T : Component;
    }
}