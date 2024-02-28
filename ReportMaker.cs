using System.Text;

namespace Delegates.Reports;

public class ReportMaker 
{
	private string Caption { get; } = null!;
    private readonly Func<string> BeginList = null!;
    private readonly Func<string> EndList = null!;
    private readonly Func<string, string> MakeCaption = null!;
    private readonly Func<string, string, string> MakeItem = null!;
    private readonly Func<IEnumerable<double>, string> MakeStatistics = null!;

    public ReportMaker(
        string caption,
        Func<string> beginList,
        Func<string> endList,
        Func<string, string> makeCaption,
        Func<string, string, string> makeItem,
        Func<IEnumerable<double>, string> makeStatistics)
    {
        Caption = caption;
        BeginList = beginList;
        EndList = endList;
        MakeCaption = makeCaption;
        MakeItem = makeItem;
        MakeStatistics = makeStatistics;
    }

    public string MakeReport(IEnumerable<Measurement> measurements)
	{
        var data = measurements.ToList();
		var result = new StringBuilder();
		result.Append(MakeCaption(Caption));
		result.Append(BeginList());
        result.Append(MakeItem("Temperature", MakeStatistics(data.Select(z => z.Temperature))));
		result.Append(MakeItem("Humidity", MakeStatistics(data.Select(z => z.Humidity))));
		result.Append(EndList());
		return result.ToString();
	}
}

public static class ReportMakerHelper
{
	public static string MeanAndStdHtmlReport(IEnumerable<Measurement> measurements)
	{
		return new ReportMaker(
                caption: "Mean and Std",
                beginList: () => "<ul>",
                endList: () => "</ul>",
                makeCaption: (caption) => $"<h1>{caption}</h1>",
                makeItem: (valueType, entry) => $"<li><b>{valueType}</b>: {entry}",
                makeStatistics: (d) =>
                {
                    var data = d.ToList();
                    var mean = data.Average();
                    var std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count - 1));

                    return new MeanAndStd
                    {
                        Mean = mean,
                        Std = std
                    }.ToString();
                }
                )
			.MakeReport(measurements);
    }

	public static string MedianMarkdownReport(IEnumerable<Measurement> measurements)
	{
        return new ReportMaker(
                caption: "Median",
                beginList: () => "",
                endList: () => "",
                makeCaption: (caption) => $"## {caption}\n\n",
                makeItem: (valueType, entry) => $" * **{valueType}**: {entry}\n\n",
                makeStatistics: (d) =>
                {
                    var list = d.OrderBy(z => z).ToList();
                    if (list.Count % 2 == 0)
                        return ((list[list.Count / 2] + list[list.Count / 2 - 1]) / 2).ToString();

                    return list[list.Count / 2].ToString();
                }
                )
            .MakeReport(measurements);
    }

	public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
	{
        return new ReportMaker(
                caption: "Mean and Std",
                beginList: () => "",
                endList: () => "",
                makeCaption: (caption) => $"## {caption}\n\n",
                makeItem: (valueType, entry) => $" * **{valueType}**: {entry}\n\n",
                makeStatistics: (d) =>
                {
                    var data = d.ToList();
                    var mean = data.Average();
                    var std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count - 1));

                    return new MeanAndStd
                    {
                        Mean = mean,
                        Std = std
                    }.ToString();
                }
                )
            .MakeReport(measurements);
    }

	public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
	{
        return new ReportMaker(
                caption: "Median",
                beginList: () => "<ul>",
                endList: () => "</ul>",
                makeCaption: (caption) => $"<h1>{caption}</h1>",
                makeItem: (valueType, entry) => $"<li><b>{valueType}</b>: {entry}",
                makeStatistics: (d) =>
                {
                    var list = d.OrderBy(z => z).ToList();
                    if (list.Count % 2 == 0)
                        return ((list[list.Count / 2] + list[list.Count / 2 - 1]) / 2).ToString();

                    return list[list.Count / 2].ToString();
                }
                )
            .MakeReport(measurements);
    }
}