namespace scrapy.net;

public class PriorityAttribute : Attribute
{
    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }

    public int Priority { get; private set; }
}


public record SpiderPipelineDescriptior(Type ItemType, int? Priority);