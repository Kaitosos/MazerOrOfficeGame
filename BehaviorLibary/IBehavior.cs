using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorLibary
{
    private interface IBehavior
    {
        BehaviorState Behave();
    }
}
