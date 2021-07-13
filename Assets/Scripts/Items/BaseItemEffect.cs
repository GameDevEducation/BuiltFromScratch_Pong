using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemEffect : ScriptableObject
{
    public bool AppliesToLastTouchedBy = false;
    public bool AppliesToOpponent = true;

    [SerializeField] protected float Duration = 10f;

    [System.NonSerialized] float TimeActive = 0f;

    public bool IsActive => TimeActive < Duration;

    public virtual void TickEffect()
    {
        TimeActive += Time.deltaTime;
    }

    public virtual float ModifyInput(float currentInput)
    {
        return currentInput;
    }
}
