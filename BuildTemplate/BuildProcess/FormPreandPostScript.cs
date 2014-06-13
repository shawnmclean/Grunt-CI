/*-------------------------------------------------------------------------
Copyright 2013 Microsoft Open Technologies, Inc.
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
--------------------------------------------------------------------------
*/
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
    public sealed class FormPreandPostScript : CodeActivity
    {
        public InArgument<string> BuildPath { get; set; }
        public InArgument<string> PreScript { get; set; }
        public InArgument<string> PostScript { get; set; }

        const string PreScriptFile = "PreScript.sh";
        const string PostScriptFile = "PostScript.sh";
        const string winScriptFile = "DownloadBuildBinariesFromAzureStorage.ps1";

        protected override void Execute(CodeActivityContext context)
        {
            string PreScriptFileName = BuildPath.Get(context) + "\\" + PreScriptFile;
            string PostScriptFileName = BuildPath.Get(context) + "\\" + PostScriptFile;

            string wPreScriptFileName = BuildPath.Get(context) + "\\" + winScriptFile;

            string sPreScript = PreScript.Get(context);
            string sPostScript = PostScript.Get(context);

            if (!sPreScript.Equals(string.Empty))
            {

                var str = File.ReadAllText(wPreScriptFileName);
                str = str.Replace("#PreScript", sPreScript);
                File.WriteAllText(wPreScriptFileName, str);

                if (!File.Exists(PreScriptFileName))
                    File.Delete(PreScriptFileName);

                FileStream fsScript = new FileStream(PreScriptFileName, FileMode.Create, FileAccess.Write);

                using (StreamWriter writer = new StreamWriter(fsScript))
                {
                    writer.Write(sPreScript);
                    writer.Close();
                }
            }

            if (!sPostScript.Equals(string.Empty))
            {
                var str = File.ReadAllText(wPreScriptFileName);
                str = str.Replace("#PostScript", sPostScript);
                File.WriteAllText(wPreScriptFileName, str);

                if (!File.Exists(PostScriptFileName))
                    File.Delete(PostScriptFileName);

                FileStream fsScript = new FileStream(PostScriptFileName, FileMode.Create, FileAccess.Write);

                using (StreamWriter writer = new StreamWriter(fsScript))
                {
                    writer.Write(sPostScript);
                    writer.Close();
                }
            }
        }
    }
}