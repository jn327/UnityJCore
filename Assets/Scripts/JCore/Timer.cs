public class Timer
{
    public float remainingTime 
    {
        get;
        private set;
    }

    public event System.Action onComplete;

    public Timer( float duration )
    {
        remainingTime = duration;
    }

    public void Tick(float deltaTime)
    {
        //if it's already reached 0 do nothing.
        if (remainingTime <= 0)
        {
            return;
        }

        //count down remaning time.
        remainingTime -= deltaTime;

        //if we've reached 0 then send off our complete event.
        if (remainingTime <= 0)
        {
            remainingTime = 0;

            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }
    }
}
