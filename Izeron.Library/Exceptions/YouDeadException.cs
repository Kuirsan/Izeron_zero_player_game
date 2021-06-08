using System;
using System.Collections.Generic;
using System.Text;

namespace Izeron.Library.Exceptions
{
    [Serializable]
    public class YouDeadException:Exception
    {
       public YouDeadException(string message):base(message)
       {
       }
    }
}
