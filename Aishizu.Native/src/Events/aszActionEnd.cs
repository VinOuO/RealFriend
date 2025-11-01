using Aishizu.Native.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aishizu.Native.Events
{
    internal class aszActionEnd : aszIEvent
    {
        public int actionId { get; set; }
    }
}
