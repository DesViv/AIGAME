﻿using UnityEngine;
using System.Collections;
using Completed;
using System;

public class AISimple : AIBase { //This is a simple AI unit that prefers to move to the right and down
    public override void onTurn()
    {
        base.onTurn();
        foreach(Enemy e in self) //computes for each unit
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
            if(!fin)
                actions.Add(new AIAction(e, AIAction.Actions.Move, cur + new Vector3(1, 1, 0)));
        }
    }
}
