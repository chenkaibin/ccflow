using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Net;


namespace BP.Web.Ctrl
{
    public class Glo
    {
        public void DownFileByPath(string filepath, string filename)
        {
            if (null == filepath || filepath.Trim().Length < 1) return;
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;
            if (null == filename || filename.Trim().Length < 1) filename = Path.GetFileName(filepath);

            System.IO.Stream iStream = null;

            // Buffer to read 10K bytes in chunk:
            byte[] buffer = new Byte[10240];

            // Length of the file:
            int length;

            // Total bytes to read:
            long dataToRead;

            // Identify the file to download including its path.
            if (!File.Exists(filepath))
            {
            response.Write("要下载的文件不存在!"+filepath);
                return;
            }
            try
            {
                
                // Open the file.
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                response.Clear();

                // Total bytes to read:
                dataToRead = iStream.Length;

                long p = 0;
                if (request.Headers["Range"] != null)
                {
                    response.StatusCode = 206;
                    p = long.Parse(request.Headers["Range"].Replace("bytes=", "").Replace("-", ""));
                }
                if (p != 0)
                {
                    response.AddHeader("Content-Range", "bytes " + p.ToString() + "-" + ((long)(dataToRead - 1)).ToString() + "/" + dataToRead.ToString());
                }
                response.AddHeader("Content-Length", ((long)(dataToRead - p)).ToString());
                response.ContentType = "application/octet-stream";
                //response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(request.ContentEncoding.GetBytes(filename)));
                //response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(filename, Encoding.Unicode));
                //response.Charset = "gb2312";
                response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(filename, Encoding.GetEncoding("GB2312")));
                iStream.Position = p;
                dataToRead = dataToRead - p;
                // Read the bytes.
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (response.IsClientConnected)
                    {
                        // Read the data in buffer.
                        length = iStream.Read(buffer, 0, 10240);

                        // Write the data to the current output stream.
                        response.OutputStream.Write(buffer, 0, length);

                        // Flush the data to the HTML output.
                        response.Flush();

                        buffer = new Byte[10240];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    //Close the file.
                    iStream.Close();
                }
                response.End();
            }
        }

        public void DownFileByURL(string url, string filename)
        {
            if (null == url || url.Trim().Length < 1) return;

            string filepath = HttpContext.Current.Server.MapPath(url);
            DownFileByPath(filepath, filename);
        }

        public static string DealStrLength(string str, int maxLen)
        {
            if (str.Length > maxLen)
                return str.Substring(0, maxLen);
            return str;
        }
        public static string NetDiskFtpIP
        {
            get
            {
                return SystemConfig.AppSettings["NetDiskFtpIP"];
            }
        }

        public static string NetDiskFtpUser
        {
            get
            {
                return SystemConfig.AppSettings["NetDiskFtpUser"];
            }
        }

        public static string NetDiskFtpPass
        {
            get
            {
                return SystemConfig.AppSettings["NetDiskFtpPass"];
            }
        }


        public static void DownFile(string filepath, string fileName, HttpRequest request, HttpResponse response)
        {
            System.IO.Stream iStream = null;
            // Buffer to read 10K bytes in chunk:string
            byte[] buffer = new Byte[10240];
            // Length of the file:
            int length;
            // Total bytes to read:
            long dataToRead;

            // Identify the file to download including its path.
            if (!File.Exists(filepath)) return;

            // Identify the file name.
            string filename = Path.GetFileName(filepath);

            try
            {
                // Open the file.
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                response.Clear();

                // Total bytes to read:
                dataToRead = iStream.Length;

                long p = 0;
                if (request.Headers["Range"] != null)
                {
                    response.StatusCode = 206;
                    p = long.Parse(request.Headers["Range"].Replace("bytes=", "").Replace("-", ""));
                }
                if (p != 0)
                {
                    response.AddHeader("Content-Range", "bytes " + p.ToString() + "-" + ((long)(dataToRead - 1)).ToString() + "/" + dataToRead.ToString());
                }
                response.AddHeader("Content-Length", ((long)(dataToRead - p)).ToString());
                response.ContentType = "application/octet-stream";
                response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);

                iStream.Position = p;
                dataToRead = dataToRead - p;
                // Read the bytes.
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (response.IsClientConnected)
                    {
                        // Read the data in buffer.
                        length = iStream.Read(buffer, 0, 10240);

                        // Write the data to the current output stream.
                        response.OutputStream.Write(buffer, 0, length);

                        // Flush the data to the HTML output.
                        response.Flush();

                        buffer = new Byte[10240];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    //Close the file.
                    iStream.Close();
                }
                response.End();
            }
        }
    }
}
