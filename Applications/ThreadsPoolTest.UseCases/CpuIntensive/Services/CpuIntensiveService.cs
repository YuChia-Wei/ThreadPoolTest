using System.Security.Cryptography;
using ThreadsPoolTest.CrossCutting.Observability.Tracing;

namespace ThreadsPoolTest.UseCases.CpuIntensive.Services;

[TracingMethod]
public class CpuIntensiveService : ICpuIntensiveService
{
    public void DoWork(int iterations)
    {
        for (var i = 0; i < iterations; i++)
        {
            // Simulate a CPU-intensive operation by repeatedly creating a key derivative.
            // This is a classic CPU-bound task that will occupy a thread.
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(i.ToString(), salt, 10000, HashAlgorithmName.SHA256);
            pbkdf2.GetBytes(256 / 8);
        }
    }
}