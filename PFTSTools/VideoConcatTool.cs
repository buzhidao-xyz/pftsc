using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PFTSTools
{
    public class VideoConcatTool
    {
        public Action<int, int> ReportProgress;

        public void Concat(IList<string> videos, string outName)
        {
            string ffmpeg = "ffmpeg.exe";
            string arguments = "";

            List<string> temps = new List<string>();
            int total = videos.Count + 2;
            int idx = 0;

            foreach (var v in videos)
            {
                string tempFile = GetTempFileName(".mpg");
                File.Delete(tempFile);
                arguments = " -i \"" + v + "\" -qscale 6 " + "\"" + tempFile + "\"";
                ProcessStartInfo psi = new ProcessStartInfo(ffmpeg, arguments);
                Process p = Process.Start(psi);
                p.WaitForExit();
                temps.Add(tempFile);

                if (ReportProgress != null)
                {
                    idx++;
                    ReportProgress(idx, total);
                }
            }
            arguments = " /b";
            var coma = "";
            bool start = true;
            foreach (var t in temps)
            {
                if (!start)
                {
                    coma += "+\"" + t + "\"";
                }
                else
                {
                    start = false;
                    coma = "\"" + t + "\"";
                }
            }
            //copy
            string outCtempFile = GetTempFileName(".mpg");
            File.Delete(outCtempFile);
            var cmd = "copy /b " + coma + " \"" + outCtempFile + "\"";
            ExcuteXCopyCmd(cmd);
            if (ReportProgress != null)
            {
                idx++;
                ReportProgress(idx, total);
            }
            //combine
            arguments = " -i " + outCtempFile + " -qscale 6 " + outName;
            ProcessStartInfo psie = new ProcessStartInfo(ffmpeg, arguments);
            psie.CreateNoWindow = true;
            Process pe = Process.Start(psie);
            pe.WaitForExit();
            if (ReportProgress != null)
            {
                idx++;
                ReportProgress(idx, total);
            }

            foreach (var t in temps)
            {
                File.Delete(t);
            }
            File.Delete(outCtempFile);
        }

        public string GetTempFileName(string extension)
        {
            string tempFileName = Path.GetTempFileName();
            string newTempFileName = Path.ChangeExtension(tempFileName, extension);
            File.Move(tempFileName, newTempFileName);
            return newTempFileName;
        }

        public string GetTempFileName(string prefix, string extension)
        {
            return GetTempFileName(prefix, extension, null);
        }


        /// <summary>
        /// 生成临时文件
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="extension">文件后缀名，包含前导句点('.')</param>
        /// <param name="directory">指定在该目录下生成，默认用户目录下的临时目录</param>
        /// <returns>临时文件的完整路径</returns>
        public string GetTempFileName(string prefix, string extension, string directory)
        {
            string tempFileName = string.Empty;
            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.GetTempPath();
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            tempFileName = prefix + Guid.NewGuid().ToString() + extension;
            tempFileName = Path.Combine(directory, tempFileName);
            FileStream fs = new FileInfo(tempFileName).Create();
            fs.Close();
            return tempFileName;
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sCmd"></param>
        private void ExcuteXCopyCmd(string sCmd)
        {
            System.Diagnostics.Process proIP = new System.Diagnostics.Process();
            proIP.StartInfo.FileName = "cmd.exe";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;
            proIP.Start();
            proIP.StandardInput.WriteLine(sCmd);
            proIP.StandardInput.WriteLine("exit");
            string strResult = proIP.StandardOutput.ReadToEnd();
            //lstLog.Items.Insert(0, string.Format("【{0}】子目录批量复制命令结果：", strResult));
            //Log(string.Format("【{0}】子目录批量复制命令结果", strResult));
            proIP.Close();

        }
    }
}
