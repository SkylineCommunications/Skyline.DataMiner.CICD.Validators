namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal class TocFolder : TocItem
    {
        public List<TocItem> Children { get; set; }

        public TocFolder(string path) : base(path)
        {
            Children = new List<TocItem>();

            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories)
            {
                Children.Add(new TocFolder(dir));
            }

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                Children.Add(new TocItem(file));
            }
        }

        private protected override string GetName()
        {
            return Path.Split("/").Last();
        }

        private protected override string GetTopicUid()
        {
            return FindFirstChildWithTopicUid(this);
        }

        private static string FindFirstChildWithTopicUid(TocItem child)
        {
            if (child is TocFolder folder)
            {
                return folder.Children.Count == 0 ? String.Empty : FindFirstChildWithTopicUid(folder.Children[0]);
            }

            return child.TopicUid;
        }

        /// <summary>
        /// Builds a tocfile of an TocItem.
        /// </summary>
        /// <param name="tocContent"></param>
        /// <param name="level"></param>
        public override void Build(StringBuilder tocContent, int level = 0)
        {
            tocContent.Append(GetSpacesBasedOnLevel(level));
            tocContent.AppendLine($"- name: {Name}");

            tocContent.Append(GetSpacesBasedOnLevel(level));
            tocContent.AppendLine("  items:");

            level++;
            foreach (TocItem child in Children)
            {
                child.Build(tocContent, level);
            }
        }
    }
}