using System;
using UnityEngine;
using CodeMonkey;

public class TimeTickSystem : MonoBehaviour
{
    public static event EventHandler<OnTickEventArgs> OnTick;
    public class OnTickEventArgs : EventArgs
    {
        public int _tick;
    }

    private const float TICK_TIMER_MAX = .2f;

    private int tick;
    private float tickTimer;

    private void Awake()
    {
        tick = 0;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if(tickTimer >= TICK_TIMER_MAX)
        {
            tickTimer -= TICK_TIMER_MAX;
            tick++;
            OnTick?.Invoke(this, new OnTickEventArgs { _tick = tick });
        }
    }
}
