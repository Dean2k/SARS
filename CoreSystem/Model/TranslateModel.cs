using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreSystem.Model
{
    public class TranslateModel
    {
        public string q { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public string format { get; set; }
        public string api_key { get; set; }
    }

    public class DetectedLanguage
    {
        public string confidence { get; set; }
        public string language { get; set; }
    }

    public class TranslateResult
    {
        public DetectedLanguage detectedLanguage { get; set; }
        public string translatedText { get; set; }
    }
}
