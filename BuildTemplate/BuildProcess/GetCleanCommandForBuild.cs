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
    public sealed class GetCleanCommandForBuild : CodeActivity<string>
    {
        public InArgument<string> BuildPath { get; set; }
        const string CleanBAT = "CleanBuild.bat";
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

            var CleanExe = BuildPath.Get(context) + "\\" + CleanBAT;
            // var CleanExe = @"C:\Builds\1\GruntSampleAppProject\GruntBuild\src\NodejsApp1\" + CleanBAT;
            string[] Resourcenames = this.GetType().Assembly.GetManifestResourceNames();
            GruntExec = "start...GruntExe:" + CleanExe;

            foreach (string rName in Resourcenames)
            {
                if (rName.EndsWith(CleanBAT))
                {
                    GruntExec = GruntExec + "_" + rName + "_" + Assembly.GetExecutingAssembly().GetManifestResourceStream(rName).Length.ToString();
                    Stream sRunGruntCli = Assembly.GetExecutingAssembly().GetManifestResourceStream(rName);//.CopyTo(File.OpenWrite(GruntExec));

                    if (!Directory.Exists(CleanExe))
                    {
                        FileInfo batFile = new FileInfo(CleanExe);
                        string dirPath = batFile.Directory.FullName;
                        System.IO.Directory.CreateDirectory(dirPath);
                    }

                    CleanExe = CleanExe.Replace("CleanBuild.bat", "zzzzCleanBuild.bat");

                    using (Stream fRunGruntCli = File.Create(CleanExe))
                    {
                        sRunGruntCli.CopyTo(fRunGruntCli);
                        fRunGruntCli.Close();
                    }
                    return CleanExe;
                }

            }

            return string.Empty;

        }
    }
}