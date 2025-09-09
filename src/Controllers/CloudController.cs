using System;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]

public class CloudStorageController : ControllerBase
{
    private readonly S3Service _s3Service;

    public CloudStorageController(S3Service s3Service)
    {
        _s3Service = s3Service;
    }

    [HttpGet("generate-presigned-url")]
    public IActionResult GeneratePresignedUrl([FromQuery] string fileName, [FromQuery] string contentType)
    {
        var url = _s3Service.GeneratePreSignedUploadUrl(fileName, contentType);
        return Ok(new { url });
    }
}