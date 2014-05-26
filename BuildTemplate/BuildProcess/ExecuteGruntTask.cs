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
    public sealed class ExecuteGruntTask : CodeActivity<string>
    {
        public InArgument<string> BuildPath { get; set; }
        const string GruntBAT = "ExecGrunt.bat";
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

            var GruntExe = BuildPath.Get(context) + "\\" + GruntBAT;
            string[] Resourcenames = this.GetType().Assembly.GetManifestResourceNames();
            GruntExec = "start...GruntExe:" + GruntExe;

            foreach (string rName in Resourcenames)
            {
                if (rName.EndsWith(GruntBAT))
                {
                    GruntExec = GruntExec + "_" + rName + "_" + Assembly.GetExecutingAssembly().GetManifestResourceStream(rName).Length.ToString();
                    Stream sRunGruntCli = Assembly.GetExecutingAssembly().GetManifestResourceStream(rName);//.CopyTo(File.OpenWrite(GruntExec));
                    using (Stream fRunGruntCli = File.Create(GruntExe))
                    {
                        sRunGruntCli.CopyTo(fRunGruntCli);
                        fRunGruntCli.Close();
                    }
                    return GruntExe;

                    //var proc = new Process
                    //{
                    //    StartInfo = new ProcessStartInfo
                    //    {
                    //        FileName = NPMInstallExe,
                    //        // Arguments = "command line arguments to your executable",
                    //        UseShellExecute = false,
                    //        RedirectStandardOutput = true,
                    //        CreateNoWindow = true,
                    //        RedirectStandardInput = true,
                    //        RedirectStandardError = true
                    //    }

                    //};

                    ////proc.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                    //proc.Start();
                    ////while (!proc.StandardOutput.EndOfStream)
                    ////{
                    ////    try
                    ////    {
                    ////        GruntExec = GruntExec + proc.StandardOutput.ReadLine();
                    ////        GruntExec = GruntExec + Environment.NewLine;
                    ////    }
                    ////    catch { }
                    ////}
                    //proc.WaitForExit();
                    //proc.Close();
                    //File.Delete(NPMInstallExe);

                }
            }
            GruntExec = GruntExec + "...end";

            return GruntExec;

        }
    }
}