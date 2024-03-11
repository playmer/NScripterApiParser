// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.Xml;

//class Function
//{
//    string name;
//};


List<string> SplitParameters(string parametersStr)
{
    var parameters = new List<string>();

    int min = int.MaxValue;
    do
    {
        var comma = parametersStr.IndexOf(',');
        var pipe = parametersStr.IndexOf('|');
        var arrayOpen = parametersStr.IndexOf('[');
        comma = comma == -1 ? int.MaxValue : comma;
        pipe = pipe == -1 ? int.MaxValue : pipe;
        arrayOpen = arrayOpen == -1 ? int.MaxValue : arrayOpen;

        min = Math.Min(Math.Min(comma, pipe), arrayOpen);

        if (min == comma)
        {

        }
        else if (min == pipe)
        {

        }
        else if (min == arrayOpen)
        {

        }

    } while (min != int.MaxValue);

    return parameters;
}

void RecursiveRun(List<NodeRef> nodes,  HtmlNode? node, int tabLevel)
{
    if (node == null)
        return;

    ++tabLevel;


    //var processedInnerText = node.InnerText.Trim();


    nodes.Add(new NodeRef(node, tabLevel));

    foreach (var child in node.ChildNodes)
    {
        RecursiveRun(nodes, child, tabLevel);
    }
}


using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
{
    var builder = new StringBuilder();
    string html = client.DownloadString("https://kaisernet.org/onscripter/api/NScrAPI.html");


    var fullHtmlDoc = new HtmlDocument();
    fullHtmlDoc.LoadHtml(html);

    //XmlDocument xmlDoc = new XmlDocument();
    //xmlDoc.LoadXml(html);

    var htmlSplit = html.Split("<div id=\"MAIN\">");

    var categories = htmlSplit[0];
    var functions = htmlSplit[1].Split("<hr><a href=\"#top\">page top</a> / <a href=\"#LIST\">list</a> / <a href=\"#MAIN\">main</a><br></div>");

    var functionHtmls = new List<HtmlDocument>();

    int i = 0;
    foreach (var function in functions)
    {
        var doc = new HtmlDocument();
        var trimmedFunction = function.Trim();

        if (trimmedFunction.Length == 0)
            continue;

        doc.LoadHtml(trimmedFunction);

        functionHtmls.Add(doc);
        //var description = doc.DocumentNode.ChildNodes[doc.DocumentNode.ChildNodes.Count - 1];
        //var functionName = description.ChildNodes[description.ChildNodes.Count - 2];
        //Console.WriteLine($"Count: {doc.DocumentNode.ChildNodes.Count}; {functionName.InnerText}; ({i++})");
        //Console.WriteLine($"{i++}");

        builder.AppendLine($"{i++}");

        foreach (var child in doc.DocumentNode.ChildNodes)
        {
            var outerHtml = child.OuterHtml.Replace("\r\n", "");
            var innerHtml = child.InnerHtml.Replace("\r\n", "");
            if (outerHtml.StartsWith("<div class=\"Arguments\">"))
            {
                builder.AppendLine($"\tArguments");

                foreach (var argInfo in child.ChildNodes)
                {
                    builder.AppendLine($"\t\t{argInfo.Name}, {argInfo.ChildNodes.Count}, {argInfo.OuterHtml}");
                }
            }
            else
            {
                builder.AppendLine($"\t{child.Name}: {outerHtml}");
                builder.AppendLine($"\t\t{innerHtml}");
            }
        }

        //foreach (var child in doc.DocumentNode.ChildNodes[1].ChildNodes)
        //{
        //}
    }
    //var doc = new HtmlDocument();
    //doc.LoadHtml(html);
    //
    //var nodes = new List<NodeRef>();
    //
    //foreach (var child in doc.DocumentNode.ChildNodes)
    //{
    //    RecursiveRun(nodes, child, 0);
    //}
    //
    //
    //foreach (var node in nodes)
    //{
    //    if (node.node.Name == "html" 
    //        || node.node.Name == "html"
    //        || (node.node.Name == "body" && node.node.InnerText.Trim().StartsWith("Last edited 20")))
    //        continue;
    //
    //    if (node.node.InnerText.Trim().Length == 0)
    //        continue;
    //
    //    var tabs = new String('\t', node.tabLevel);
    //    builder.AppendLine($"{tabs}{node.node.Name}: \"{node.node.InnerText}\"");
    //}


    File.WriteAllText("C:/Users/playmer/NScrAPI.html", html);
    File.WriteAllText("C:/Users/playmer/NScrAPI_formatted.html", fullHtmlDoc.ParsedText);
    File.WriteAllText("C:/Users/playmer/NScrAPI_output.txt", builder.ToString());

    //foreach (var child in doc.DocumentNode.ChildNodes)
    //{
    //    Console.WriteLine($"{child.InnerText}");
    //    foreach (var child2 in child.ChildNodes)
    //    {
    //        Console.WriteLine($"\t{child2.InnerText}");
    //        foreach (var child3 in child2.ChildNodes)
    //        {
    //            Console.WriteLine($"\t\t{child3.InnerText}");
    //        }
    //    }
    //}

    //Console.Write(html);
}



struct NodeRef
{
    public HtmlNode node;
    public int tabLevel = 0;

    public NodeRef(HtmlNode node, int tabLevel)
    {
        this.node = node;
        this.tabLevel = tabLevel;
    }
}

enum ParameterType
{
    String,
    Number,
    StringOrNumber,
    StringVariable,
    NumberVariable
}

struct Parameter
{
    public string Name;
    public ParameterType Type;
    public bool IsOptional;

    public Parameter(string name, ParameterType type, bool isOptional)
    {
        Name = name;
        Type = type;
        IsOptional = isOptional;
    }
}


struct Function
{
    public string Name;
    public List<Parameter> Parameters;

    public Function(string name, List<Parameter> parameters)
    {
        Name = name;
        Parameters = parameters;
    }
}