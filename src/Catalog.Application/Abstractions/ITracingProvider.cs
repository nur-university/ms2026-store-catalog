using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Abstractions;

public interface ITracingProvider
{
    string GetCorrelationId();

    string GetTraceId();

    string GetSpanId();

    void SetCorrelationId(string correlationId);

    void SetTraceId(string traceId);
    void SetSpanId(string spanId);
}
