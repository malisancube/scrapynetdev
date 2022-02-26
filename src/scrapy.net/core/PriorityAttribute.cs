namespace scrapy.net;

/// <summary>
/// The priority attribute to set the order of execution of pipelines
/// </summary>
public class PriorityAttribute : Attribute
{
    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }

    public int Priority { get; private set; }
}

internal record SpiderPipelineDescriptior(Type ItemType, int? Priority);