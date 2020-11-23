

namespace EdgeFramework
{
    public class SystemSytleFrameEvent : SytleFrameEvent
    {

        public SystemSytleFrameEvent(string title, string systemMethodName)
            : base(title)
        {
            SystemMethodName = systemMethodName;
        }

        public string SystemMethodName { get; set; }
    }

}
