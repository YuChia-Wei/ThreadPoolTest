namespace ThreadsPoolTest.UseCases.CpuIntensive.Services;

public interface ICpuIntensiveService
{
    void DoWork(int iterations);
}
