using System;
using System.IO;
using Newtonsoft.Json;
using WindFrostBot.SDK;

namespace JsonTool
{
    public class JsonRw<T> where T : new()
    {
        public T ConfigObj { get; private set; }
        public string ConfigPath { get; private set; }
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<CreatingEvent> OnCreating;
        public JsonRw(string path, T initialConfig)//有初始类
        {
            ConfigPath = Path.Combine(AppContext.BaseDirectory, path);
            ConfigObj = initialConfig;
            ReadConfig();
        }
        public JsonRw(string path)//无初始类
        {
            ConfigPath = Path.Combine(AppContext.BaseDirectory, path);
            ConfigObj = new();
            ReadConfig();
        }
        public bool Exists()
        {
            return File.Exists(ConfigPath);
        }
        protected virtual void ErrorOccurred(Exception ex)
        {
            OnError?.Invoke(this , new ErrorEventArgs(ex));
        }
        protected virtual void CreatingOccurred(string text)
        {
            OnCreating?.Invoke(this, new CreatingEvent(text));
        }
        public void ReadConfig()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    CreatingOccurred("未找到初始配置!为您生成默认配置...");
                }
                else
                {
                    using (StreamReader file = File.OpenText(ConfigPath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        ConfigObj = (T)serializer.Deserialize(file, typeof(T));
                    }
                }
                WriteConfig();
            }
            catch(Exception ex)
            {
                Message.Erro(ex.ToString());
                ErrorOccurred(ex);
            }
        }
        public void WriteConfig()
        {
            FileStream fileStream = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.Write);
            string value = JsonConvert.SerializeObject(ConfigObj, Formatting.Indented);
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(value);
            }
        }
    }
    public class CreatingEvent : EventArgs
    {
        public string Text { get; set; }
        public CreatingEvent(string text)
        {
            Text = text;
        }   
    }
}
