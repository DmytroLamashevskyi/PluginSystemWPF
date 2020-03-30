using System;

namespace PluginLib
{
    public interface IPlugin
    {
        string Name { set; get; }
        string Author { set; get; }
        string Version { set; get; }
        string Code { set; get; } 

        void Calculate();
    }


}
