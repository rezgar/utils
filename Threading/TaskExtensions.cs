using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rezgar.Utils.Threading
{
    public static class TaskExtensions
    {
        public static bool IsCompletedSuccessfully(this Task task)
        {
            return task.Status == TaskStatus.RanToCompletion;
        }
    }
}
