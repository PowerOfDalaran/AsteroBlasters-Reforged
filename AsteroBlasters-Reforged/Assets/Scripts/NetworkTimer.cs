public class NetworkTimer
{
    float timer;
    public float MinTimeBetweenTicks { get; }
    public int currentTick { get; private set; }

    public NetworkTimer(float serverTickRate)
    {
        MinTimeBetweenTicks = 1f / serverTickRate;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;
    }

    public bool ShouldTick()
    {
        if (timer >= MinTimeBetweenTicks)
        {
            timer -= MinTimeBetweenTicks;
            currentTick++;
            return true;
        }

        return false;
    }
}
