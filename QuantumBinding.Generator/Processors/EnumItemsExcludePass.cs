using System;
using System.Collections.Generic;
using System.Globalization;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public enum ExcludeCheck
    {
        StartsWithStrict,
        StartsWithIgnoreCase,
        ContainsStrict,
        ContainsIgnoreCase
    }

    public class EnumItemsExcludePass : PreGeneratorPass
    {
        private readonly string[] excludedItems;
        private ExcludeCheck excludeItemsCheck;

        public EnumItemsExcludePass(ExcludeCheck excludeItemsCheck, params string[] excludedItems)
        {
            Options.VisitEnums = true;
            this.excludedItems = excludedItems;
            this.excludeItemsCheck = excludeItemsCheck;
        }

        public override bool VisitEnum(Enumeration enumeration)
        {
            if (IsVisited(enumeration))
                return false;

            foreach (var enumItem in enumeration.Items)
            {
                if (excludedItems != null && excludedItems.Length > 0)
                {
                    foreach (var item in excludedItems)
                    {
                        bool result = false;
                        if (excludeItemsCheck == ExcludeCheck.StartsWithStrict)
                        {
                            result = enumItem.Name.StartsWith(item);
                        }
                        else if (excludeItemsCheck == ExcludeCheck.StartsWithIgnoreCase)
                        {
                            result = enumItem.Name.StartsWith(item, true, CultureInfo.InvariantCulture);
                        }
                        else if (excludeItemsCheck == ExcludeCheck.ContainsStrict)
                        {
                            result = enumItem.Name.Contains(item);
                        }
                        else if (excludeItemsCheck == ExcludeCheck.ContainsIgnoreCase)
                        {
                            result = enumItem.Name.Contains(item, StringComparison.OrdinalIgnoreCase);
                        }

                        if (result)
                        {
                            enumItem.IsIgnored = true;
                            break;
                        }
                    }
                }
            }

            return true;
        }
    }
}