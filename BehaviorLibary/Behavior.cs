using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorLibary
{
    public class Behavior : IBehavior
    {
        private Task task;

        public Behavior()
        {
            this.task = null;
        }

        public Behavior(Task task)
        {
            this.task = task;
        }

        public BehaviorState Behave()
        {
            if (task == null)
                return BehaviorState.Fail;
            else
                return task();
        }
    }
}
