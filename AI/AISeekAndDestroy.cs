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
                        actions.Add(new AIAction(e, AIAction.Actions.Attack, g.transform.position));
                    }
                }
                }

            }
        }
    }
}