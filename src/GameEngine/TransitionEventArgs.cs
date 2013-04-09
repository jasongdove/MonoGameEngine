using System;

namespace GameEngine
{
    public class TransitionEventArgs : EventArgs
    {
        private readonly float _percent;
        private readonly float _time;

        public TransitionEventArgs(float percent, float time)
        {
            _percent = percent;
            _time = time;
        }

        public float Percent
        {
            get { return _percent; }
        }

        public float Time
        {
            get { return _time; }
        }
    }
}