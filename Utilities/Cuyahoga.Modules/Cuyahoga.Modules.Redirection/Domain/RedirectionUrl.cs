using System;
using Cuyahoga.Core.Domain;

namespace Cuyahoga.Modules.Redirection.Domain
{
    public class RedirectionUrl
    {
        public RedirectionUrl()
        {
            Id = -1;
        }
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public virtual int NumberOfDownloads { get; set; }
        public virtual DateTime DatePublished { get; set; }
        public virtual Section Section { get; set; }
        public virtual User Publisher { get; set; }
        public virtual DateTime UpdateTimestamp { get; set; }
    }
}