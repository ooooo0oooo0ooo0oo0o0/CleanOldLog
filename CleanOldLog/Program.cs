using System;
using System.Configuration;
using System.IO;

namespace CleanOldLog
{
    class Program
    {
        static void Main()
        {
            var logPaths = ConfigurationManager.AppSettings["logPath"].Split(',');
            String extension = ConfigurationManager.AppSettings["extension"];
            String limitDayNumber = ConfigurationManager.AppSettings["limitDayNumber"];

            if (!int.TryParse(limitDayNumber, out int limitDay))
            {
                return;
            }

            // クリーンアップ対象とする境界の日時を生成
            TimeSpan timespan = new TimeSpan(limitDay, 0, 0, 0);
            DateTime limitDateTime = DateTime.Now - timespan;

            foreach(var logPath in logPaths)
            {
                try
                {
                    // 指定パス配下のファイルリストを取得
                    var dirInfo = new DirectoryInfo(@logPath);
                    var files = dirInfo.EnumerateFiles(("*." + extension + "*"), SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        if ((limitDateTime.Date - file.CreationTime.Date).Days > limitDay)
                        {
                            // 指定日時よりも前に生成されたファイルは削除する。
                            file.Delete();
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    // 指定したディレクトリが無い場合は、何もしない。
                }
            }
        }
    }
}
