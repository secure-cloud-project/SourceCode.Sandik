using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sandik.GuvenliDepolama.Models
{
    public class UserFile
    {
        public int UserID { get; set; }
        public string FileNameOrjinal { get; set; }

        public string FileNameGuid { get; set; }

        public string FilePath { get; set; }

        public Int64 FileSizeByte { get; set; }

        public int ID { get; set; }

        public int IsDeleted { get; set; }

    }
}