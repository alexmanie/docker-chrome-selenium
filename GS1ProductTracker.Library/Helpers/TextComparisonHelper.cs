using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GS1ProductTracker.Shared.Helpers
{
    public static class TextComparisonHelper
    {
        //El orden de los elementos importa
        private static List<string> stopWords = new List<string>
        {
            "lt",
            "cm3",
            "c3",
            "cl",
            "kj",
            "kcal",
            "kg",
            "gr",
            "mg",
            "m3",
            "me",
            "hl",
            "ml",
            "g",
            "l",
            "m",
            "µ"
        };

        public static string CleanTextNumeric(string text)
        {
            foreach (var word in stopWords)
            {
                text = text.Replace(word, string.Empty);
            }

            decimal number;

            //Hacemos un trim para quitar los espacios despues de limpiar 
            text = text.Trim();
            var isNumeric = decimal.TryParse(text, out number);

            //Sustituimos las , por .
            if (isNumeric)
            {
                var textReplaceDot = text.Replace('.', ',');
                var numberReplaceDot = decimal.Parse(textReplaceDot);
                var textReplaceComma = text.Replace(',', '.');
                var numberReplaceComma = decimal.Parse(textReplaceComma);

                if(numberReplaceDot <= numberReplaceComma)
                {
                    number = numberReplaceDot;
                }
                else
                {
                    number = numberReplaceComma;
                }
            }

            if (isNumeric)
            {
                text = number.ToString("G29");
            }


            return text.Trim();
        }

        public static string CleanText(string text, bool isIngredient = false, bool cleanAccents = false, bool cleanPunctuation = false)
        {
            if (isIngredient)
            {
                text = text.Replace("ingredientes:", "");
                text = text.Replace("ingredientes :", "");
            }

            if (cleanAccents)
            {
                //Eliminamos acentos
                if (string.IsNullOrWhiteSpace(text))
                    return text;

                text = text.Normalize(NormalizationForm.FormD);
                var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
                text = new string(chars).Normalize(NormalizationForm.FormC);
            }

            if (cleanPunctuation)
            {
                text = text.Normalize(NormalizationForm.FormD);
                var chars = text.Where(c => !char.IsPunctuation(c)).ToArray();
                text = new string(chars).Normalize(NormalizationForm.FormC);
            }

            //Quitamos espacios en blanco, y saltos de linea
            return Regex.Replace(text, @"\s+|\r\n?|\n", "");
        }
    }
}
