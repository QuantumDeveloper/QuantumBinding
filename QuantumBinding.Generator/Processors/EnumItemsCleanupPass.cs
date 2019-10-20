using QuantumBinding.Generator.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumBinding.Generator.Processors
{
    public class EnumItemsCleanupPass : PreGeneratorPass
    {
        public EnumItemsCleanupPass()
        {
            Options.VisitEnums = true;
        }

        public override bool VisitEnum(Enumeration enumeration)
        {
            if (IsVisited(enumeration))
                return false;

            if (enumeration.Name == "VkSharingMode")
            {

            }

            var groupItems = enumeration.Items.GroupBy(x => x.Value).Where(group => group.Count() > 1);

            if (groupItems.Any())
            {
                foreach(var group in groupItems)
                {
                    var items = group.Select(x => x).ToList();

                    for (int i = 1; i < items.Count; ++i)
                    {
                        items[i].IsIgnored = true;
                    }
                }
            }

            return true;
        }
    }
}
