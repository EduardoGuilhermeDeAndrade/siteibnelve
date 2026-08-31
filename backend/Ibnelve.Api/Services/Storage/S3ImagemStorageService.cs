using Amazon.S3;
using Amazon.S3.Model;

namespace Ibnelve.Api.Services.Storage;

/// <summary>
/// Implementação S3-compatível (funciona com MinIO em dev local e com S3/R2/Spaces reais em
/// produção — a troca é só de configuração: Storage:ServiceUrl, Storage:AccessKey/SecretKey).
/// </summary>
public class S3ImagemStorageService : IImagemStorageService
{
    private readonly IAmazonS3 _client;
    private readonly string _bucket;
    private readonly string _publicBaseUrl;

    public S3ImagemStorageService(IAmazonS3 client, IConfiguration configuration)
    {
        _client = client;
        _bucket = configuration["Storage:Bucket"]
            ?? throw new InvalidOperationException("Configuração ausente: Storage:Bucket");
        _publicBaseUrl = (configuration["Storage:PublicBaseUrl"] ?? $"http://localhost:9000/{_bucket}")
            .TrimEnd('/');
    }

    public async Task GarantirBucketAsync(CancellationToken ct = default)
    {
        var existe = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_client, _bucket);
        if (!existe)
        {
            await _client.PutBucketAsync(new PutBucketRequest { BucketName = _bucket }, ct);
        }

        // Bucket é criado privado por padrão; essas imagens são todas destinadas a exibição
        // pública no site (Hero, ministérios etc.), então precisam de leitura anônima.
        await _client.PutBucketPolicyAsync(_bucket, $$"""
            {
              "Version": "2012-10-17",
              "Statement": [
                {
                  "Effect": "Allow",
                  "Principal": "*",
                  "Action": ["s3:GetObject"],
                  "Resource": ["arn:aws:s3:::{{_bucket}}/*"]
                }
              ]
            }
            """, ct);
    }

    public async Task<string> SalvarAsync(string chave, Stream conteudo, string contentType, string extensao, CancellationToken ct = default)
    {
        var objectKey = $"{chave}/{Guid.NewGuid():N}{extensao}";

        await _client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = objectKey,
            InputStream = conteudo,
            ContentType = contentType
        }, ct);

        return $"{_publicBaseUrl}/{objectKey}";
    }
}
