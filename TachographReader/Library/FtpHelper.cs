namespace TachographReader.Library
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Connect.Shared.Models;

    public static class FtpHelper
    {
        private const string FtpPath = "ftp://ftp.webcalconnect.com/";

        public static void SaveDatabaseBackup(ServiceCredentials serviceCredentials, string filePath)
        {
            if (serviceCredentials == null)
            {
                throw new ArgumentNullException(nameof(serviceCredentials));
            }

            if (!ConnectHelper.IsConnectEnabled())
            {
                return;
            }

            var userId = serviceCredentials.UserId;

            if (!DirectoryExists(userId.ToString(), serviceCredentials.Username, serviceCredentials.Password))
            {
                CreateDirectory(userId.ToString(), serviceCredentials.Username, serviceCredentials.Password);
            }

            Upload(userId.ToString(), serviceCredentials.Username, serviceCredentials.Password, filePath);
        }

        private static bool DirectoryExists(string userId, string username, string password)
        {
            var response = GetResponse(WebRequestMethods.Ftp.ListDirectory, FtpPath, username, password);
            if (response == null)
            {
                throw new Exception("Unable to connect to backup server or error getting directory list.");
            }

            var filesAndDirectories = response.Split('\n', '\r');
            return filesAndDirectories.Where(c => !string.IsNullOrEmpty(c)).Where(fileOrDirectory => !fileOrDirectory.Contains('.')).Any(fileOrDirectory => fileOrDirectory == userId);
        }

        private static void CreateDirectory(string userId, string username, string password)
        {
            GetResponse(WebRequestMethods.Ftp.MakeDirectory, FtpPath + userId + "/", username, password);
        }

        private static string GetResponse(string method, string directory, string username, string password)
        {
            var request = WebRequest.Create(directory);
            request.Method = method;
            request.Credentials = new NetworkCredential(username, password);
            
            try
            {
                using (var resp = (FtpWebResponse) request.GetResponse())
                {
                    var stream = new StreamReader(resp.GetResponseStream());
                    return stream.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void Upload(string directoryName, string username, string password, string filePath)
        {
            var filename = $"{DateTime.Now.ToString("dd-MM-yyyy HHmm")}.sdf";
            var data = DocumentHelper.Zip(filePath);

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(username, password);
                client.UploadData(FtpPath + directoryName + "/" + filename, data);
            }
        }
    }
}