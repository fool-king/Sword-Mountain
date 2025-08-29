// TaskState.cs
using UnityEngine;

public class TaskState : MonoBehaviour
{
    public enum State
    {
        None,
        Available,
        InProgress,
        Completed
    }

    public State currentState = State.None;

    // 这个方法很重要！DialogueManager会调用它
    public State GetState()
    {
        return currentState;
    }

    public void SetState(State newState)
    {
        currentState = newState;
        HandleStateChange(newState);
    }

    private void HandleStateChange(State newState)
    {
        Debug.Log($"任务状态改变: {newState}");
        // 状态改变时的处理逻辑
    }
}