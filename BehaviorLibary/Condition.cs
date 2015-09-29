using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorLibary
{
    public class Condition : IBehavior
    {
        public IBehavior Condition;
        public IBehavior Task;

        public Condition()
        {
            this.Condition = new Behavior();
            this.Task = new Behavior();
        }

        public Condition(IBehavior condition, IBehavior task)
        {
            this.Condition = condition;
            this.Task = task;
        }

        public BehaviorState Behave()
        {
            if (Condition.Behave() == BehaviorState.Success)
                return Task.Behave();
            return BehaviorState.Fail;
        }
    }
}
