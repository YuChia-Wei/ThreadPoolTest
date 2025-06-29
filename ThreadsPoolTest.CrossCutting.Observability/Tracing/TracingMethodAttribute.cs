using AspectInjector.Broker;

namespace ThreadsPoolTest.CrossCutting.Observability.Tracing;

[Injection(typeof(TracingMethodAspect))]
public class TracingMethodAttribute : Attribute
{
}