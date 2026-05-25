using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Extensions;

namespace QuantumBinding.Generator.Processors;

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

        var firstItem = enumItems.FirstOrDefault();
        if (firstItem == null) return false;
        
        string firstName = firstItem.Name;
        int commonLength = firstName.Length;

        if (enumItems.Count > 1)
        {
            foreach (var item in enumItems.Skip(1))
            {
                int j = 0;
                while (j < commonLength && j < item.Name.Length && firstName[j] == item.Name[j])
                {
                    j++;
                }

                commonLength = j;
                if (commonLength == 0) break;
            }
        }
        else
        {
            string typePrefix = enumeration.Name.ToSnakeCase();
            
            int j = 0;
            while (j < firstName.Length && j < typePrefix.Length && firstItem.Name[j] == typePrefix[j])
            {
                j++;
            }
    
            int lastUnderscoreInMatch = firstName.Substring(0, j).LastIndexOf('_');
            commonLength = (lastUnderscoreInMatch != -1) ? lastUnderscoreInMatch + 1 : j;
        }

        if (commonLength > 0)
        {
            string fullPrefix = firstName.Substring(0, commonLength);
            int lastUnderscore = fullPrefix.LastIndexOf('_');
        
            int finalPrefixLength = (lastUnderscore != -1) ? lastUnderscore + 1 : commonLength;

            foreach (var enumItem in enumItems)
            {
                enumItem.Name = enumItem.Name.Substring(finalPrefixLength);
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