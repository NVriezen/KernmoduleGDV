using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    Title = 0,
    Register = 1,
    Login = 2,
    Gameplay = 3,
    GameEnd = 4
}

public class FiniteStateMachine : MonoBehaviour
{
    private Dictionary<GameState, FSMState> allStates = new Dictionary<GameState, FSMState>();
    private FSMState currentState;

    private void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public void ChangeState(GameState nextState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        if (allStates.ContainsKey(nextState))
        {
            FSMState state;
            if (allStates.TryGetValue(nextState, out state))
            {
                currentState = state;
                currentState.OnEnter();
            }
        }
    }
}
