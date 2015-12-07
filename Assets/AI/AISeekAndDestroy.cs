using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Completed;

public class AISeekAndDestroy : AIBase { //This class greedily finds the nearest player unit and tries to kill them
	private List<Transform> targets;
	private List<Integer> distances;
    public override void onTurn()
    {
        base.onTurn();
        foreach (Enemy e in self) //action for each enemy unit
        {
			int xDir = 0;
			int yDir = 0;
            Vector3 cur = e.currentPos;
			MovingObject closest = other[0];
			int closestDistance = Mathf.Sqrt(Mathf.Pow((closest.transform.position.x - cur.x),2) + Mathf.Pow ((closest.transform.position.y - cur.y),2));
            foreach (MovingObject obj in other) //calculates closest object
			{
				int distance = Mathf.Sqrt(Mathf.Pow((obj.transform.position.x - cur.x),2) + Mathf.Pow ((obj.transform.position.y - cur.y),2));
			    if (distance < closestDistance) {
					closestDistance = distances;
					closest = obj;
				}
			}
			bool xComparison = Mathf.Abs (closest.transform.position.x - cur.x) < float.Epsilon;
			bool yComparison = Mathf.Abs (closest.transform.position.y - cur.y) < float.Epsilon;
			if (xComparison && yComparison) {  //if object is within range, then can attack that object
				actions.Add (new AIAction(e, AIAction.Actions.Attack, closest.transform.position));
			}
			else {  //if not, then try to get closer to object
    		   if (xComparison) {
			      yDir = closest.transform.position.y > cur.y ? 1 : -1;
			   }
			   else {
			 	  xDir = closest.transform.position.x > cur.x ? 1 : -1;
			   }
		       actions.Add(new AIAction(e, AIAction.Actions.Move, cur + new Vector3(xDir, yDir, 0)));
		   }
        }
    }
}
