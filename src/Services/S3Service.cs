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
        var extension = System.IO.Path.GetExtension(fileName);
        var randomName = Guid.NewGuid().ToString();
        var newName = randomName + extension;

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = newName,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(_expiresInMinutes)
        };

        var url = _s3Client.GetPreSignedURL(request);
        return url;
    }

    public string GetPublicUrl(string newName)
    {
        return $"https://{_bucketName}.s3.amazonaws.com/{newName}";
    }
}