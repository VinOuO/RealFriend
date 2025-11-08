using Aishizu.Native.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aishizu.Native.Events
{
    public class aszWait : aszEvent
    {
        public float Duration { get; set; }
        public aszWait() 
        {
            Name = "Wait";
        }
    }
}
