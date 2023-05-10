using System;
using System.Collections.Generic;
using System.Text;

namespace FileUtility.Model
{
    public class FileDetails
    {
        private string nameValue;
        public string Name
        {
            get
            {
                return nameValue;
            }
            set
            {
                nameValue = value;
            }
        }
        public string DestinationPath { get; set; }
        public string SourcePath { get; set; }
    }
}
