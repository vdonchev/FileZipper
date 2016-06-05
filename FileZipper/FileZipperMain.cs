namespace FileZipper
{
    using System;
    using IO;

    public static class FileZipperMain
    {
        [STAThread]
        public static void Main()
        {
            var io = new Io();

            var app = new App(io);
            app.Run();
        }
    }
}
