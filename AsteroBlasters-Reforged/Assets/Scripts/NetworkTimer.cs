/// <summary>
/// Class being the special timer, ticking by the fixed amount of time, determinated by the tick rate given to the constructor.
/// </summary>
public class NetworkTimer
{
    float timer;
    public float MinTimeBetweenTicks { get; }
    public int CurrentTick { get; private set; }

    /// <summary>
    /// Special contructor for Network Timer, which calculates its minimum time between ticks.
    /// </summary>
    /// <param name="serverTickRate">Amount of ticks per frame</param>
    public NetworkTimer(float serverTickRate)
    {
        MinTimeBetweenTicks = 1f / serverTickRate;
    }

    /// <summary>
    /// Method updating the the timer.
    /// </summary>
    /// <param name="deltaTime">Value of Time.deltaTime parameter</param>
    public void Update(float deltaTime)
    {
        timer += deltaTime;
    }

    /// <summary>
    /// Method checking if the timer crossed the minTimeBetweenTicks parameter, if so decreasing it's value, incrementing the Current Tick parameter
    /// </summary>
    /// <returns>True if the timer crossed the minTimeBetweenTicks parameter otherwise returns false</returns>
    public bool ShouldTick()
    {
        if (timer >= MinTimeBetweenTicks)
        {
            timer -= MinTimeBetweenTicks;
            CurrentTick++;
            return true;
        }

        return false;
    }
}
