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

namespace GruntBuildProcess
{
    public sealed class ExecuteNPMInstall : CodeActivity<string>
    {
        public InArgument<string> BuildPath { get; set; }
        const string GruntBAT = "RunNPMInstall.bat";
        string GruntExec = Path.GetTempFileName();

        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Collect the sort command output. 
            if (!String.IsNullOrEmpty(e.Data))
            {
                GruntExec = GruntExec + e.Data + Environment.NewLine;
            }
        }

        protected override string Execute(CodeActivityContext context)
        {
            var NPMInstallExe = BuildPath.Get(context) + "\\" + GruntBAT;

            string[] Resourcenames = this.GetType().Assembly.GetManifestResourceNames();
            GruntExec = "start...NPMInstallExe:" + NPMInstallExe;

            foreach (string rName in Resourcenames)
            {
                if (rName.EndsWith(GruntBAT))
                {
                    GruntExec = GruntExec + "_" + rName + "_" + Assembly.GetExecutingAssembly().GetManifestResourceStream(rName).Length.ToString();
                    Stream sRunGruntCli = Assembly.GetExecutingAssembly().GetManifestResourceStream(rName);//.CopyTo(File.OpenWrite(GruntExec));
                    using (Stream fRunGruntCli = File.Create(NPMInstallExe))
                    {
                        sRunGruntCli.CopyTo(fRunGruntCli);
                        fRunGruntCli.Close();
                    }
                    return NPMInstallExe;

                   // StreamWriter sw;
                   // StreamReader sr;
                   // StreamReader err;
                   // var proc = new Process
                   // {
                   //     StartInfo = new ProcessStartInfo
                   //     {
                   //         FileName = NPMInstallExe,
                   //         // Arguments = "command line arguments to your executable",
                   //         UseShellExecute = false,
                   //         RedirectStandardOutput = true,
                   //         CreateNoWindow = true,
                   //         RedirectStandardInput = true,
                   //         RedirectStandardError = true
                   //     }
                        
                   // };

                   //// proc.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                   // proc.Start();
                   // //while (!proc.StandardOutput.EndOfStream)
                   // //{
                   // //    try
                   // //    {
                   // //        GruntExec = GruntExec + proc.StandardOutput.ReadLine();
                   // //        GruntExec = GruntExec + Environment.NewLine;
                   // //    }
                   // //    catch { }
                   // //}
                   // proc.WaitForExit();
                   // proc.Close();
                    //File.Delete(NPMInstallExe);

                }
            }
            //GruntExec = GruntExec + "...end";

            return string.Empty;

        }
    }
}