using Aishizu.Native.Actions;
using System.Collections.Generic;

namespace Aishizu.Native.Sequencer
{
    public interface aszISequencer
    {
        void Enqueue(aszAction action);
        void Enqueue(IEnumerable<aszAction> actions);
        void Play();
        void Pause(bool isPaused);
        void Stop();
    }
}
