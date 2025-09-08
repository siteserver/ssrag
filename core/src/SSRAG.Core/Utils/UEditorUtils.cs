using SSRAG.Core.StlParser.StlElement;

namespace SSRAG.Core.Utils
{
    public static class UEditorUtils
    {
        public static string TranslateToStlElement(string html)
        {
            var retVal = html;
            if (!string.IsNullOrEmpty(retVal))
            {
                retVal = retVal.Replace($"<img {StlPlayer.EditorPlaceHolder1} ", $"<{StlPlayer.ElementName} ");
                retVal = retVal.Replace($"<img {StlPlayer.EditorPlaceHolder2} ", $"<{StlPlayer.ElementName} ");
                retVal = retVal.Replace($"<img {StlPlayer.EditorPlaceHolder3} ", $"<{StlPlayer.ElementName} ");
                retVal = retVal.Replace($"<img {StlPlayer.EditorPlaceHolder4} ", $"<{StlPlayer.ElementName} ");
                retVal = retVal.Replace($"<img {StlAudio.EditorPlaceHolder1} ", $"<{StlAudio.ElementName} ");
                retVal = retVal.Replace($"<img {StlAudio.EditorPlaceHolder2} ", $"<{StlAudio.ElementName} ");
            }
            return retVal;
        }

        public static string TranslateToHtml(string html)
        {
            var retVal = html;
            if (!string.IsNullOrEmpty(retVal))
            {
                retVal = retVal.Replace($"<{StlPlayer.ElementName} ", $"<img {StlPlayer.EditorPlaceHolder1} ");
                retVal = retVal.Replace($"<{StlAudio.ElementName} ", $"<img {StlAudio.EditorPlaceHolder1} ");
            }
            return retVal;
        }
    }
}
