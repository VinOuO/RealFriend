using Aishizu.Native.Actions;
using System.Collections.Generic;

namespace Aishizu.Native.Sequencer
{
    public interface aszISequencer
    {
        void Enqueue(aszIAction action);
        void Enqueue(IEnumerable<aszIAction> actions);
        void Play();
        void Pause(bool isPaused);
        void Stop();
    }
}
