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

            if (enumeration.Name == "Color_Space_KHR")
            {

            }

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
                var finalItem = new StringBuilder();
                for (int i = 0; i < splitted.Length; i++)
                {
                    bool isUpper = false;
                    var item = splitted[i];

                    for (int j = 0; j < item.Length; ++j)
                    {
                        if (j == 0)
                        {
                            if (casePattern == CasePattern.CamelCase && i > 0)
                            {
                                finalItem.Append(Char.ToLower(item[0]));
                                isUpper = true;
                            }
                            else
                            {
                                isUpper = true;
                                finalItem.Append(Char.ToUpper(item[0]));
                            }
                        }
                        else
                        {
                            if (Char.IsUpper(item[j]) && isUpper)
                            {
                                finalItem.Append(Char.ToLower(item[j]));
                            }
                            else
                            {
                                finalItem.Append(item[j]);
                            }
                        }
                    }
                }

                enumItem.Name = finalItem.ToString();

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
    }
}