using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSDesktop.common
{
    public static class GetVideoLength
    {
        public static string GetMediaTimeLen(string path)
        {
            try
            {
                Shell32.Shell shell = new Shell32.Shell();
                //文件路径               
                Shell32.Folder folder = shell.NameSpace(path.Substring(0, path.LastIndexOf("\\")));
                //文件名称             
                Shell32.FolderItem folderitem = folder.ParseName(path.Substring(path.LastIndexOf("\\") + 2));
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    return folder.GetDetailsOf(folderitem, 27);
                }
                else
                {
                    return folder.GetDetailsOf(folderitem, 21);
                }
            }
            catch (Exception ex) { return null; }
        }

        public static int GetMediaTimeLenSecond(string path)
        {
            try
            {
                Shell32.Shell shell = new Shell32.Shell();
                //文件路径               
                Shell32.Folder folder = shell.NameSpace(path.Substring(0, path.LastIndexOf("\\")));
                //文件名称             
                Shell32.FolderItem folderitem = folder.ParseName(path.Substring(path.LastIndexOf("\\") + 2));
                string len;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    len = folder.GetDetailsOf(folderitem, 27);
                }
                else
                {
                    len = folder.GetDetailsOf(folderitem, 21);
                }

                string[] str = len.Split(new char[] { ':' });
                int sum = 0;
                sum = int.Parse(str[0]) * 60 * 60 + int.Parse(str[1]) * 60 + int.Parse(str[2]);

                return sum;
            }
            catch (Exception ex) { return 0; }
        }
    }
}
