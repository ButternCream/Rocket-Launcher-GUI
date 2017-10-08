using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketLauncher_GUI
{
    public class LogFile
    {
        private readonly StringBuilder _builder;
        private readonly bool _autoSave;
        private readonly string _filePath;

        public void WriteLine(string line)
        {
            _builder.AppendLine($"[{DateTime.Now.ToString()}] {line}");
            if(_autoSave)
                Save();
        }

        public void Clear()
        {
            _builder.Clear();
            if (_autoSave)
                Save();
        }

        public void Save()
        {
            File.WriteAllText(_filePath, _builder.ToString());
        }

        public LogFile(string filePath, bool autoSave = true)
        {
            _builder = new StringBuilder();
            _autoSave = autoSave;
            _filePath = filePath;

            //append to existing
            if(File.Exists(filePath))
                _builder.AppendLine(File.ReadAllText(filePath));
        }
    }
}
