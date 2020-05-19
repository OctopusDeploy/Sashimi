using Amazon.S3.Model;
using Octopus.CoreUtilities;

namespace Calamari.Aws.Integration.S3
{
    public class S3UploadResult
    {
        PutObjectRequest Request { get; }
        Maybe<PutObjectResponse> Response { get; }

        public S3UploadResult(PutObjectRequest request, Maybe<PutObjectResponse> response)
        {
            Request = request;
            Response = response;
        }

        public bool IsSuccess()
        {
            return !Response.None();
        }

        public string BucketKey => Request.Key;

        /// <summary>
        /// Return the version from the underlying PutObjectResponse or some user friendly string if the bucket 
        /// is not versioned.
        /// </summary>
        public string Version
        {
            get
            {
                if (Response.Some())
                {
                    return string.IsNullOrEmpty(Response.Value.VersionId) ? "Not versioned" : Response.Value.VersionId;
                }

                return null;
            }
        }
    }
}