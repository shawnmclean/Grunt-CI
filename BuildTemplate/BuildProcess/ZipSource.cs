using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.IO.Compression;

namespace GruntBuildProcess
{
    public sealed class ZipSource : CodeActivity<string>
    {
        public InArgument<string> BuildPath { get; set; }
        public OutArgument<string> RARFileName { get; set; }
        string ZipedFile = string.Empty;

        protected override string Execute(CodeActivityContext context)
        {
            string fileName = string.Format("GruntBuiltSource-{0:yyyy-MM-dd_hh-mm-ss-tt}.rar",
        DateTime.Now);
            ZipedFile = BuildPath.Get(context) + fileName;
            RARFileName.Set(context, fileName);

            FileInfo buidir = new FileInfo(BuildPath.Get(context));

            if (File.Exists(ZipedFile))
                File.Delete(ZipedFile);

            ZipFile.CreateFromDirectory(BuildPath.Get(context), ZipedFile);
            return ZipedFile;
        }
    }
}