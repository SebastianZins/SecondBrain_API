namespace SecondBrain.Utils
{
    public static class LogHelper
    {
        public static Exception LogError(Exception exception, string sectionName, string customException)
        {
            DateTime timestamp = DateTimeOffset.UtcNow.DateTime;
            string timeStampString = timestamp.ToString("yyyy.MM.dd HH:mm:ss"); ;
            Console.WriteLine($"[{timeStampString}] {exception.Message} {exception.StackTrace}");
            return new Exception("ERROR_MSG." + sectionName + "." + customException);
        }
    }
}
