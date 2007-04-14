using System;
using System.IO;

namespace Rhino.Igloo
{
    /// <summary>
    /// Represent a single file uploaded by the user
    /// </summary>
    public class UploadedFile
    {
        private string name;
        private Stream stream;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public Stream Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFile"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="stream">The stream.</param>
        public UploadedFile(string name, Stream stream)
        {
            this.name = name;
            this.stream = stream;
        }

        /// <summary>
        /// Saves the stream to the given path
        /// </summary>
        /// <param name="path">The path.</param>
        public virtual void SaveAs(string path)
        {
            byte[] bytes = new BinaryReader(stream).ReadBytes((int)stream.Length);
            File.WriteAllBytes(path, bytes);
        }
    }
}