using Microsoft.CSharp;
using PluginLib;
using PluginSystemWPF.MVVM.Models;
using System;
using System.CodeDom.Compiler;
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
    public class CompillerViewModel: ViewModel
    {
        private string _code ;
        public string SourceCode
        {
            get { return _code; }
            set { _code = value; }
        }

        public string PluginName { set; get; }
        public string PluginVersion { set; get; }
        public string PluginAutor { set; get; }
        public ObservableCollection<String> ResultConsole { set; get; }
        public ICommand GenerateCodeCommand { set; get; }
        public CompillerViewModel()
        {
            SourceCode = "{ \n//Place youre code here \n}";
            Notify("SourceCode"); 
            ResultConsole = new ObservableCollection<string>();
            GenerateCodeCommand = new SimpleCommand(GenerateCode);
        }
        
        public void GenerateCode()
        {
            ResultConsole.Clear();
            if (string.IsNullOrEmpty(PluginName))
            {
                ResultConsole.Add("Enter plugin name.");
                return;
            }

            if (!Directory.Exists("./Plugins"))
            {
                Directory.CreateDirectory("./Plugins");
            }

            string codeToCompile = $@" 
            namespace PluginSystemWPF.Plugins
            {{
            public class {PluginName}:PluginLib.IPlugin  {{ 
                     
                public string Name {{ set; get; }}
                public string Author {{ set; get; }}
                public string Version {{ set; get; }}
                public string Code {{ set; get; }}        


                public {PluginName}(){{
                
                    Name = ""{PluginName}""; 
                    Version= ""{PluginVersion}"";  
                    Code = @""{SourceCode.Replace("\"","\"\"")}"";  
                    Author=  ""{PluginAutor}"";
                }} 

                public void Calculate()
                {{  
                    {SourceCode}
                }}  

            }} 
            }}";
             
             

            var codeProvider = new CSharpCodeProvider(); 
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location); 
            parameters.ReferencedAssemblies.Add(typeof(IPlugin).Assembly.Location); 
 
            parameters.OutputAssembly = $"./Plugins/{PluginName}.dll";
            CompilerResults rresults = codeProvider.CompileAssemblyFromSource(parameters, codeToCompile);
            if(rresults.Errors.Count > 0)
            {
                foreach (var error in rresults.Errors)
                {
                    ResultConsole.Add(error.ToString());
                }
            }
            else
            {
                ResultConsole.Add("Compiller sucsess."); 
            } 
             
        }
    }
}
