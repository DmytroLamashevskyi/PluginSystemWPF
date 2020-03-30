using PluginLib;
using PluginSystemWPF.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PluginSystemWPF.MVVM.ViewModels
{
    public class PluginManagerViewModel : ViewModel
    {
        public string versionPlugin { get { return Plugins.FirstOrDefault(x => x.Name == SelectedPluginName)?.Version; } }
        public string autorPlugin { get { return Plugins.FirstOrDefault(x => x.Name == SelectedPluginName)?.Author; } }
        public string namePlugin { get { return Plugins.FirstOrDefault(x => x.Name == SelectedPluginName)?.Name; } }
        public string codePlugin { get { return Plugins.FirstOrDefault(x => x.Name == SelectedPluginName)?.Code; } }

        public ObservableCollection<String> PluginsList { get; set; }

        private string _selectedPluginName;
        public String SelectedPluginName { 
            set {
                _selectedPluginName = value;
                Notify("versionPlugin");
                Notify("autorPlugin");
                Notify("namePlugin"); 
                Notify("codePlugin");
            }
            get { return _selectedPluginName; } 
        }
        public ObservableCollection<IPlugin> Plugins { get; set; } 
        public ICommand ReloadCommand { set; get; }

        public PluginManagerViewModel()
        {
            ReloadCommand = new SimpleCommand(ReloadPlugins);
            Plugins = new ObservableCollection<IPlugin>();
            PluginsList = new ObservableCollection<string>();
            if (!Directory.Exists("./Plugins"))
            {
                Directory.CreateDirectory("./Plugins");
            }

            LoadAll();
            foreach(IPlugin plugin in Plugins)
            {
                PluginsList.Add(plugin.Name);
            }
            Notify("PluginsList");
        }

        private void ReloadPlugins()
        {
            SelectedPluginName = null;
            Plugins = new ObservableCollection<IPlugin>();
            PluginsList = new ObservableCollection<string>();
            LoadAll();
            foreach (IPlugin plugin in Plugins)
            {
                PluginsList.Add(plugin.Name);
            }
            Notify("PluginsList");
        }

        public void LoadAll()
        {
            String[] files = Directory.GetFiles("./Plugins/", "*.dll");

            foreach (var s in files)
                Load(Path.Combine(Environment.CurrentDirectory, s));

        }

        public void Load(String file)
        {
            if (!File.Exists(file) || !file.EndsWith(".dll", true, null))
                return;

            Assembly asm = null;
            string fileName = Path.GetFileNameWithoutExtension(file);
            try
            {
                asm = Assembly.LoadFile(file);
            }
            catch (Exception)
            {
                // unable to load
                return;
            }

            Type pluginInfo = null;
            try
            {
                Type[] types = asm.GetTypes();
                Assembly core = AppDomain.CurrentDomain.GetAssemblies().Single(x => x.GetName().Name.Equals(fileName));
                Type type = core.GetType($".PluginSystemWPF.Plugins.{fileName}"); 
                foreach (var t in types)
                    if (type.IsAssignableFrom((Type)t))
                    {
                        pluginInfo = t;
                        break;
                    }

                if (pluginInfo != null)
                {
                    Object o = Activator.CreateInstance(pluginInfo);
                    IPlugin plugin = (IPlugin)o;
                    Plugins.Add(plugin);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
