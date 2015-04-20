using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JenkinsToPivotal
{
    /// <summary>
    /// Jenkins Change Item Model.
    /// </summary>
    public class Item
    {
        [XmlElement("commitId")]
        public string CommitRevision { get; set; }
        [XmlElement("msg")]
        public string Message { get; set; }
        [XmlElement("user")]
        public string Author { get; set; }
        [XmlElement("date")]
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Jenkins Changes Model.
    /// </summary>
    [XmlRoot("changes")]
    public class Changes
    {
        [XmlElement("item")]
        public List<Item> Items { get; set; }
    }
}
