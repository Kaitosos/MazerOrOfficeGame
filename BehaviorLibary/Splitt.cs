using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorLibary
{
    public class Splitt : IBehavior
    {
        public IBehavior[] Behaviors;

        public Splitt(int count)
        {
            this.Behaviors = new IBehavior[count];
        }

        public bool AddBehavior(int position, IBehavior behavior)
        {
            if (position < 0 || position > this.Behaviors.Length)
                return false;
            if (behavior == null)
                return false;
            return true;
        }

        public BehaviorState Behave()
        {
            foreach(IBehavior iB in this.Behaviors)
            {
                if(iB != null)
                {
                    switch(iB.Behave())
                    {
                        case BehaviorState.Running:
                            return BehaviorState.Running;
                        case BehaviorState.Fail:
                            return BehaviorState.Fail;
                    }
                }
            }
            return BehaviorState.Success;
        }
    }
}
