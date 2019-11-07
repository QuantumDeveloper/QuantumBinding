using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var enumItems = enumeration.Items.Where(x => !x.IsIgnored).ToList();

            var basicItem = enumItems.FirstOrDefault();
            if (basicItem == null) return false;

            var basicName = basicItem.Name;
            var finalName = string.Empty;
            List<string> shortenedNames = new List<string>();

            foreach (var item in enumItems)
            {
                if (basicName == item.Name || item.IsIgnored) continue;

                var name = item.Name;
                var tempName = string.Empty;
                for (int i = 0; i < name.Length; ++i)
                {
                    if (i > basicName.Length - 1 || name[i] != basicName[i]) break;

                    tempName += name[i];
                }

                shortenedNames.Add(tempName);
            }


            finalName = shortenedNames.OrderBy(x => x.Length).FirstOrDefault();

            if (enumItems.Count > 1 && !string.IsNullOrEmpty(finalName))
            {
                foreach (var enumItem in enumItems)
                {
                    enumItem.Name = enumItem.Name.Replace(finalName, "");
                }
            }

            if (excludedEnums.Contains(enumeration.Name))
            {
                return false;
            }

            foreach (var enumItem in enumItems)
            {
                var splitted = enumItem.Name.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Length > 1 || (splitted.Length == 1 && HasUniformRegister(splitted[0])))
                {
                    var finalItem = new StringBuilder();
                    RenameItem(splitted, finalItem);
                    enumItem.Name = finalItem.ToString();
                }

                if (char.IsDigit(enumItem.Name[0]))
                {
                    enumItem.Name = separator + enumItem.Name;
                }

                if (CodeGenerator.ReservedWords.Contains(enumItem.Name))
                {
                    enumItem.Name = $"@{enumItem.Name}";
                }
            }

            //Remove duplicated items if exists after renaming pass
            for (int i = enumItems.Count - 1; i >= 0; i--)
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

        private void RenameItem (string[] enumItems, StringBuilder builder)
        {
            for (int i = 0; i < enumItems.Length; i++)
            {
                bool isUpper = false;
                var item = enumItems[i];

                for (int j = 0; j < item.Length; ++j)
                {
                    if (j == 0)
                    {
                        if (casePattern == CasePattern.CamelCase && i > 0)
                        {
                            builder.Append(Char.ToLower(item[0]));
                            isUpper = true;
                        }
                        else
                        {
                            isUpper = true;
                            builder.Append(Char.ToUpper(item[0]));
                        }
                    }
                    else
                    {
                        if (Char.IsUpper(item[j]) && isUpper)
                        {
                            builder.Append(Char.ToLower(item[j]));
                        }
                        else
                        {
                            builder.Append(item[j]);
                        }
                    }
                }
            }
        }

        private bool HasUniformRegister(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            return value == value.ToLower() || value == value.ToUpper();
        }

    }
}