using Aishizu.Native.Actions;
using System.Collections.Generic;

namespace Aishizu.Native.Sequencer
{
    public interface ISequencer
    {
        void Enqueue(IAction action);
        void Enqueue(IEnumerable<IAction> actions);
        void Play();
        void Pause(bool isPaused);
        void Stop();
    }
}
