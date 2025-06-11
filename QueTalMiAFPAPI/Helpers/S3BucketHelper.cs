using Amazon.S3;
using Amazon.S3.Model;
using System.Text;

namespace QueTalMiAFPAPI.Helpers {
    public class S3BucketHelper(IAmazonS3 amazonS3) {

        public async Task<string> UploadFile(string contenido) {
            string bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME_LARGE_RESPONSES") ?? throw new ArgumentNullException("BUCKET_NAME_LARGE_RESPONSES");
            string keyName = Guid.NewGuid().ToString();

            using MemoryStream stream = new();
            using StreamWriter writer = new(stream, Encoding.UTF8);
            writer.Write(contenido);
            writer.Flush();
            stream.Position = 0;

            PutObjectRequest request = new() {
                BucketName = bucketName,
                Key = keyName,
                InputStream = stream,
            };

            PutObjectResponse response = await amazonS3.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                return keyName;
            }

            throw new Exception("Ocurrió un error al subir la respuesta al bucket de S3.");
        }
    }
}
