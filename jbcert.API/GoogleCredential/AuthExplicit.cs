using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jbcert.API.GoogleCredential
{
    public static class AuthExplicit
    {
        public static void CredentialRegistration()
        {
            string credential_path = Path.Combine(Directory.GetCurrentDirectory(), "GoogleCredential/OCR Project-c2a9107fb663.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

            //string credentialPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            //// Explicitly use service account credentials by specifying 
            //// the private key file.
            //var credential = GoogleCredential.FromFile(credential_path);
            //var storage = StorageClient.Create(credential);
            //// Make an authenticated API request.
            //var buckets = storage.ListBuckets("ocr-project-290410");
            //foreach (var bucket in buckets)
            //{
            //    Console.WriteLine(bucket.Name);
            //}
        }
    }
}
