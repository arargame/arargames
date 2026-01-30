using System;

namespace ArarGames.Core.Utils
{
    public class Timer
    {
        public float Duration { get; set; }
        public float ElapsedTime { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsLooping { get; set; }

        public event Action OnCompleted;

        public Timer(float duration, bool isLooping = false)
        {
            Duration = duration;
            IsLooping = isLooping;
        }

        public void Start()
        {
            ElapsedTime = 0;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Update(float deltaTime)
        {
            if (!IsRunning) return;

            ElapsedTime += deltaTime;
            if (ElapsedTime >= Duration)
            {
                OnCompleted?.Invoke();
                if (IsLooping)
                {
                    ElapsedTime -= Duration;
                }
                else
                {
                    IsRunning = false;
                    ElapsedTime = 0;
                }
            }
        }
    }
}
