using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Storyblok.Models.Request.Story
{
    public class ImportRequest
    {
        [Display("Translated content")]
        public File Content { get; set; }
    }
}
