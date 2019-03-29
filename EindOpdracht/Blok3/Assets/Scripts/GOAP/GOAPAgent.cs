using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GOAPAgent : MonoBehaviour
{
    private FSM stateMachine;

    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;

    private HashSet<GOAPAction> availableActions;
    private Queue<GOAPAction> currentActions;

    private IGOAP dataProvider;

    private GOAPPlanner planner;

    private void Start()
    {
        stateMachine = new FSM();
        availableActions = new HashSet<GOAPAction>();
        currentActions = new Queue<GOAPAction>();
        planner = new GOAPPlanner();
    }

    private void Update()
    {
        stateMachine.Update(this.gameObject);
    }

    public void AddAction(GOAPAction a)
    {
        availableActions.Add(a);
    }

    public GOAPAction GetAction(Type action)
    {
        foreach(GOAPAction a in availableActions)
        {
            if (gameObject.GetType().Equals(action))
            {
                return a;
            }
        }
        return null;
    }

    public void RemoveAction(GOAPAction action)
    {
        availableActions.Remove(action);
    }

    private bool HasActionPlan()
    {
        return currentActions.Count > 0;
    }

    private void CreateIdleState()
    {
        idleState = (fsm, gameObj) =>
        {
            HashSet<KeyValuePair<string, object>> worldState = dataProvider.GetWorldState();
            HashSet<KeyValuePair<string, object>> goal = dataProvider.CreateGoalState();

            Queue<GOAPAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);
            if (plan != null)
            {
                currentActions = plan;
                dataProvider.PlanFound(goal, plan);
                fsm.PopState();
                fsm.PushState(performActionState);
            }
            else
            {
                dataProvider.PlanFailed(goal);
                fsm.PopState();
                fsm.PushState(idleState);
            }
        };
    }

    private void CreateMoveToState()
    {
        moveToState = (fsm, gameObj) =>
        {
            GOAPAction action = currentActions.Peek();
            if (action.RequiresInRange() && action.target == null)
            {
                fsm.PopState(); //move
                fsm.PopState(); //perform
                fsm.PushState(idleState);
                return;
            }

            if (dataProvider.MoveAgent(action))
            {
                fsm.PopState();
            }

            //MovableComponent movable = (MovableComponent)gameObj.GetComponent(typeof(MovableComponent));
            //if (movable == null)
            //{
            //    fsm.popState(); //move
            //    fsm.popState(); //perform
            //    fsm.pushState(idleState);
            //    return;
            //}

            //float step = movable.moveSpeed * Time.deltaTime;
            //gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, action.target.transform.position, step);

            //if (gameObj.transform.position.Equals(action.target.transform.position))
            //{
            //    action.SetInRange(true);
            //    fsm.popState();
            //}
        };
    }

    private void CreatePerformActionState()
    {
        performActionState = (fsm, gameObj) =>
        {
            if (!HasActionPlan())
            {
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
                return;
            }

            GOAPAction action = currentActions.Peek();
            if (action.IsDone())
            {
                currentActions.Dequeue();
            }

            if (HasActionPlan())
            {
                action = currentActions.Peek();
                bool inRange = true;
                if (action.RequiresInRange())
                {
                    inRange = action.IsInRange();
                }

                if (inRange)
                {
                    bool success = action.Perform(gameObj);

                    if (!success)
                    {
                        fsm.PopState();
                        fsm.PushState(idleState);
                        dataProvider.PlanAborted(action);
                    }
                } else
                {
                    fsm.PushState(moveToState);
                }
            } else
            {
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
            }
        };
    }

    private void FindDataProvider()
    {
        foreach(Component c in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(IGOAP).IsAssignableFrom(c.GetType()))
            {
                dataProvider = (IGOAP)c;
                return;
            }
        }
    }

    private void LoadActions()
    {
        GOAPAction[] actions = gameObject.GetComponents<GOAPAction>();
        foreach(GOAPAction a in actions)
        {
            availableActions.Add(a);
        }
    }
}
