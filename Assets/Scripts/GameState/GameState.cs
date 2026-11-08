using UnityEngine;

public abstract class GameState : MonoBehaviour
{
    protected void SwitchState(GameState nextState)
    {
        enabled = false;
        nextState.enabled = true;
    }
}
