using System.Collections.Generic;
using System.IO;
using Telerik.Reporting;
using Telerik.Reporting.Services;

namespace HRMvc.StartupConfig; 

// ✅ Minimal Resolver: Only loads TRDP file from /Reports folder
public class CustomReportSourceResolver : IReportSourceResolver
{
    private readonly string _reportsPath;

    public CustomReportSourceResolver(string reportsPath)
    {
        _reportsPath = reportsPath;
    }

    public ReportSource Resolve(string reportId, OperationOrigin operationOrigin, IDictionary<string, object> currentParameters)
    {
        var reportFile = Path.Combine(_reportsPath, reportId);

        if (!File.Exists(reportFile))
            throw new FileNotFoundException($"Report file not found: {reportFile}");

        var packager = new ReportPackager();
        Telerik.Reporting.Report report;
        using (var stream = File.OpenRead(reportFile))
            report = (Telerik.Reporting.Report)packager.UnpackageDocument(stream);

        return new InstanceReportSource { ReportDocument = report };
    }
}
