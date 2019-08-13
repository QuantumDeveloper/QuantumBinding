using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator.Processors
{
    public class EnumItemsRenamePass : PreGeneratorPass
    {
        private CasePattern casePattern;
        private char separator = '_';
        private string[] excludedEnums;

        public EnumItemsRenamePass(CasePattern casePattern, params string[] excludedEnums)
        {
            this.casePattern = casePattern;
            Options.VisitEnums = true;
            this.excludedEnums = excludedEnums;
        }

        public override bool VisitEnum(Enumeration enumeration)
        {
            if (IsVisited(enumeration))
                return false;

            var list = new List<string>(enumeration.Items[0].Name.Split(separator, StringSplitOptions.RemoveEmptyEntries));
            foreach (var item in enumeration.Items)
            {
                var splitted = item.Name.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var maxIterations = Math.Min(splitted.Length, list.Count);
                for (int i = 0; i < maxIterations; i++)
                {
                    var original = list[i];
                    var compared = splitted[i];
                    if (!string.Equals(original, compared, StringComparison.Ordinal))
                    {
                        var count = list.Count - i;
                        list.RemoveRange(i, count);
                        break;
                    }
                }
            }

            var removePart = string.Join(separator, list);

            if (enumeration.Items.Count > 1 && !string.IsNullOrEmpty(removePart))
            {
                foreach (var enumItem in enumeration.Items)
                {
                    enumItem.Name = enumItem.Name.Replace(removePart+separator, "");
                }
            }

            if (excludedEnums.Contains(enumeration.Name))
            {
                return false;
            }

            foreach (var enumItem in enumeration.Items)
            {
                var splitted = enumItem.Name.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                string finalItem = String.Empty;
                for (int i = 0; i < splitted.Length; i++)
                {
                    var item = splitted[i];
                    if (casePattern == CasePattern.CamelCase && i == 0)
                    {
                        finalItem += item.ToLower();
                    }
                    else
                    {
                        finalItem += item[0].ToString().ToUpper() + item.Substring(1).ToLower();
                    }
                }
                enumItem.Name = finalItem;

                if (char.IsDigit(enumItem.Name[0]))
                {
                    enumItem.Name = separator + enumItem.Name;
                }

                if (CodeGenerator.ReservedWords.Contains(enumItem.Name))
                {
                    enumItem.Name = $"@{enumItem.Name}";
                }
            }

            for (int i = enumeration.Items.Count - 1; i >= 0; i--)
            {
                var item = enumeration.Items[i];
                var result = enumeration.Items.Where(x => x.Name == item.Name && x.Value == item.Value).ToList();
                if (result.Count > 1)
                {
                    for (int j = 1; j < result.Count; j++)
                    {
                        result[j].Name += j;
                    }
                }
            }

            return true;
        }
    }
}