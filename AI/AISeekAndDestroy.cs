using UnityEngine;
using System.Collections;
using Completed;

public class AISeekAndDestroy : AIBase
{
    public override void onTurn()
    {
        base.onTurn();

        foreach (Enemy e in self)
        {
            Vector3 cur = e.currentPos;

            bool fin = false;
            e.showValidAttack();

            foreach (GameObject g in e.validAttack)
            {
                foreach (MovingObject mo in other)
                {
                    if (!fin)
                    {
                        if (g.transform.position.Equals(mo.currentPos)) { }
                        actions.Add(new AIAction(e, AIAction.Actions.Attack, g.transform.position));
                        Debug.Log("Attack");
                        fin = true;
                    }
                }
            }
            if (!fin)
            {
                int xDir = 0;
                int yDir = 0;
                MovingObject closest = other[0];
                float closestDistance = Mathf.Sqrt(Mathf.Pow((closest.currentPos.x - cur.x), 2) + Mathf.Pow((closest.currentPos.y - (int)cur.y), 2));
                foreach (MovingObject obj in other)
                {
                    float distance = Mathf.Sqrt(Mathf.Pow((obj.transform.position.x - cur.x), 2) + Mathf.Pow((obj.transform.position.y - cur.y), 2));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = obj;
                    }
                }
                bool xComparison = Mathf.Abs(closest.transform.position.x - cur.x) < float.Epsilon;
                bool yComparison = Mathf.Abs(closest.transform.position.y - cur.y) < float.Epsilon;
                if (xComparison && yComparison)
                {
                    actions.Add(new AIAction(e, AIAction.Actions.Attack, closest.transform.position));
                }
                else
                {
                    if (xComparison)
                    {
                        yDir = closest.transform.position.y > cur.y ? 1 : -1;
                    }
                    else
                    {
                        xDir = closest.transform.position.x > cur.x ? 1 : -1;
                    }
                    actions.Add(new AIAction(e, AIAction.Actions.Move, cur + new Vector3(xDir, yDir, 0)));
                }

            }
        }
    }
}