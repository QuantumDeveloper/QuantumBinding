using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumBinding.Generator
{
    public static class RenameTargetsUtil
    {
        public static RenameTargets Any()
        {
            return RenameTargets.Any;
        }

        public static RenameTargets Except(this RenameTargets targets, RenameTargets except)
        {
            return targets & ~except;
        }

        public static RenameTargets AnyExcept(RenameTargets except)
        {
            return RenameTargets.Any & ~except;
        }
    }
}
