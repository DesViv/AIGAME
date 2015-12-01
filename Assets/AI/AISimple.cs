using UnityEngine;
using System.Collections;
using Completed;
using System;

public class AISimple : AIBase {
    public override void onTurn()
    {
        base.onTurn();
        foreach(Enemy e in self)
        {
            Vector3 cur = e.currentPos;
            actions.Add(new AIAction(e, AIAction.Actions.Move, cur + new Vector3(1, 1, 0)));
        }
    }
}
