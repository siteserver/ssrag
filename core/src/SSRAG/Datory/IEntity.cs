using System;

namespace SSRAG.Datory
{
    public interface IEntity : ICloneable
    {
        int Id { get; set; }

        string Uuid { get; set; }

        DateTime? CreatedDate { get; set; }

        DateTime? LastModifiedDate { get; set; }

        void Remove(string name);
    }
}
