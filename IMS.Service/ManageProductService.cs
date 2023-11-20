using IMS.DAO;
using IMS.Models;
using Microsoft.AspNetCore.Hosting.Server;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface IManageProductService
    {
        (string, string, string) ProcessDescription(string description, string targetFolderPath);
        string SaveDataUriAsImage(string dataUri, string targetFolderPath);
    }
    public class ManageProductService : IManageProductService
    {
        private readonly BaseDAO<Product> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public ManageProductService()
        {
            _repository = new BaseDAO<Product>();
        }
        public (string, string, string) ProcessDescription(string description, string targetFolderPath)
        {
            string pattern = "<img.*?src=[\"'](.*?)[\"'].*?>";
            var match = Regex.Match(description, pattern);

            if (match.Success)
            {
                string dataUri = match.Groups[1].Value;

                if (dataUri.StartsWith("data:image/"))
                {
                    byte[] imageBytes = Convert.FromBase64String(dataUri.Split(',')[1]);

                    if (imageBytes.Length > 5 * 1024 * 1024)
                    {
                        return (description, null, "Image size cannot exceed 5 MB.");
                    }
                    string imageUrl = SaveDataUriAsImage(dataUri, targetFolderPath);

                    // Remove the embedded image from the description
                    string processedDescription = description.Replace(match.Value, string.Empty);

                    // Remove extra line breaks and white spaces
                    processedDescription = processedDescription.Trim();

                    return (processedDescription, imageUrl, null);
                }
            }

            return (description, null, null);
        }

        public string SaveDataUriAsImage(string dataUri, string targetFolderPath)
        {
            // Extract the file extension from the data URI
            string extension = dataUri.Split(';')[0].Split('/')[1];

            // Create a unique file name
            string fileName = Guid.NewGuid() + "." + extension;

            // Get the base64-encoded image data
            string base64Data = dataUri.Split(',')[1];

            // Decode and save the image as a file
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            string imagePath = Path.Combine(targetFolderPath, fileName);
            System.IO.File.WriteAllBytes(imagePath, imageBytes);


            return fileName; 
        }

    }
}
