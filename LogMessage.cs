namespace WrightCover
{
    public class LogMessage
    {
        public enum LogType
        {
            [ImageName("warning.png")] Warning,
            [ImageName("exclamation.png")] Error,
            [ImageName("info.png")] Info,
            [ImageName("tick.png")] Success
        }

        public string Message { get; set; }
        public string ImageSource { get; set; }

        public static LogMessage Log(string message, LogType logType)
        {
            var attributes = typeof(LogType).GetMember(logType.ToString())[0].GetCustomAttributes(typeof(ImageNameAttribute), false);
            var imageName = ((ImageNameAttribute) attributes[0]).ImageName;

            return new LogMessage
            {
                Message = message,
                ImageSource = $"{Utilities.GetAssemblyDirectory}\\images\\{imageName}"
            };
        }
    }
}
