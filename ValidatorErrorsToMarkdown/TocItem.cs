namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Creates an object TocItem.
    /// </summary>
    internal class TocItem
    {
        private string topicUid;
        private string name;

        public string Path { get; }

        public string Name => name ??= GetName();

        public string TopicUid => topicUid ??= GetTopicUid();

        /// <summary>
        /// Creates an instance of class <see cref="TocItem"/>.
        /// </summary>
        /// <param name="path"></param>
        public TocItem(string path)
        {
            Path = path;
        }

        private protected virtual string GetName()
        {
            string[] text = File.ReadAllLines(Path);
            return text.Skip(6).Take(1).First()[3..];
        }

        private protected virtual string GetTopicUid()
        {
            return Path.Split("/").Last().Split(".").First();
        }

        public virtual void Build(StringBuilder tocContent, int level)
        {
            tocContent.Append(GetSpacesBasedOnLevel(level));
            tocContent.AppendLine($"- name: {Name}");

            tocContent.Append(GetSpacesBasedOnLevel(level));
            tocContent.AppendLine($"  topicUid: {TopicUid}");
        }

        internal static StringBuilder GetSpacesBasedOnLevel(int level)
        {
            StringBuilder spaces = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                spaces.Append("  ");
            }

            return spaces;
        }
    }
}
