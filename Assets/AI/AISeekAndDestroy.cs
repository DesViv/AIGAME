using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Completed;

public class AISeekAndDestroy : AIBase {
	private List<Transform> targets;
	private List<Integer> distances;
    public override void onTurn()
    {
        base.onTurn();
        foreach (Enemy e in self)
        {
			int xDir = 0;
			int yDir = 0;
            Vector3 cur = e.currentPos;
			MovingObject closest = other[0];
			int closestDistance = Mathf.Sqrt(Mathf.Pow((closest.transform.position.x - cur.x),2) + Mathf.Pow ((closest.transform.position.y - cur.y),2));
            foreach (MovingObject object in other)
			{
				int distance = Mathf.Sqrt(Mathf.Pow((object.transform.position.x - cur.x),2) + Mathf.Pow ((object.transform.position.y - cur.y),2));
			    if (distance < closestDistance) {
					closestDistance = distances;
					closest = object;
				}
			}
			bool xComparison = Mathf.Abs (closest.transform.position.x - cur.x) < float.Epsilon;
			bool yComparison = Mathf.Abs (closest.transform.position.y - cur.y) < float.Epsilon;
			if (xComparison && yComparison) {
				actions.Add (new AIAction(e, AIAction.Actions.Attack, closest.transform.position));
			}
			else {
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
