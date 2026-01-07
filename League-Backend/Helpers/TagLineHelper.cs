namespace League_Backend.Helpers
{
    public static class TagLineHelper
    {
        public static string TryStripHashtagFromTagLine(string tagLine)
        {
            if (tagLine.StartsWith('#'))
            {
                tagLine = tagLine[1..];
            }
            return tagLine;
        }
    }
}
