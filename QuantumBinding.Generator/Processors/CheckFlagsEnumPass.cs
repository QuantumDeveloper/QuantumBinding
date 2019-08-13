﻿using System;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public class CheckFlagEnumsPass : PreGeneratorPass
    {
        public CheckFlagEnumsPass()
        {
            Options.VisitEnums = true;
        }

        public override bool VisitEnum(Enumeration enumeration)
        {
            if (IsVisited(enumeration))
                return false;

            foreach (var item in enumeration.Items)
            {
                try
                {
                    var value = Convert.ToInt32(item.Value);

                    if (value < 0)
                    {
                        return false;
                    }

                    if (!Utils.IsPowerOfTwo((uint)value) && value != int.MaxValue)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            enumeration.IsFlagEnum = true;

            return true;
        }
    }
}