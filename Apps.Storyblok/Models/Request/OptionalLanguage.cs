using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Storyblok.Models.Request
{
    public class OptionalLanguage
    {
        [Display("Language")]
        public string? Language { get; set; }
    }
}
