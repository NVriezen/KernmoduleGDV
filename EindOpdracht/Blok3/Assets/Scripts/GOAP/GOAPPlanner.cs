using System.Collections.Generic;
using UnityEngine;

public class GOAPPlanner
{
    public Queue<GOAPAction> Plan(
                                    GameObject agent, 
                                    HashSet<GOAPAction> availableActions,
                                    HashSet<KeyValuePair<string, object>> worldState,
                                    HashSet<KeyValuePair<string, object>> goal
                                 )
    {
        foreach(GOAPAction a in availableActions)
        {
            a.ResetAction();
        }

        HashSet<GOAPAction> usableActions = new HashSet<GOAPAction>();
        foreach(GOAPAction a in availableActions)
        {
            if (a.CheckProceduralPrecondition(agent))
            {
                usableActions.Add(a);
            }
        }

        List<Node> leaves = new List<Node>();

        Node start = new Node(null, 0, worldState, null);
        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            return null;
        }

        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if (cheapest == null){
                cheapest = leaf;
            } else
            {
                if (leaf.runningCost < cheapest.runningCost)
                {
                    cheapest = leaf;
                }
            }
        }

        List<GOAPAction> result = new List<GOAPAction>();
        Node n = cheapest;
        while(n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<GOAPAction> queue = new Queue<GOAPAction>();
        foreach (GOAPAction a in result)
        {
            queue.Enqueue(a);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, HashSet<GOAPAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
    {
        bool foundOne = false;

        foreach (GOAPAction action in usableActions)
        {
            if(InState(action.Preconditions, parent.state))
            {
                HashSet<KeyValuePair<string, object>> currentState = PopulateState(parent.state, action.Effects);
                Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);
                
                if (InState(goal, currentState))
                {
                    leaves.Add(node);
                    foundOne = true;
                } else
                {
                    HashSet<GOAPAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                    {
                        foundOne = true;
                    }
                }
            }
        }
        return foundOne;
    }

    private HashSet<GOAPAction> ActionSubset(HashSet<GOAPAction> actions, GOAPAction remove)
    {
        HashSet<GOAPAction> subset = new HashSet<GOAPAction>();
        foreach (GOAPAction a in actions)
        {
            if (!actions.Equals(remove))
            {
                subset.Add(a);
            }
        }
        return subset;
    }

    private HashSet<KeyValuePair<string, object>> PopulateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange)
    {
        HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();

        foreach(KeyValuePair<string, object> s in currentState)
        {
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string, object> change in stateChange)
        {
            bool exist = false;

            foreach (KeyValuePair<string, object> s in state)
            {
                if (s.Equals(change))
                {
                    exist = true;
                    break;
                }
            }

            if (exist)
            {
                state.RemoveWhere((KeyValuePair<string, object> k) => { return k.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                state.Add(updated);
            } else
            {
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }

    private bool InState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
    {
        bool allMatch = true;

        foreach(KeyValuePair<string, object> t in test)
        {
            bool match = false;
            foreach(KeyValuePair<string, object> s in state)
            {
                if (s.Equals(t))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                allMatch = false;
            }
        }
        return allMatch;
    }

    private class Node
    {
        public Node parent;
        public float runningCost;
        public HashSet<KeyValuePair<string, object>> state;
        public GOAPAction action;

        public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, object>> state, GOAPAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }
}
