namespace QuantumBinding.Generator.Utils
{
    public static class RenameTargetsUtils
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
        
        public static RenameTargets ClassTypeToRenameTargets(ClassType type)
        {
            switch (type)
            {
                case ClassType.Class:
                    return RenameTargets.Class;
                case ClassType.Struct:
                    return RenameTargets.Struct;
                case ClassType.Union:
                    return RenameTargets.Union;
                case ClassType.StructWrapper:
                    return RenameTargets.StructWrapper;
                case ClassType.UnionWrapper:
                    return RenameTargets.UnionWrapper;
            }

            return RenameTargets.None;
        }
    }
}
