namespace Taka.Models
{
    public class TakaLabel
    {
        public TakaLabel()
        {

        }

        public TakaLabel(string key, string value) : this()
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}