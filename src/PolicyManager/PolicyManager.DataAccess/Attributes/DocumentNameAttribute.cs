using System;

namespace PolicyManager.DataAccess.Attributes
{
    public class DocumentNameAttribute
        : Attribute
    {
        public DocumentNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
