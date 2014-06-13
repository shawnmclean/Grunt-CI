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
    public sealed class CopyZIPUtil : CodeActivity<string>
    {
        public InArgument<string> BuildPath { get; set; }
        const string ZIPUtil = "7za920.zip";
        const string ZipExe = "7za.exe";

        protected override string Execute(CodeActivityContext context)
        {
            var ZIPExeFolderPath = BuildPath.Get(context) + "\\7zip\\";
            string[] Resourcenames = this.GetType().Assembly.GetManifestResourceNames();
            var ZIPExePath = ZIPExeFolderPath;

            foreach (string rName in Resourcenames)
            {
                if (rName.EndsWith(ZIPUtil))
                {
                    if (!Directory.Exists(ZIPExeFolderPath))
                        Directory.CreateDirectory(ZIPExeFolderPath);

                    ZIPExePath = ZIPExeFolderPath + ZIPUtil;
                    if (!File.Exists(ZIPExePath))
                    {
                        Stream sZIPExe = Assembly.GetExecutingAssembly().GetManifestResourceStream(rName);//.CopyTo(File.OpenWrite(GruntExec));
                        using (Stream fZIPExe = File.Create(ZIPExePath))
                        {
                            sZIPExe.CopyTo(fZIPExe);
                            fZIPExe.Close();
                        }
                        ZipFile.ExtractToDirectory(ZIPExePath, ZIPExeFolderPath);
                    }
                    return ZIPExeFolderPath + ZipExe;

                }

            }

            return string.Empty;

        }
    }
}