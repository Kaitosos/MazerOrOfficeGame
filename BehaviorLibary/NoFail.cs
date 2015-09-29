using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorLibary
{
    public class NoFail : IBehavior
    {
        public IBehavior Behavior;

        public NoFail()
        {
            this.Behavior = null;
        }

        public NoFail(IBehavior behavior)
        {
            this.Behavior = behavior;
        }

        public BehaviorState Behave()
        {
            if (Behavior == null)
                return BehaviorState.Success;

            switch(this.Behavior.Behave())
            {
                case BehaviorState.Running:
                    return BehaviorState.Running;
                    
                case BehaviorState.Fail:
                default:
                    return BehaviorState.Success;
            }
        }
    }
}
