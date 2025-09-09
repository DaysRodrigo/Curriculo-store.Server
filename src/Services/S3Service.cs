using System;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly int _expiresInMinutes = 5;

    public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWS:BucketName"];
    }
    
    public string GeneratePreSignedUploadUrl(string fileName, string contentType)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(_expiresInMinutes)
        };

        var url = _s3Client.GetPreSignedURL(request);
        return url;
    }

    public string GetPublicUrl(string fileName)
    {
        return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
    }
}